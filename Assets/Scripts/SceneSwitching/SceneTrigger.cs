using UnityEngine;
using System.Collections;

public class SceneTrigger : MonoBehaviour {

	public int sceneToLoad = 0;

	void OnTriggerEnter2D(Collider2D other){
		Application.LoadLevel (sceneToLoad);
		Nav.instance.RecreateMap ();
	}
}
