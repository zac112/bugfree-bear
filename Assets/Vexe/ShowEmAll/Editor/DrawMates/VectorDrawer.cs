using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using System;
using Vexe.RuntimeExtensions;
using Object = UnityEngine.Object;
using sp = UnityEditor.SerializedProperty;

namespace ShowEmAll.DrawMates
{
	public abstract class VectorDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private const string RESET = "0";
		private const string NORMALIZE = "1";
		private const string RANDOMIZE = "r";
		protected static Func<float> rand = () => UnityEngine.Random.Range(-100, 100);
		protected string label;
		protected override string key { get { return base.key + label; } }


		public VectorDrawer() { }

		public VectorDrawer(TWrapper gui, Object target)
			: base(gui, target)
		{ }

		public VectorDrawer(string label, TWrapper gui, Object target)
			: base(gui, target)
		{
			Set(label);
		}

		protected void Set(string label)
		{
			this.label = label;
		}

		protected void Draw(Action field, Action buttons)
		{
			Action doFoldout = () =>
			{
				bool isGw = gui is GUIWrapper;
				gui.Space(7f);
				gui.Foldout(foldout, newValue =>
				{
					if (foldout != newValue)
					{
						foldout = newValue;
						HeightHasChanged();
					}
				});
				gui.Space(isGw ? -10f : 0f);
			};
			DoWideTrick(field, doFoldout, buttons);
		}

		protected void DoButtons(Action copy, Action paste, Action randomize, Action normalize, Action reset)
		{
			if (foldout)
			{
				gui.FlexibleSpace();
				var option = new TOption { Height = 13f };
				gui.MiniButton("Copy", "Copy vector value", option, copy);
				gui.MiniButton("Paste", "Paste vector value", option, paste);
				gui.MiniButton(RANDOMIZE, "Randomize values between [-100, 100]", MiniButtonStyle.ModMid, option, randomize);
				gui.MiniButton(NORMALIZE, "Normalize", MiniButtonStyle.ModMid, option, normalize);
				gui.MiniButton(RESET, "Reset", MiniButtonStyle.ModRight, option, reset);
			}
		}

		// Just to play nice with wideMode being false
		protected void DoWideTrick(Action field, Action doFoldout, Action buttons)
		{
			bool isGw = gui is GUIWrapper;
			if (EditorGUIUtility.wideMode)
			{
				gui.HorizontalBlock(() =>
				{
					field();
					doFoldout();
				});

				gui.HorizontalBlock(() =>
				{
					buttons();
					gui.Space(isGw ? 20f : 25f);
				});
			}
			else
			{
				int space = isGw ? 25 : 35;

				// draw the field on its own
				gui.HorizontalBlock(field);

				// and then the buttons and the foldout, on their own - pull them up to allign them
				gui.Space(-space);
				gui.HorizontalBlock(() =>
				{
					gui.FlexibleSpace();
					buttons();
					doFoldout();
				});

				// finally some space so we don't overlap on neighbour controls
				gui.Space(18f);
			}
		}
	}

	public class Vector2Drawer<TWrapper, TOption> : VectorDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private Func<Vector2> getValue;
		private Action<Vector2> setValue;
		private Action field;

		public Vector2Drawer(string label, TWrapper gui, Object target)
			: base(label, gui, target)
		{ }

		protected void DoV2Buttons()
		{
			DoButtons(
				@copy: () => Clipboard.Vector2Value = getValue(),
				@paste: () => setValue(Clipboard.Vector2Value),
				randomize: () => setValue(new Vector2(rand(), rand())),
				normalize: () => setValue(Vector2.one),
				reset: () => setValue(Vector2.zero)
			);
		}

		public void Set(sp spVector2)
		{
			field = () =>
				gui.ApplyAfterChange(() =>
					gui.PropertyField(spVector2, label), spVector2.serializedObject);

			getValue = () => spVector2.vector2Value;

			setValue = newValue =>
			{
				if (!spVector2.vector2Value.ApproxEqual(newValue))
				{
					spVector2.vector2Value = newValue;
				}
			};
		}

		public void Set(Func<Vector2> getValue, Action<Vector2> setValue, Object target)
		{
			this.getValue = getValue;
			this.setValue = setValue;
			this.target = target;

			var current = getValue();
			field = () => gui.Vector2Field(label, current, newValue =>
			{
				if (!current.ApproxEqual(newValue))
				{
					Undo.RecordObject(target, "Changed vector value");
					setValue(newValue);
				}
			});
		}

		public void Draw(Func<Vector2> getValue, Action<Vector2> setValue, Object target)
		{
			Set(getValue, setValue, target);
			Draw();
		}

		public void Draw(sp spVector2)
		{
			Set(spVector2);
			Draw();
		}

		public override void Draw()
		{
			base.Draw(field, DoV2Buttons);
		}
	}

	public class Vector3Drawer<TWrapper, TOption> : VectorDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private Func<Vector3> getValue;
		private Action<Vector3> setValue;
		private Action field;

		public Vector3Drawer(TWrapper gui, Object target)
			: base(gui, target)
		{ }

		public Vector3Drawer(string label, TWrapper gui, Object target)
			: base(label, gui, target)
		{ }

		protected void DoV3Buttons()
		{
			DoButtons(
				copy: () => Clipboard.Vector3Value = getValue(),
				paste: () => setValue(Clipboard.Vector3Value),
				randomize: () => setValue(new Vector3(rand(), rand(), rand())),
				normalize: () => setValue(Vector3.one),
				reset: () => setValue(Vector3.zero)
			);
		}

		public void Set(sp spVector3)
		{
			field = () =>
				gui.ApplyAfterChange(() =>
					gui.PropertyField(spVector3, label), spVector3.serializedObject);

			getValue = () => spVector3.vector3Value;

			setValue = newValue =>
			{
				if (!spVector3.vector3Value.ApproxEqual(newValue))
				{
					spVector3.vector3Value = newValue;
				}
			};
		}

		public void Set(Func<Vector3> getValue, Action<Vector3> setValue)
		{
			this.getValue = getValue;
			this.setValue = setValue;

			var current = getValue();
			field = () => gui.Vector3Field(label, current, newValue =>
			{
				if (!current.ApproxEqual(newValue))
				{
					Undo.RecordObject(target, "Changed vector value");
					setValue(newValue);
				}
			});
		}

		public void Set(Func<Vector3> getValue, Action<Vector3> setValue, Object target)
		{
			this.target = target;
			Set(getValue, setValue);
		}

		public void Draw(Func<Vector3> getValue, Action<Vector3> setValue)
		{
			Set(getValue, setValue);
			Draw();
		}

		public void Draw(string label, Func<Vector3> getValue, Action<Vector3> setValue)
		{
			Set(label);
			Set(getValue, setValue);
			Draw();
		}

		public void Draw(string label, Func<Vector3> getValue, Action<Vector3> setValue, Object target)
		{
			Set(label);
			Draw(getValue, setValue, target);
		}

		public void Draw(Func<Vector3> getValue, Action<Vector3> setValue, Object target)
		{
			Set(getValue, setValue, target);
			Draw();
		}

		public void Draw(string label, sp spVector3)
		{
			Set(label);
			Draw(spVector3);
		}

		public void Draw(sp spVector3)
		{
			Set(spVector3);
			Draw();
		}

		public override void Draw()
		{
			base.Draw(field, DoV3Buttons);
		}
	}
}