using UnityEngine;
using UnityEditor;
using Vexe.EditorHelpers;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(DraggableAttribute))]
	public class DraggableDrawer : BetterPropertyDrawer<DraggableAttribute>
	{
		protected override void Init(SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.ObjectReference)
				throw new Vexe.RuntimeHelpers.Exceptions.InvalidTypeException("DraggableAttribute must be used on a UnityEngine.Object field");

			base.Init(property, label);
		}

		protected override void Code()
		{
			ApplyAfterChange(() => gui.DraggablePropertyField(property));
		}
	}
}