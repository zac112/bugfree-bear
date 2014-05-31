using UnityEngine;
using System;
using Vexe.RuntimeExtensions;

public abstract class Mover : MonoBehaviour
{
	[SerializeField]
	private float speed = 2f;

	protected Transform cachedTransform;

	void Awake()
	{
		cachedTransform = transform;
	}

	public float Speed
	{
		get { return speed; }
		set { speed = value; }
	}

	public virtual void Move(Vector2 direction)
	{
		cachedTransform.position = Vector2.MoveTowards(cachedTransform.position, direction, Speed / 100);
	}
}