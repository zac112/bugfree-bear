using UnityEngine;
using System.Collections.Generic;
using Vexe.RuntimeHelpers;
using Vexe.RuntimeExtensions;
using uFAction;
using ShowEmAll;
using Vexe.RuntimeHelpers.Helpers;

[DefineCategory("Delegates")]
public class Seeker : BetterBehaviour
{
	public Transform target;
	public Mover mover;
	public float minTargetMoveDist = .4f;

	[ShowDelegate, CategoryMember("Delegates")]
	public UnityAction
		onTargetReached = new UnityAction(),
		onUnreachableEndPosition = new UnityAction(),
		onAtTargetLocation = new UnityAction();

	[SerializeField, HideInInspector]
	private int[] currentPos = new int[2];
	[SerializeField, BetterCollection, Readonly]
	protected List<Vector2> path = new List<Vector2>();
	[SerializeField, HideInInspector]
	private Vector3 rememberedTargetPos;
	[SerializeField, HideInInspector]
	private int index;

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
		//has the target location moved enough to warrant searching for a new path?
		if ((rememberedTargetPos - target.position).sqrMagnitude > minTargetMoveDist)
		{
			try
			{
				Nav.Map.FindPath(cachedTransform, target, path);
			}
			catch (UnityException e)
			{
				path.Clear();
				onUnreachableEndPosition.Invoke();
				e.Source = "Seeker";
				//Debug.Log(e.Message);
				return;
			}
			rememberedTargetPos = target.position;
			index = 0;
		}


		if (path.Count == 0)
		{
			onAtTargetLocation.Invoke();
			//Debug.Log("already at target");
			return;
		}

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
			mover.MoveTowards(target.position);
		}
		else
		{
			mover.MoveTowards(path[index]);
		}
	}

	private void OnDrawGizmos()
	{
		if (!path.IsNullOrEmpty())
		{
			var list = new List<Vector2>(path);
			list.Reverse();
			for (int i = 0; i < list.Count - 1; i++)
			{
				GizHelper.DrawLine(list[i], list[i + 1], pathColor);
			}
		}
		if (target != null)
			GizHelper.DrawLine(transform.position, target.position, targetColor);
	}

	[CategoryMember(DBug)]
	public Color targetColor, pathColor;
}