using UnityEngine;
using UnityEditor;
using Vexe.RuntimeExtensions;
using System.Text.RegularExpressions;
using System;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(RegexAttribute), true)]
	public class RegexDrawer : BetterPropertyDrawer<RegexAttribute>
	{
		private string mostRecentValidValue;
		private bool shouldShowHelp;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			base.Init(property, label);
			if (property.propertyType != SerializedPropertyType.String)
				throw new Vexe.RuntimeHelpers.Exceptions.InvalidTypeException("Field must be a string");

			Validate();
		}

		protected override void Code()
		{
			gui.ChangeBlock(() => gui.PropertyField(property), Validate);
			if (shouldShowHelp)
				gui.HelpBox(TypedValue.helpMessage, MessageType.Error);
		}

		private void Validate()
		{
			string oldValue = property.stringValue;

			if (!Regex.IsMatch(property.stringValue, TypedValue.pattern))
			{
				if (!TypedValue.helpMessage.IsNullOrEmpty())
				{
					ShowHelp(true);
				}
				else property.stringValue = oldValue;
			}
			else
			{
				serializedObject.ApplyModifiedProperties();
				ShowHelp(false);
			}
		}

		private void ShowHelp(bool show)
		{
			if (shouldShowHelp != show)
			{
				shouldShowHelp = show;
				gui.HeightHasChanged();
			}
		}
	}
}