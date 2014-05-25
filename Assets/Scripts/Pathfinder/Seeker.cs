using UnityEngine;
using System.Collections.Generic;

public class Seeker : MonoBehaviour {

	public GameObject endPos;
	public Mover mover;
	public List<Vector2> path = new List<Vector2>();

	void FixedUpdate(){
		Nav.map.FindPath(transform, endPos.transform,path);
		Vector2 v2;
		if(path.Count == 0){
			//Debug.Log("path not found!");
			return;
		}

		if(path.Count <= 1){
			v2 = endPos.transform.position;
		}else{
			v2 = (path[0]);
		}
		mover.Move(v2);
	}

}
