using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using sp = UnityEditor.SerializedProperty;
using ShowEmAll.DrawMates;
using System;

namespace ShowEmAll
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Transform))]
	public class TransformEditor : Editor
	{
		private GLWrapper gui;
		private sp spPos;
		private sp spRot;
		private sp spScale;
		private TransformDrawer<GLWrapper, GLOption> transformDrawer;

		void OnEnable()
		{
			StuffHelper.InitTransformSPs(serializedObject, out spPos, out spRot, out spScale);
			gui = new GLWrapper();
			transformDrawer = new TransformDrawer<GLWrapper, GLOption>(
				gui, serializedObject.targetObject, spPos, spRot, spScale
			);
		}

		//float value;

		public override void OnInspectorGUI()
		{
			//float size = 75;
			//var rect = GUILayoutUtility.GetRect(size, size);
			//var rect = new Rect(0, 0, size, size);
			//value = EditorStuffHelper.Angle(rect, value, Repaint, 1, false);
			serializedObject.Update();
			transformDrawer.Draw();
			serializedObject.ApplyModifiedProperties();
		}
	}
}