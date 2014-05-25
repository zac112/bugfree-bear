using UnityEngine;

public class Seeker : MonoBehaviour {

	public GameObject endPos;
	public Mover mover;
	public Vector2[] path;

	void FixedUpdate(){
		path = Nav.map.FindPath(transform, endPos.transform);
		Vector2 v2;
		if(path == null){
			Debug.Log("path not found!");
			return;
		}

		if(path.Length <= 1){
			v2 = endPos.transform.position;
		}else{
			v2 = (path[0]);
		}
		mover.Move(v2);
	}

}
