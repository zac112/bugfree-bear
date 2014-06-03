using UnityEngine;
using Vexe.RuntimeExtensions;

public class TargetFollower : Mover
{
	public Transform target;

	private void Update()
	{
		var dif = target.position - cachedTransform.position;
		if (dif.sqrMagnitude < .25f)
			return;

		cachedTransform.position += dif * SmoothedMovement;
	}
}