using UnityEngine;
using UnityEditor;
using spType = UnityEditor.SerializedPropertyType;
using ShowEmAll.DrawMates;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(AdvancedV3))]
	public class AdvancedV3Drawer : BetterPropertyDrawer<AdvancedV3Drawer>
	{
		private Vector3Drawer<GUIWrapper, GUIOption> vDrawer;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != spType.Vector3)
				throw new Vexe.RuntimeHelpers.Exceptions.InvalidTypeException(property.propertyType + " Type must be of Vector3");

			base.Init(property, label);

			vDrawer = new Vector3Drawer<GUIWrapper, GUIOption>
			{
				label = label.text,
				gui = gui,
				target = target
			};
		}

		protected override void Code()
		{
			vDrawer.Draw(property);
		}
	}
}