using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace EditorGUIFramework
{
	public abstract class BetterPropertyDrawer<T> : PropertyDrawer where T : class
	{
		protected GUIWrapper gui = new GUIWrapper();
		protected SerializedProperty property;
		protected GUIContent label;
		private bool hasInit;

		public T TypedValue { get { return typeof(PropertyAttribute).IsAssignableFrom(typeof(T)) ? attribute as T : fieldInfo.GetValue(target) as T; } }
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
			gui.Draw(position, Code);
		}

		protected virtual void Init(SerializedProperty property, GUIContent label)
		{
			this.property = property;
			this.label = label;
		}

		protected void ApplyAfterChange(Action change)
		{
			gui.ApplyAfterChange(change, serializedObject);
		}

		protected abstract void Code();
	}
}