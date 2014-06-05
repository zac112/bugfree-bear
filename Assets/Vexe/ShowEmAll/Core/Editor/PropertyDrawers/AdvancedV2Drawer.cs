using UnityEngine;
using UnityEditor;
using spType = UnityEditor.SerializedPropertyType;
using ShowEmAll.DrawMates;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(AdvancedV2Attribute))]
	public class AdvancedV2Drawer : BetterPropertyDrawer<AdvancedV2Drawer>
	{
		private Vector2Drawer<GUIWrapper, GUIOption> vDrawer;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != spType.Vector2)
				throw new Vexe.RuntimeHelpers.Exceptions.InvalidTypeException(property.propertyType + " Type must be of Vector2");

			base.Init(property, label);

			vDrawer = new Vector2Drawer<GUIWrapper, GUIOption>(label.text, gui, target);
		}

		protected override void Code()
		{
			vDrawer.Draw(property);
		}
	}
}