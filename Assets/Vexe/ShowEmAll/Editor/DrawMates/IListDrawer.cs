using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using System.Collections;
using System.Reflection;
using System;
using Vexe.EditorHelpers;
using Vexe.RuntimeHelpers;
using Vexe.RuntimeExtensions;
using Object = UnityEngine.Object;
using System.Text;
using System.Linq;

namespace ShowEmAll.DrawMates
{
	public class IListDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private IList list;
		private FieldInfo info;
		private bool advancedMode;
		private AdvancedAreaData data;

		public bool advancedCollection { get; set; }
		private AdvancedAreaData Data { get { return RTHelper.LazyValue(() => data, d => data = d); } }
		private Type IListType { get { return info.FieldType; } }
		private Type ElementType { get { return IsArray ? IListType.GetElementType() : IListType.GetProperty("Item").PropertyType; } }
		private bool IsArray { get { return IListType.IsArray; } }
		protected override string key { get { return base.key + info.Name; } }

		public IListDrawer(TWrapper gui, FieldInfo iListInfo, Object target)
			: base(gui, target)
		{
			Set(iListInfo);
		}

		public IListDrawer() { }

		public void Set(FieldInfo iListInfo)
		{
			info = iListInfo;
		}

		public void Draw(TWrapper gui, FieldInfo iListInfo, Object target)
		{
			Set(gui, target);
			Set(iListInfo);
			Draw();
		}

		public override void Draw()
		{
			gui.Foldout(GetListName(), foldout, new TOption { ExpandWidth = true }, newValue =>
			{
				if (foldout != newValue)
				{
					foldout = newValue;
					HeightHasChanged();
				}
			});

			if (!foldout) return;

			gui.VerticalBlock(GUI.skin.box, () =>
			{
				list = GetList();

				DrawHeader(@add: AddToList);

				if (advancedMode)
					DrawAdvancedArea();

				gui.Splitter();

				if (list.Count == 0)
				{
					gui.HelpBox("Collection is empty", MessageType.Info);
					return;
				}

				gui.IndentedBlock(GUI.skin.box, () =>
				{
					for (int iLoop = 0; iLoop < list.Count; iLoop++)
					{
						int i = iLoop; // avoid any potential closure demons
						var current = list[i];
						DrawField(
							@numeric: i,
							@value: current,
							@setValue: newValue =>
							{
								if (list[i] != newValue)
									SetListElementAtIndex(i, newValue);
							},
							@moveDown: () => list.MoveElementDownNonGen(i),
							@moveUp: () => list.MoveElementUpNonGen(i),
							@remove: () => RemoveListElementAtIndex(i)
						);
					}
				});
			});

			if (typeof(Object).IsAssignableFrom(ElementType))
				DrawAddingArea();

			ApplyList();
		}

		private class AdvancedAreaData
		{
			public int index;
			public object value;
			public object valueToCheck;
			public int newSize;
		}

		private void DrawAdvancedArea()
		{
			var d = Data;
			gui.VerticalBlock(GUI.skin.box, () =>
			{
				gui.HorizontalBlock(() =>
				{
					gui.IntField("New size", d.newSize, newSize => d.newSize = newSize);
					gui.MiniButton("c", "Commit", MiniButtonStyle.ModRight, () =>
					{
						if (d.newSize != list.Count)
							list.AdjustSize(d.newSize, RemoveListElementAtIndex, AddToList);
					});
				});
				gui.HorizontalBlock(() =>
				{
					gui.IntField("Remove by index", d.index, newIndex => d.index = newIndex);
					gui.MiniButton("c", "Commit", MiniButtonStyle.ModRight, () => RemoveListElementAtIndex(d.index));
				});
				gui.HorizontalBlock(() =>
				{
					gui.GuessField("Remove by value", d.value, ElementType, newValue => d.value = newValue);
					gui.MiniButton("c", "Commit", MiniButtonStyle.ModRight, () => RemoveListElement(d.value));
				});
				gui.HorizontalBlock(() =>
				{
					bool contains = list.Contains(d.valueToCheck);
					gui.ColorBlock(contains ? GuiHelper.GreenColorDuo.FirstColor : GuiHelper.RedColorDuo.FirstColor, () =>
					{
						gui.GuessField("Contains value?", d.valueToCheck, ElementType, newValue => d.valueToCheck = newValue);
					});
				});
				gui.HorizontalBlock(() =>
				{
					gui.Label("Command buttons");
					gui.MiniButton("Shuffle",
						"Shuffle list (randomize the order of the list's elements",
						null,
						list.ShuffleNonGen);
					gui.MoveDownButton(() => list.Shift(true));
					gui.MoveUpButton(() => list.Shift(false));
					gui.ClearButton("elements", MiniButtonStyle.ModRight, ClearList);
				});
			});
		}

