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
		private string displayText;

		private PropertyInfo mProperty;
		private bool isAutoProp;

		public PropertyInfo property
		{
			get { return mProperty; }
			set
			{
				mProperty = value;
				displayText = value.Name.SplitPascalCase();
				isAutoProp = value.IsAutoProperty();
			}
		}

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
			gui.GuessField(displayText, value, PropertyType, newValue =>
			{
				if (value != newValue)
				{
					Undo.RecordObject(target, "Property set");
					PropertyValue = newValue;
				}
			});
			if (isAutoProp)
				gui.HelpBox("Auto properties won't serialize", MessageType.Warning);
		}
	}
}