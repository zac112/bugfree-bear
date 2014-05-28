using System;
using UnityEditor;
using UnityEngine;
using Vexe.RuntimeExtensions;
using Object = UnityEngine.Object;
using EditorGUIFramework;
using ShowEmAll.DrawMates;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
	public class EnumMaskAttributeDrawer : BetterPropertyDrawer<EnumMaskAttribute>
	{
		private EnumMaskDrawer<GUIWrapper, GUIOption> enumDrawer;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			base.Init(property, label);
			enumDrawer = new EnumMaskDrawer<GUIWrapper, GUIOption>();
			enumDrawer.Set(gui, target);
			enumDrawer.Set(fieldInfo, TypedValue.displayName.IsNullOrEmpty() ? label.text : TypedValue.displayName);
		}

		protected override void Code()
		{
			enumDrawer.Draw();
		}
	}
}