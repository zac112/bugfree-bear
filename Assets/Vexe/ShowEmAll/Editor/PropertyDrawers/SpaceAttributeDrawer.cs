using UnityEngine;
using UnityEditor;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(SpaceAttribute))]
	public class SpaceAttributeDrawer : BetterPropertyDrawer<SpaceAttribute>
	{
		protected override void Code()
		{
			gui.Space(TypedValue.pixels);
			gui.PropertyField(property);
		}
	}
}