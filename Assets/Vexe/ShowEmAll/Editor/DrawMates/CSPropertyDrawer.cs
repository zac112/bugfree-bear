using UnityEditor;
using EditorGUIFramework;
using System.Reflection;
using System;
using Vexe.RuntimeExtensions;
using Object = UnityEngine.Object;

namespace ShowEmAll.DrawMates
{
	public class CSPropertyDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		public PropertyInfo property { get; set; }

		private Type PropertyType { get { return property.PropertyType; } }

		private object PropertyValue
		{
			get { return property.GetValue(target, null); }
			set { property.GetSetMethod().Invoke(target, new object[] { value }); }
		}

		public CSPropertyDrawer(TWrapper gui, Object target)
			: base(gui, target)
		{ }

		public CSPropertyDrawer()
		{ }

		public override void Draw()
		{
			var value = PropertyValue;
			gui.GuessField(property.Name, value, PropertyType, newValue =>
			{
				if (value != newValue)
				{
					Undo.RecordObject(target, "Property set");
					PropertyValue = newValue;
				}
			});
			if (property.IsAutoProperty())
				gui.HelpBox("Auto properties won't serialize", MessageType.Warning);
		}
	}
}