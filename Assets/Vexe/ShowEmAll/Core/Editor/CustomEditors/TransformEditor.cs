using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using sp = UnityEditor.SerializedProperty;
using ShowEmAll.DrawMates;

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

		private void OnEnable()
		{
			StuffHelper.InitTransformSPs(serializedObject, out spPos, out spRot, out spScale);
			gui = new GLWrapper();
			transformDrawer = new TransformDrawer<GLWrapper, GLOption>(gui, serializedObject.targetObject)
			{
				spPos = spPos,
				spRot = spRot,
				spScale = spScale
			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			transformDrawer.Draw();
			serializedObject.ApplyModifiedProperties();
		}
	}
}