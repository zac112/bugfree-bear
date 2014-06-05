using UnityEditor;
using EditorGUIFramework;
using System.Reflection;
using System;
using Vexe.RuntimeExtensions;
using Object = UnityEngine.Object;
using Vexe.EditorHelpers;

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

		private bool TryGetPropertyValue(out object value)
		{
			try
			{
				value = property.GetGetMethod().Invoke(target, null);
				return true;
			}
			catch (Exception e)
			{
				value = null;
				return false;
			}
		}

		private void SetPropertyValue(object value)
		{
			try
			{
				property.GetSetMethod().Invoke(target, new object[] { value });
			}
			catch (Exception e)
			{
				gui.HelpBox("Property setter threw an exception: " + e.Message, MessageType.Error);
			}
		}

		public CSPropertyDrawer(TWrapper gui, Object target)
			: base(gui, target)
		{ }

		public CSPropertyDrawer()
		{ }

		public override void Draw()
		{
			object value;
			if (!TryGetPropertyValue(out value))
			{
				gui.HorizontalBlock(() =>
				{
					gui.Label(displayText);
					gui.ColorBlock(GuiHelper.RedColorDuo.FirstColor, () =>
						gui.TextFieldLabel("Property getter threw an exception")
					);
				});
				return;
			}
			gui.GuessField(displayText, value, PropertyType, newValue =>
			{
				if (value != newValue)
				{
					Undo.RecordObject(target, "Property set");
					SetPropertyValue(newValue);
				}
			});
			if (isAutoProp)
				gui.HelpBox("Auto properties won't serialize", MessageType.Warning);
		}
	}
}