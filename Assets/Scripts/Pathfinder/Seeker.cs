using UnityEngine;
using System.Collections.Generic;
using Vexe.RuntimeHelpers;
using uFAction;
using ShowEmAll;

public class Seeker : BetterBehaviour
{
	public Transform target;
	public Mover mover;
	public float minTargetMoveDist = .4f;

	[ShowDelegate]
	public UnityAction onTargetReached = new UnityAction();

	[SerializeField, HideInInspector] private int[] currentPos = new int[2];
	[SerializeField, HideInInspector] private List<Vector2> path = new List<Vector2>();
	[SerializeField, HideInInspector] private Vector3 rememberedTargetPos;
	[SerializeField, HideInInspector] private int index;

	private Transform mTransform;
	private Transform cachedTransform { get { return RTHelper.LazyValue(ref mTransform, () => transform); } }

	private void Awake()
	{
		currentPos = new int[] {
						(int)Mathf.Round (cachedTransform.position.x),
						(int)Mathf.Round (cachedTransform.position.y)
				};
	}

	private void FixedUpdate()
	{
		if ((rememberedTargetPos - target.position).sqrMagnitude > minTargetMoveDist)
		{
			try{
				Nav.Map.FindPath(cachedTransform, target, path);
			}catch(UnityException e){
				path.Clear();
				//Debug.Log("didn't find path");
				return;
			}
			rememberedTargetPos = target.position;
			index = 0;
		}

		if(path.Count == 0)
			return;

		if (currentPos[0] != (int)Mathf.Round(cachedTransform.position.x) || currentPos[1] != (int)Mathf.Round(cachedTransform.position.y))
		{
			if (index == path.Count - 1)
			{
				onTargetReached.Invoke();
			}

			index = Mathf.Min(index + 1, path.Count - 1);
			currentPos = new int[] {
								(int)Mathf.Round (cachedTransform.position.x),
								(int)Mathf.Round (cachedTransform.position.y)
						};
		}

		if (index == path.Count - 1)
		{
			mover.Move(target.position);
		}
		else
		{
			mover.Move(path[index]);
		}
	}
}