using UnityEngine;
using System.Collections.Generic;
using Vexe.RuntimeHelpers;

public class Seeker : MonoBehaviour
{
	public Transform targetPos;
	public Mover mover;
	public float minimumTargetMoveDistance = .4f;

	[SerializeField, HideInInspector] private int[] currentPos = new int[2];
	[SerializeField, HideInInspector] private List<Vector2> path = new List<Vector2>();
	[SerializeField, HideInInspector] private Vector3 rememberedTargetPos;
	[SerializeField, HideInInspector] private int index;

	private Transform mTransform;
	private Transform cachedTransform { get { return RTHelper.LazyValue(ref mTransform, () => transform); } }

	void Awake()
	{
		currentPos = new int[] {
						(int)Mathf.Round (cachedTransform.position.x),
						(int)Mathf.Round (cachedTransform.position.y)
				};
	}

	void FixedUpdate()
	{
		if ((rememberedTargetPos - targetPos.position).sqrMagnitude > minimumTargetMoveDistance)
		{
			Nav.Map.FindPath(cachedTransform, targetPos, path);
			rememberedTargetPos = targetPos.position;
			index = 0;
		}

		if (currentPos[0] != (int)Mathf.Round(cachedTransform.position.x) || currentPos[1] != (int)Mathf.Round(cachedTransform.position.y))
		{
			index = Mathf.Min(index + 1, path.Count - 1);
			currentPos = new int[] {
								(int)Mathf.Round (cachedTransform.position.x),
								(int)Mathf.Round (cachedTransform.position.y)
						};
		}

		if (path.Count == 0)
		{
			return;
		}

		if (path.Count <= 1)
		{
			mover.Move(targetPos.position);
			index = 0;
		}
		else
		{
			mover.Move(path[index]);
		}
	}
}