		private void DrawAddingArea()
		{
			Action<Object> add = AddToList;
			var eType = ElementType;

			gui.Space(-3f);
			gui.DragDropArea(
				@label: "+",
				@labelSize: 17,
				@style: GUI.skin.textField,
				@canSetVisualModeToCopy: dragObjects => dragObjects.All(o => eType.IsAssignableFrom(o.GetType())),
				@cursor: MouseCursor.Link,
				@onDrop: add,
				@onMouseUp: () => SelectionWindow.Show<Object>(
					@getValues: () => Object.FindObjectsOfType(eType),
					@getTarget: () => null,
					@setTarget: add,
					@getValueName: value => value.name,
					@label: eType.Name + "s"),
				@preSpace: 10f,
				@postSpace: 5f,
				@height: .5f
			);
		}

		private void DrawField(int numeric, object value, Action<object> setValue, Action moveDown, Action moveUp, Action remove)
		{
			gui.HorizontalBlock(() =>
			{
				gui.NumericLabel(numeric);
				gui.GuessField(value, ElementType, setValue);
				if (advancedMode)
					MoveUpDown(moveDown, moveUp);
				gui.RemoveButton("element", MiniButtonStyle.ModRight, remove);
			});
		}

		private void MoveUpDown(Action moveDown, Action moveUp)
		{
			MakeChange("Moved up/down", () =>
			{
				gui.MoveDownButton(moveDown);
				gui.MoveUpButton(moveUp);
			});
		}

		private void DrawHeader(Action add)
		{
			gui.HorizontalBlock(() =>
			{
				gui.BoldLabel("Elements");
				gui.FlexibleSpace();
				if (advancedCollection)
					gui.CheckButton(advancedMode, value => advancedMode = value, "advanced mode");
				gui.AddButton("element", MiniButtonStyle.ModRight, add);
			});
		}

		private string GetListName()
		{
			var builder = new StringBuilder();
			builder.Append(info.Name);
			builder.Append(" (");
			var type = IListType;
			if (IsArray)
			{
				builder.Append(type.Name);
			}
			else
			{
				builder.Append(type.Name.Remove(type.Name.IndexOf("`")));
				builder.Append("<");
				builder.Append(type.GetGenericArguments()[0].Name);
				builder.Append(">");
			}
			builder.Append(")");
			return builder.ToString();
		}

		private void ApplyList()
		{
			info.SetValue(target, IsArray ?
				(list as ArrayList).ToArray(ElementType) :
				list);
		}

		private IList GetList()
		{
			return IsArray ?
				new ArrayList(info.GetValue(target) as IList) :
				info.GetValue(target) as IList;
		}

		private void RemoveListElement(object value)
		{
			int index = list.IndexOf(value);
			if (index < 0)
				throw new InvalidOperationException("value not found");
			RemoveListElementAtIndex(index);
		}

		private void MakeChange(string changeName, Action change)
		{
			Undo.RecordObject(target, changeName);
			change();
		}

		private void RemoveListElementAtIndex(int atIndex)
		{
			MakeChange("Removed element at index", () => list.RemoveAt(atIndex));
		}

		private void SetListElementAtIndex(int atIndex, object to)
		{
			MakeChange("List element set", () => list[atIndex] = to);
		}

		private void ClearList()
		{
			MakeChange("Cleared", list.Clear);
		}

		private void InsertToList(int atIndex, object value)
		{
			MakeChange("Inserted element", () => list.Insert(atIndex, value));
		}

		private void AddToList(object value)
		{
			InsertToList(list.Count, value);
		}

		private void AddToList()
		{
			AddToList(ElementType.GetDefaultValueEmptyIfString());
		}
	}
}