using ShowEmAll;
using UnityEngine;
using Vexe.RuntimeExtensions;

public abstract class RandomMover : Mover
{
	[SerializeField, HideInInspector]
	private float rotationSpeed = 1f;

	private Transform2D mTransform2d;

	[SerializeField, Readonly]
	protected Vector2 targetPos;

	[SerializeField]
	protected Vector2 randX;

	[SerializeField]
	protected Vector2 randY;

	[SerializeField]
	protected Renderer bgRenderer;

	[ShowProperty]
	public float RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }

	public float SmoothedRotation { get { return RotationSpeed * dt; } }

	protected Transform2D transform2d
	{
		get
		{
			if (mTransform2d == null)
				mTransform2d = new Transform2D(transform);
			return mTransform2d;
		}
	}

	[ShowMethod]
	private void SetToBounds()
	{
		var bounds = bgRenderer.bounds;
		randX = new Vector2(bounds.min.x, bounds.max.x);
		randY = new Vector2(bounds.min.y, bounds.max.y);
	}

	private void Awake()
	{
		SetToBounds();
		RandomizeTargetPos();
	}

	private void RandomizeTargetPos()
	{
		targetPos = new Vector2(Random.Range(randX.x, randX.y), Random.Range(randY.x, randY.y));
	}

	protected virtual void Update()
	{
		if (targetPos.SqrDistanceToV2(transform2d.position) <= .25f)
			RandomizeTargetPos();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(cachedTransform.position, targetPos);
		Gizmos.DrawSphere(targetPos, .3f);
	}
}