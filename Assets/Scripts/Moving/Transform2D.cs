using UnityEngine;
using System;

[Serializable]
public class Transform2D
{
	[SerializeField]
	private Transform transform;

	public Transform2D(Transform transform)
	{
		this.transform = transform;
	}

	public Vector2 position
	{
		get { return (Vector2)transform.position; }
		set { transform.position = value; }
	}

	public Vector2 localPosition
	{
		get { return (Vector2)transform.localPosition; }
		set { transform.localPosition = value; }
	}

	public Quaternion rotation
	{
		get { return transform.rotation; }
		set { transform.rotation = value; }
	}

	public Quaternion localRotation
	{
		get { return transform.localRotation; }
		set { transform.localRotation = value; }
	}

	public float depth
	{
		get { return transform.position.z; }
		set
		{
			Vector3 v = transform.position;
			v.z = value;
			transform.position = v;
		}
	}

	public Vector2 localScale
	{
		get { return transform.localScale; }
		set { transform.localScale = value; }
	}

	public Vector2 up
	{
		get { return transform.up; }
		set { transform.up = value; }
	}

	public Vector2 right
	{
		get { return transform.right; }
		set { transform.right = value; }
	}

	public void Translate(Vector2 v, Space relativeTo)
	{
		transform.Translate(v, relativeTo);
	}
	public void Translate(float x, float y, Space relativeTo)
	{
		Translate(new Vector2(x, y), relativeTo);
	}
	public void Translate(Vector2 v)
	{
		Translate(v, Space.Self);
	}
	public void Translate(float x, float y)
	{
		Translate(x, y, Space.Self);
	}
}