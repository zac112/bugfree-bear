using UnityEngine;
using UnityEditor;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(MinAttribute))]
	public class MinAttributeDrawer : ConstrainValueAttributeDrawer
	{
		protected override void ConstrainValue()
		{
			var clamp = GetTypedAttribute<MinAttribute>();
			if (pType == SerializedPropertyType.Integer)
			{
				property.intValue = Mathf.Max(property.intValue, clamp.iMin);
			}
			else
			{
				property.floatValue = Mathf.Max(property.floatValue, clamp.fMin);
			}
		}
	}
}