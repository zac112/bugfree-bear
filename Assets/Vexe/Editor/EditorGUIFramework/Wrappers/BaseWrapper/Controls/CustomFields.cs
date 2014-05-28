using UnityEngine;
using Object = UnityEngine.Object;
using Option = EditorGUIFramework.GUIOption;
using UnityEditor;
using System;
using Vexe.EditorHelpers;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		/// <summary>
		/// Creates a draggable label field (a label using a custom style making it resemble an object field)
		/// What if you wanted drop support? then you wouldn't be looking at this method, but one of the others above,
		/// like DraggablePropertyObjectField, and DragObjectField (they support dropping by default)
		/// or manually register your field for drop support via RegisterFieldForDrop, or even use DragAndDropObjectField.
		/// </summary>
		public void DraggableLabelField(string label, string field, Object value, GUIStyle style = null, MouseCursor cursor = MouseCursor.Link)
		{
			Label(field, style ?? GUI.skin.textField);
			GetLastRect(lastRect => GuiHelper.RegisterFieldForDrag(lastRect, value));
		}
		public void DraggableLabelField(string label, Object value)
		{
			DraggableLabelField(label, value == null ? "null" : value.name, value);
		}
		public void DraggableLabelField(Object value)
		{
			DraggableLabelField("", value);
		}
		public void DragDropArea<T>(
			string label, int labelSize, GUIStyle style,
			Predicate<Object[]> canSetVisualModeToCopy, MouseCursor cursor,
			Action<T> onDrop, Action onMouseUp,
			float preSpace, float postSpace, float height) where T : UnityEngine.Object
		{
			HorizontalBlock(() =>
			{
				Space(preSpace);
				IndentedBlock(style, () =>
				{
					Space(height / 4);
					HorizontalBlock(() => FlexibleSandwich(() => Label(label, GuiHelper.CreateLabel(labelSize))));
					Space(height / 4);
				});

				GetLastRect(dropArea =>
				{
					// cache the current event
					Event currentEvent = Event.current;

					// if our mouse isn't contained within that box area, exit out
					if (!dropArea.Contains(currentEvent.mousePosition)) return;

					GuiHelper.AddCursorRect(dropArea, cursor);

					if (onMouseUp != null)
						if (currentEvent.type == EventType.MouseUp)
							onMouseUp();

					if (onDrop != null)
					{
						if (currentEvent.type == EventType.DragUpdated ||
							currentEvent.type == EventType.DragPerform)
						{

							// set the visual mode to copy
							DragAndDrop.visualMode = canSetVisualModeToCopy(DragAndDrop.objectReferences) ?
								DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

							// if we dropped something
							if (currentEvent.type == EventType.DragPerform)
							{

								// register that this drag-drop event has been handled by this control
								DragAndDrop.AcceptDrag();

								// loop over the dropped items
								foreach (var item in DragAndDrop.objectReferences)
								{
									onDrop(item as T);
								}
							}
							// since we've used the DragPerform event, we'll mark it as used
							// (its type will change to EventType.Used)
							// so that other controls ignore it
							Event.current.Use();
						}
					}
				});
				Space(postSpace);
			});
		}
	}
}