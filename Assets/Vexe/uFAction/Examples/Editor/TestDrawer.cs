using System;
using System.Linq;
using EditorGUIFramework;
using UnityEditor;
using UnityEngine;

#pragma warning disable 0649

namespace Assets.Vexe.uFAction.Scripts.Examples.Editor
{
	[CustomPropertyDrawer(typeof(DrawMeInACustomWayPlease))]
	public class TestDrawer : BetterPropertyDrawer<DrawMeInACustomWayPlease>
	{
		private string[] options = { "Option1", "Option2", "Option3" };
		private int selectionIndex;
		private SerializedProperty someProperty;
		private Bounds boundsValue;
		private Rect rectValue;
		private Color colorValue1;
		private Color colorValue2;
		private int intValue;
		private float floatValue;
		private bool toggle;

		protected override void Code()
		{
			gui.BoundsField(boundsValue, newBounds => boundsValue = newBounds);
			gui.RectField(rectValue, newRect => rectValue = newRect);

			gui.HorizontalBlock(() =>
			{
				gui.ColorField(colorValue1, newColor => colorValue1 = newColor);
				gui.ColorField(colorValue2, newColor => colorValue2 = newColor);
			});

			gui.HorizontalBlock(() =>
			{
				gui.FloatField("FloatValue", floatValue, newValue => floatValue = newValue);
				gui.IntField("IntValue", intValue, newValue => intValue = newValue);
			});

			gui.ChangeBlock(
				() =>
				{
					gui.PropertyField(property.FindPropertyRelative("name"));
				},
				() => Debug.Log("Something changed"));

			gui.HorizontalBlock(() =>
			{
				gui.EnabledBlock(toggle, () => gui.Button("BUTTON"));
				gui.FlexibleSpace();
				gui.AddButton("something", () => Debug.Log("Adding stuff..."));
				gui.ClearButton("everything", () => Debug.Log("Clearing, you sure about this?"));
				gui.RemoveButton("Stuff", MiniButtonStyle.Right, () => Debug.Log("Removing stuff..."));
			});

			gui.Button("toggle", () =>
			{
				toggle = !toggle;
				Debug.Log("Toggled to: " + toggle);
			});

			gui.EnabledBlock(toggle, () =>
				gui.HorizontalBlock(() =>
				{
					gui.Button(toggle ? "on" : "off", () => Debug.Log("I'm on"));
					gui.Button(toggle ? "On" : "Off", () => Debug.Log("I'm On"));
					gui.Button(toggle ? "ON" : "OFF", () => Debug.Log("I'm ON"));
				}));

			gui.IndentedBlock(GUI.skin.button, 1, () =>
			{
				var spFoldout = property.FindPropertyRelative("toggle");
				//gui.Foldout(spFoldout, "Toggle");
				if (spFoldout.boolValue)
				{
					gui.Splitter();
					gui.HorizontalBlock(() =>
					{
						gui.Label(">");
						Rect foldRect = new Rect();
						gui.GetLastRect(lastRect => foldRect = lastRect);
						gui.Label("Turn " + (toggle ? "off" : "on"));
						gui.GetLastRect(lastRect =>
						{
							if (GUI.Button(CombineRects(foldRect, lastRect), GUIContent.none, GUIStyle.none))
							{
								toggle = !toggle;
							}
						});
						gui.FlexibleSpace();
						gui.EnabledBlock(toggle, () =>
							gui.ColorBlock(toggle ? Color.green : Color.red, () =>
								gui.Button(toggle ? "on" : "off", () => Debug.Log("I'm on"))));
					});

					gui.NumericTextFieldLabel(1, "this is cool! hueheu");
					gui.PropertyField(property.FindPropertyRelative("name"));
					gui.Popup("Method", selectionIndex, options, newIndex => selectionIndex = newIndex);

					gui.DragDropArea<GameObject>(
						label: "Drag-drop GameObject",
						labelSize: 13,
						style: GUI.skin.textArea,
						canSetVisualModeToCopy: dragObjects => dragObjects.All(o => o is GameObject),
						cursor: MouseCursor.Link,
						onDrop: go => Debug.Log(go.name),
						onMouseUp: () => Debug.Log("Click"),
						preSpace: 20f,
						postSpace: 0f,
						height: 80
					);
				}
			});
		}

		public static Rect CombineRects(Rect a, Rect b)
		{
			return new Rect(Math.Min(a.x, b.x), Math.Min(a.y, b.y), a.width + b.width, Math.Max(a.height, b.height));
		}
	}
}