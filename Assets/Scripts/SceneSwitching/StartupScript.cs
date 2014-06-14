using UnityEngine;
using System.Collections;

public class StartupScript : MonoBehaviour {

	public int levelToLoad = 1; 

	void Start () {
		Application.LoadLevel (levelToLoad);
	}

}
