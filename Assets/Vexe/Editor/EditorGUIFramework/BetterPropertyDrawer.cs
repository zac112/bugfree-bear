using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using Vexe.RuntimeExtensions;

namespace EditorGUIFramework
{
	public abstract class BetterPropertyDrawer<T> : PropertyDrawer where T : class
	{
		protected GUIWrapper gui = new GUIWrapper();
		protected SerializedProperty property;
		protected GUIContent label;
		private bool hasInit;

		public T TypedValue { get { return typeof(T).IsA<PropertyAttribute>() ? GetTypedAttribute<T>() : fieldInfo.GetValue(target) as T; } }
		public SerializedObject serializedObject { get { return property.serializedObject; } }
		public Object target { get { return serializedObject.targetObject; } }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!hasInit || serializedObject != property.serializedObject)
			{
				hasInit = true;
				Init(property, label);
			}

			return gui.Layout(Code);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			this.property = property;
			gui.Draw(position, Code);
		}

		protected virtual void Init(SerializedProperty property, GUIContent label)
		{
			this.property = property;
			this.label = label;
		}

		protected void ApplyAfterChange(Action change)
		{
			gui.ApplyAfterChange(serializedObject, change);
		}

		protected TAttribute GetTypedAttribute<TAttribute>() where TAttribute : class, T
		{
			return attribute as TAttribute;
		}

		protected abstract void Code();
	}
}