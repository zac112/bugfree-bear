using UnityEngine;

public class Seeker : MonoBehaviour {

	public GameObject endPos;

	void Update(){
		Debug.Log(transform.position);
		Vector2[] path = Nav.map.FindPath(transform, endPos.transform);

		Debug.Log("The path found was:");
		foreach(Vector2 v in path){
			Debug.Log(v);
		}
	}
}
