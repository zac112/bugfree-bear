using System.Collections.Generic;
using Vexe.RuntimeExtensions;
using UnityEngine;

public class CleverRandomMover : RandomMover
{
	public List<Vector2> path = new List<Vector2>();

	protected override void Update()
	{
		//base.Update();

		//Nav.map.FindPath(cachedTransform.position, targetPos, path);

		//if (!path.IsEmpty())
		//{
		//	Vector2 to = path.Count == 1 ? (Vector2)targetPos : path[0];
		//	Move(to);
		//}
	}
}