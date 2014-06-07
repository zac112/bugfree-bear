using System;
using UnityEngine;
using UnityEditor;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	public abstract class ConstrainValueAttributeDrawer : BetterPropertyDrawer<ConstrainValueAttribute>
	{
		protected SerializedPropertyType pType;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			pType = property.propertyType;
			if (pType != SerializedPropertyType.Integer &&
				pType != SerializedPropertyType.Float)
				throw new InvalidOperationException("MinAttribute should only be applied on ints or floats");

			base.Init(property, label);
		}

		protected abstract void ConstrainValue();

		protected override void Code()
		{
			gui.ChangeBlock(() => gui.PropertyField(property, label), ConstrainValue);
		}
	}
}