using UnityEngine;
using UnityEditor;

public class SceneMemory : ScriptableObject
{
	public int oldScene = 0;

	void OnEnable(){
		this.hideFlags = HideFlags.DontSave;
		DontDestroyOnLoad(this);
	}
}

