using UnityEngine;
using UnityEditor;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(NumericClampAttribute))]
	public class NumericClampAttributeDrawer : ConstrainValueAttributeDrawer
	{
		protected override void ConstrainValue()
		{
			var clamp = GetTypedAttribute<NumericClampAttribute>();
			if (pType == SerializedPropertyType.Integer)
			{
				property.intValue = Mathf.Clamp(property.intValue, clamp.iMin, clamp.iMax);
			}
			else
			{
				property.floatValue = Mathf.Clamp(property.floatValue, clamp.fMin, clamp.fMax);
			}
		}
	}
}