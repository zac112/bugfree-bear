using System;
using UnityEditor;
using EditorGUIFramework.Helpers;

namespace EditorGUIFramework
{
	public class GUIPropertyField : GUIControl, IChangableGUIControl
	{
		public SerializedProperty sp;
		public Action OnChange { get; set; }
		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				EditorGUI.PropertyField(Rect, sp, Content),
				OnChange);
			base.Draw();
		}
	}
}