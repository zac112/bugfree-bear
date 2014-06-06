using UnityEngine;
using System.Collections.Generic;

public class Seeker : MonoBehaviour
{
	public Transform targetPos;
	public Mover mover;
	public List<Vector2> path = new List<Vector2>();

	void FixedUpdate()
	{
		Nav.map.FindPath(transform, targetPos, path);

		//Vector2 to;
		if (path.Count == 0)
		{
			//Debug.Log("path not found!");
			return;
		}

		if (path.Count <= 1)
		{
			mover.Move(targetPos.position);
		}
		else
		{
			mover.Move(path[0]);
		}
	}
}