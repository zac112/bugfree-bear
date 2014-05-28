using UnityEngine;
using UnityEditor;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(CommentAttribute))]
	public class CommentAttributeDrawer : BetterPropertyDrawer<CommentAttribute>
	{
		protected override void Code()
		{
			gui.HelpBox(TypedValue.comment, MessageType.Info);
			gui.PropertyField(property);
		}
	}
}