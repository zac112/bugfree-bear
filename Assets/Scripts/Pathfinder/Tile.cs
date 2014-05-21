using UnityEngine;

public class Tile : MonoBehaviour {

	public bool IsWalkable(){
		return transform.parent.name == "Walkable tiles";
	}
}
