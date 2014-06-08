using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using Vexe.EditorExtensions;
using Vexe.RuntimeExtensions;
using System;

namespace ShowEmAll.PropertyDrawers
{
	public abstract class BaseRequirementAttributeDrawer<TRequirement> : BetterPropertyDrawer<TRequirement> where TRequirement : RequiredAttribute
	{
		private Func<GameObject> mGetSource;
		protected Func<GameObject> getSource
		{
			get
			{
				if (mGetSource == null)
				{
					var goProp = target.GetType().GetProperty("gameObject");
					mGetSource = new Func<GameObject>(() => goProp.GetGetMethod().Invoke(target, null) as GameObject).Memoize();
				}
				return mGetSource;
			}
		}
		protected Type componentType { get { return fieldInfo.FieldType; } }

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			if (!property.IsReferenceType())
				throw new InvalidOperationException("AutoAssignRef must be applied to UnityEngine.Objects");

			base.Init(property, label);

			if (target.GetType().GetProperty("gameObject") == null)
				throw new InvalidOperationException("property `" + property.name + "` doesn't have a gameObject property");
		}

		protected abstract Component GetComponent();

		protected override void Code()
		{
			if (property.IsNull())
			{
				property.objectReferenceValue = GetComponent();
			}

			gui.PropertyField(property, label);
		}
	}
}