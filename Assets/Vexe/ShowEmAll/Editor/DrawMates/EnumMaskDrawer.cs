using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using System;
using System.Reflection;
using Vexe.RuntimeExtensions;
using Object = UnityEngine.Object;

namespace ShowEmAll.DrawMates
{
	public class EnumMaskDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private Enum cachedValue;
		private bool hasChanged = true;
		private Type enumType;
		private string[] enumNames;
		private int[] enumValues;
		private FieldInfo info;
		private string text;

		public void Set(FieldInfo info, string text)
		{
			this.text = text;
			this.info = info;
			enumType = Value.GetType();
			enumNames = Enum.GetNames(enumType);
			enumValues = Enum.GetValues(enumType) as int[];
		}

		public void Draw(TWrapper gui, Object target, FieldInfo info, string text)
		{
			Set(gui, target);
			Set(info, text);
			Draw();
		}

		public override void Draw()
		{
			gui.EnumMaskFieldThatWorks(Convert.ToInt32(Value), enumValues, enumNames, text, newMask =>
			{
				var newValue = Enum.ToObject(enumType, newMask) as Enum;
				Value = newValue;
			});
		}

		private Enum Value
		{
			get
			{
				if (hasChanged)
				{
					cachedValue = info.GetValue<Enum>(target);
					hasChanged = false;
				}
				return cachedValue;
			}
			set
			{
				if (cachedValue != value)
				{
					Undo.RecordObject(target, "Mask changed");
					info.SetValue(target, value);
					hasChanged = true;
				}
			}
		}
	}
}