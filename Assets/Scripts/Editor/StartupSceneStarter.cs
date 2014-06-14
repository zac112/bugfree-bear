using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class StartupSceneStarter {

	[MenuItem("BugFreeBear/Game starter/Start&Stop Game %&s")]
	public static void Init(){
		EditorApplication.playmodeStateChanged -= CheckForStartButtonPress;

		Debug.Log("init: isplayingorwillchange "+EditorApplication.isPlayingOrWillChangePlaymode);
		Debug.Log("init: isplaying "+EditorApplication.isPlaying);


		if (EditorApplication.isPlaying) {
			LoadPrevious();
		} else if(EditorApplication.isPlayingOrWillChangePlaymode){
			SceneMemory memory = FindSceneMemory();
			RememberStartingScene(memory);
			EditorApplication.SaveScene();
			EditorApplication.OpenScene(EditorBuildSettings.scenes[0].path);
			EditorApplication.isPlaying = true;
			Resources.FindObjectsOfTypeAll<StartupScript>()[0].levelToLoad = memory.oldScene;
		}
	}

	[MenuItem("BugFreeBear/Game starter/Load Previous Scene")]
	public static void LoadPrevious(){
		SceneMemory memory = FindSceneMemory();
		EditorApplication.isPlaying = false;
		EditorApplication.OpenScene(EditorBuildSettings.scenes[memory.oldScene].path);
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

	static StartupSceneStarter(){
		EditorApplication.delayCall += RegisterStateListener;
	}

	static void RegisterStateListener()
	{
		EditorApplication.playmodeStateChanged += Init;
		Debug.Log("started: isplayingorwillchange "+EditorApplication.isPlayingOrWillChangePlaymode);
		Debug.Log("started: isplaying "+EditorApplication.isPlaying);
	}

	private static void CheckForStartButtonPress(){
		Debug.Log("checked button: isplayingorwillchange "+EditorApplication.isPlayingOrWillChangePlaymode);
		Debug.Log("checked button: isplaying "+EditorApplication.isPlaying);
		Init();
	}
}
