using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {

	void Awake(){
		DontDestroyOnLoad (gameObject);
	}

	void OnLevelWasLoaded(){
		transform.position = GameObject.Find ("Spawn").transform.position;
	}
}
