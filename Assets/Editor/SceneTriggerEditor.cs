using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneTrigger))]
public class SceneTriggerEditor : Editor {

	private SceneTrigger _target;
	private GUIContent[] sceneNames;
	private int[] sceneIndexes;

	void OnEnable(){
		_target = (SceneTrigger)target;
		int numberOfScenes = EditorBuildSettings.scenes.Length;
		sceneNames = new GUIContent[numberOfScenes];
		sceneIndexes = new int[numberOfScenes];
		for (int i=0; i<numberOfScenes; i++)
		{
			sceneIndexes[i] = i;
			string scenePath = EditorBuildSettings.scenes[i].path;
			sceneNames[i] = new GUIContent(scenePath.Substring(scenePath.LastIndexOf("/")+1));
		}
	}

	public override void OnInspectorGUI ()
	{
		_target.sceneToLoad = EditorGUILayout.IntPopup(_target.sceneToLoad, sceneNames, sceneIndexes);
	}

}
