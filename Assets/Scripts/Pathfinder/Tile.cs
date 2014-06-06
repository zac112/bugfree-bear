using UnityEngine;

public class Tile : MonoBehaviour
{
	public bool isWalkable;

	private void Start()
	{
		Nav.map.Register(transform, isWalkable);
	}
}