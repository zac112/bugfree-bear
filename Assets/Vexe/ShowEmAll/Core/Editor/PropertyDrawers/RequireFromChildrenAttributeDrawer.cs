using UnityEngine;
using UnityEditor;
using Vexe.RuntimeExtensions;
using Requirement = ShowEmAll.RequireFromChildrenAttribute;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(Requirement))]
	public class RequireFromChildrenAttributeDrawer : RequireAttributeDrawer<Requirement>
	{
		protected override Component GetComponent()
		{
			var go = getSource();
			var cType = componentType;
			var c = go.GetComponentInChildren(cType);
			if (c == null)
			{
				var path = TypedValue.childPath;
				if (path.IsNullOrEmpty())
				{
					gui.HelpBox("Couldn't find component in children", MessageType.Warning);
				}
				else
				{
					c = go.GetOrAddChildAtPath(path).AddComponent(cType);
				}
			}
			return c;
		}
	}
}