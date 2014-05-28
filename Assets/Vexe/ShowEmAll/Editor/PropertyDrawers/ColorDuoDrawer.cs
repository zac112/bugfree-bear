using UnityEngine;
using UnityEditor;
using sp = UnityEditor.SerializedProperty;
using Vexe.RuntimeHelpers.Classes.GUI;
using EditorGUIFramework;
using Vexe.EditorExtensions;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ColorDuo), true)]
	public class ColorDuoDrawer : BetterPropertyDrawer<ColorDuo>
	{
		protected override void Init(sp property, GUIContent label)
		{
			base.Init(property, label);
		}

		protected override void Code()
		{
			var spColors = property.FindPropertyRelative("colors");
			var spFoldout = property.FindPropertyRelative("foldout");

			gui.Foldout(label.text, spFoldout.boolValue, newValue =>
			{
				if (newValue != spFoldout.boolValue)
				{
					spFoldout.boolValue = newValue;
					gui.HeightHasChanged();
					serializedObject.ApplyModifiedProperties();
				}
			});

			if (spFoldout.boolValue)
			{
				gui.VerticalBlock(GUI.skin.box, () =>
				{
					ApplyAfterChange(() =>
					{
						gui.PropertyField(spColors.GetAt(0), "First");
						gui.PropertyField(spColors.GetAt(1), "Second");
					});
				});
			}
		}
	}
}