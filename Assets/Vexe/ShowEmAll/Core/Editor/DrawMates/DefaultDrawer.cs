using UnityEngine;
using UnityEditor;
using EditorGUIFramework;

namespace ShowEmAll.DrawMates
{
	public class DefaultDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		public SerializedProperty Property { get; set; }
		public string DisplayText { get; set; }

		public DefaultDrawer()
		{
		}

		public DefaultDrawer(TWrapper gui)
		{
			this.gui = gui;
		}

		public DefaultDrawer(SerializedProperty sp, string displayText, TWrapper gui)
			: this(gui)
		{
			Property = sp;
			DisplayText = displayText;
		}

		public override void Draw()
		{
			gui.PropertyField(Property, DisplayText);
		}
	}
}