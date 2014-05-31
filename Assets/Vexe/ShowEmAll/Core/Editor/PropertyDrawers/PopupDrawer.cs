using UnityEngine;
using UnityEditor;
using System.Linq;
using EditorGUIFramework;
using Vexe.RuntimeExtensions;
using System;
using spType = UnityEditor.SerializedPropertyType;
using Vexe.EditorExtensions;
using Vexe.RuntimeExtensions;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(PopupAttribute))]
	public class PopupDrawer : BetterPropertyDrawer<PopupAttribute>
	{
		private string[] values;
		private int? currentIndex;
		private spType[] supportedTypes = new[] { spType.String, spType.Integer, spType.Float };

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			base.Init(property, label);
			if (!supportedTypes.ContainsValue(property.propertyType))
				throw new Vexe.RuntimeHelpers.Exceptions.InvalidTypeException("Type " + property.propertyType.ToString() + " is not supported. Only strings, ints and floats are.");

			var fromMethod = TypedValue.fromMethod;
			if (string.IsNullOrEmpty(fromMethod))
			{
				values = TypedValue.values;
			}
			else
			{
				var method = property.serializedObject.targetObject.GetMethod(fromMethod, TypeExtensions.ALL_BINDING);

				if (method == null)
					throw new Vexe.RuntimeHelpers.Exceptions.MemberNotFoundException(fromMethod);

				string msg = "Method `" + fromMethod + "` ";
				var type = property.GetActualType().MakeArrayType();
				if (method.ReturnType != type)
					throw new Vexe.RuntimeHelpers.Exceptions.TypeMismatchException(msg + "must have a return type of " + type.Name);

				if (!method.GetParameters().IsEmpty())
					throw new InvalidOperationException(msg + "must have no parameters");

				var retValue = method.Invoke(property.serializedObject.targetObject, null);
				switch (property.propertyType)
				{
					case spType.Integer: values = (retValue as int[]).Select(v => v.ToString()).ToArray();
						break;
					case spType.Float: values = (retValue as float[]).Select(v => v.ToString()).ToArray();
						break;
					default: values = retValue as string[];
						break;
				}
			}
		}

		protected override void Code()
		{
			Action<string> setValue;
			switch (property.propertyType)
			{
				case spType.String:
					setValue = newValue => property.stringValue = newValue;
					break;
				case spType.Integer:
					setValue = newValue => property.intValue = Convert.ToInt32(newValue);
					break;
				default:
					setValue = newValue => property.floatValue = Convert.ToSingle(newValue);
					break;
			}

			string currentValue = property.GetValue().ToString();
			if (!currentIndex.HasValue)
				currentIndex = Mathf.Max(0, values.IndexOf(currentValue));

			gui.Popup(label, currentIndex.Value, values, newIndex =>
			{
				string newValue = values[newIndex];
				if (newValue != currentValue)
				{
					setValue(newValue);
					currentIndex = newIndex;
				}
			});
		}
	}
}