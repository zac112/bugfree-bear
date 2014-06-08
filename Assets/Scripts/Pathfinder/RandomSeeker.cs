using ShowEmAll;
using UnityEngine;
using System.Linq;
using Vexe.RuntimeExtensions;

public class RandomSeeker : Seeker
{
	[SerializeField, Required]
	private GameObject tiles;

	private void Start()
	{
		AssignNewTarget();
	}

	private void OnEnable()
	{
		onTargetReached += AssignNewTarget;
		onUnreachableEndPosition += AssignNewTarget;
		onAtTargetLocation += AssignNewTarget;
	}

	[ShowMethod]
	public void AssignNewTarget()
	{
		target = tiles.GetComponentsInChildren<Tile>()
					  .Where(t => t.isWalkable)
					  .RandomElement().transform;
	}

	private void OnDisable()
	{
		onUnreachableEndPosition -= AssignNewTarget;
		onTargetReached -= AssignNewTarget;
		onAtTargetLocation -= AssignNewTarget;
	}
}