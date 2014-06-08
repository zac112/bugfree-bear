using UnityEngine;
using UnityEditor;

namespace ShowEmAll.PropertyDrawers
{
	public class RequiredAttributeDrawer : BaseRequirementAttributeDrawer<RequiredAttribute>
	{
		protected override Component GetComponent()
		{
			var go = getSource();
			var cType = componentType;
			var comp = go.GetComponent(cType);
			if (comp == null)
			{
				gui.HelpBox("Component is required but yet is not assigned...", MessageType.Warning);
			}
			return comp;
		}
	}
}