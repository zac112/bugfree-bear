using UnityEngine;
using ShowEmAll;
using System;
using Vexe.RuntimeHelpers.Helpers;
using uFAction;

public abstract class Mover : BetterBehaviour
{
	[SerializeField, HideInInspector]
	private float movementSpeed = 2f;

	[SerializeField, Readonly(AssignAtEditTime = true)]
	protected Axis heading;

	[SerializeField, ShowDelegate(CanSetArgsFromEditor = false)]
	protected AxisAction onHeadingChanged = new AxisAction();

	private Transform mTransform;
	protected Transform cachedTransform { get { if (mTransform == null) mTransform = transform; return mTransform; } }
	protected float dt { get { return Time.deltaTime; } }

	[ShowProperty]
	public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; } }
	public float SmoothedMovement { get { return MovementSpeed * dt; } }
	public AxisAction OnHeadingChanged { get { return onHeadingChanged; } }

	protected virtual void Start()
	{
		// Make him face the inital heading
		Move(VectorHelper.AxisToWorldVector(heading));
	}

	public virtual void Move(Vector2 direction)
	{
		MoveTowards(direction);
	}

	public void MoveTowards(Vector2 direction)
	{
		heading = VectorHelper.DetermineHeading(direction - (Vector2)cachedTransform.position);
		dbgHeading = VectorHelper.AxisToWorldVector(heading);
		onHeadingChanged.Invoke(heading);
		cachedTransform.position = Vector2.MoveTowards(cachedTransform.position, direction, MovementSpeed / 100);
	}

	public void MoveBy(Vector2 amount)
	{
		heading = VectorHelper.VectorToAxis(amount);
		onHeadingChanged.Invoke(heading);
		dbgHeading = amount;
		cachedTransform.position += ((Vector3)amount).normalized * SmoothedMovement;
	}

	private void OnDrawGizmos()
	{
		GizHelper.DrawLine(cachedTransform.position, cachedTransform.position + (Vector3)dbgHeading, headingColor);
	}

	[Readonly, CategoryMember(DBug)]
	public Vector2 dbgHeading;
	[CategoryMember(DBug)]
	public Color headingColor;

	[Serializable]
	public class AxisAction : UnityAction<Axis> { }
}