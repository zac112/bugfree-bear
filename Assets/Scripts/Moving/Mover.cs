using UnityEngine;
using ShowEmAll;

public abstract class Mover : BetterBehaviour
{
	[SerializeField, HideInInspector]
	private float movementSpeed = 2f;

	private Transform mTransform;

	protected Transform cachedTransform { get { if (mTransform == null) mTransform = transform; return mTransform; } }
	protected float dt { get { return Time.deltaTime; } }

	[ShowProperty]
	public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; } }

	public float SmoothedMovement { get { return MovementSpeed * dt; } }

	public virtual void Move(Vector2 direction)
	{
		cachedTransform.position = Vector2.MoveTowards(cachedTransform.position, direction, MovementSpeed / 100);
	}
}