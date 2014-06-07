using UnityEngine;
using System.Collections.Generic;

public class Seeker : MonoBehaviour
{
	public Transform targetPos;
	private int[] currentPos = new int[2];
	public Mover mover;
	private List<Vector2> path = new List<Vector2> ();
	private Vector3 rememberedTargetPos;
	private List<Vector2> rememberedPath;
	private int index;

	public float minimumTargetMoveDistance = .4f;

	void Awake ()
	{
		currentPos = new int[] {
						(int)Mathf.Round (transform.position.x),
						(int)Mathf.Round (transform.position.y)
				};
	}

	void FixedUpdate ()
	{
		if ((rememberedTargetPos - targetPos.position).sqrMagnitude > minimumTargetMoveDistance) {
			Nav.map.FindPath (transform, targetPos, path);
			rememberedTargetPos = targetPos.position;
			index = 0;
		}

		if (currentPos [0] != (int)Mathf.Round (transform.position.x) || currentPos [1] != (int)Mathf.Round (transform.position.y)) {
			index = Mathf.Min (index + 1, path.Count - 1);
			currentPos = new int[] {
								(int)Mathf.Round (transform.position.x),
								(int)Mathf.Round (transform.position.y)
						};
		}

		if (path.Count == 0) {
			return;
		}

		if (path.Count <= 1) {
			mover.Move (targetPos.position);
			index = 0;
		} else {
			mover.Move (path [index]);

		}
	}
}