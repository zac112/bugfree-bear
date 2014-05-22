using UnityEngine;

public class Tile : MonoBehaviour {

	void Start(){
		Nav.map.Register(transform, IsWalkable());
	}

	public bool IsWalkable(){
		return transform.parent.name == "Walkable tiles";
	}
}
