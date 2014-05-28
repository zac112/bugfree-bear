using UnityEditor;
using EditorGUIFramework;
using System.Reflection;
using System;
using Object = UnityEngine.Object;
using Vexe.RuntimeExtensions;

namespace ShowEmAll.DrawMates
{
	public class CSPropertyDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private PropertyInfo info;

		private Type PropertyType { get { return info.PropertyType; } }

		private object PropertyValue
		{
			get { return info.GetValue(target, null); }
			set { info.GetSetMethod().Invoke(target, new object[] { value }); }
		}

		public CSPropertyDrawer() { }

		public CSPropertyDrawer(TWrapper gui, PropertyInfo info, Object target)
			: base(gui, target)
		{
			Set(info);
		}

		public void Set(PropertyInfo info)
		{
			this.info = info;
		}

		public void Draw(TWrapper gui, PropertyInfo info, Object target)
		{
			Set(gui, target);
			Set(info);
			Draw();
		}

		public override void Draw()
		{
			var value = PropertyValue;
			gui.GuessField(info.Name, value, PropertyType, newValue =>
			{
				if (value != newValue)
				{
					Undo.RecordObject(target, "Property set");
					PropertyValue = newValue;
				}
			});
			if (info.IsAutoProperty())
				gui.HelpBox("Auto properties won't serialize", MessageType.Warning);
		}
	}
}