using UnityEngine;
using UnityEditor;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ScaledCurve))]
	public class ScaledCurveDrawer : BetterPropertyDrawer<ScaledCurve>
	{
		private SerializedProperty spScale;
		private SerializedProperty spCurve;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			spScale = property.FindPropertyRelative("scale");
			spCurve = property.FindPropertyRelative("curve");
			base.Init(property, label);
		}

		protected override void Code()
		{
			gui.HorizontalBlock(() =>
			{
				gui.Slider(spScale, fieldInfo.Name, 0f, 1f, null);
				gui.PropertyField(spCurve, string.Empty, new GUIOption { Width = 60f });
			});
		}
	}
}