using UnityEngine;
using UnityEditor;
using Requirement = ShowEmAll.RequiredFromThisAttribute;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(Requirement))]
	public class RequiredFromThisAttributeDrawer : BaseRequirementAttributeDrawer<Requirement>
	{
		protected override Component GetComponent()
		{
			var go = getSource();
			var cType = componentType;
			var c = go.GetComponent(cType);
			if (c == null)
			{
				if (TypedValue.addIfNotExist)
					c = go.AddComponent(cType);
				else
					gui.HelpBox("Couldn't find component in gameObject", MessageType.Warning);
			}
			return c;
		}
	}
}