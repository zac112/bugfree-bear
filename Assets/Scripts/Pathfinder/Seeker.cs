using UnityEngine;

public class Seeker : MonoBehaviour {

	public GameObject endPos;
	public Mover mover;
	public Vector2[] path;

	void FixedUpdate(){
		path = Nav.map.FindPath(transform, endPos.transform);
		int index = Mathf.Min(path.Length-1,1);
		//Vector2 v2 = (path[index]-(Vector2)transform.position).normalized;
		Vector2 v2 = (path[index]);
		mover.Move(v2);
	}

}
