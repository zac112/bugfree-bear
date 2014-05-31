using UnityEngine;
using System.Collections.Generic;

public class Seeker : MonoBehaviour
{
	public GameObject endPos;
	public Mover mover;
	public List<Vector2> path = new List<Vector2>();

	void FixedUpdate()
	{
		Nav.map.FindPath(transform, endPos.transform, path);

		Vector2 to;
		if (path.Count == 0)
		{
			//Debug.Log("path not found!");
			return;
		}

		if (path.Count <= 1)
		{
			to = endPos.transform.position;
		}
		else
		{
			to = (path[0]);
		}
		mover.Move(to);
	}
}