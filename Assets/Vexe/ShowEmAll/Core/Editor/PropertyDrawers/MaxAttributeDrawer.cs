using UnityEngine;
using UnityEditor;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(MaxAttribute))]
	public class MaxAttributeDrawer : ConstrainValueAttributeDrawer
	{
		protected override void ConstrainValue()
		{
			var clamp = GetTypedAttribute<MaxAttribute>();
			if (pType == SerializedPropertyType.Integer)
			{
				property.intValue = Mathf.Min(property.intValue, clamp.iMax);
			}
			else
			{
				property.floatValue = Mathf.Min(property.floatValue, clamp.fMax);
			}
		}
	}
}