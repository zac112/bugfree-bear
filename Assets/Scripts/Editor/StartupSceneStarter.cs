using UnityEngine;
using UnityEditor;

public class StartupSceneStarter {

	[MenuItem("BugFreeBear/Start&Stop Game %&s")]
	public static void Init(){
		
		SceneMemory memory = FindSceneMemory();

		if (EditorApplication.isPlaying) {
			EditorApplication.isPlaying = false;
			EditorApplication.OpenScene(EditorBuildSettings.scenes[memory.oldScene].path);
		} else {
			RememberStartingScene(memory);

			EditorApplication.OpenScene(EditorBuildSettings.scenes[0].path);
			EditorApplication.isPlaying = true;
			Resources.FindObjectsOfTypeAll<StartupScript>()[0].levelToLoad = memory.oldScene;
		}
	}

	static SceneMemory FindSceneMemory()
	{
		SceneMemory[] temp = Resources.FindObjectsOfTypeAll<SceneMemory> ();
		if (temp.Length == 0) {
			return ScriptableObject.CreateInstance<SceneMemory> ();
		} else {
			return temp [0];
		}
	}

	static void RememberStartingScene(SceneMemory memory)
	{
		
		for(int i=0; i< EditorBuildSettings.scenes.Length; i++){
			if(EditorApplication.currentScene.Equals(EditorBuildSettings.scenes[i].path)){
				memory.oldScene = i;
				break;
			}
		}
	}
}
