using System;
using UnityEngine;
using Vexe.RuntimeExtensions;

public static class VectorHelper
{
	public static Axis DetermineHeading(Vector2 direction)
	{
		Vector2 right = Vector2.right;
		Vector2 left = -right;
		Vector2 up = Vector2.up;
		Vector2 down = -up;

		Func<Vector2, Vector2, float> angle = Vector2.Angle;
		Func<float, float> abs = Mathf.Abs;

		if (abs(angle(right, direction)) <= 45)
			return Axis.Right;
		if (abs(angle(left, direction)) <= 45)
			return Axis.Left;
		if (abs(angle(up, direction)) <= 45)
			return Axis.Up;
		if (abs(angle(down, direction)) <= 45)
			return Axis.Down;

		throw new InvalidOperationException("Couldn't determine heading for direction `" + direction + "`");
	}

	public static Vector3 AxisToWorldVector(Axis axis)
	{
		switch (axis)
		{
			case Axis.Up: return Vector3.up;
			case Axis.Down: return Vector3.down;
			case Axis.Right: return Vector3.right;
			case Axis.Left: return Vector3.left;
			case Axis.Forward: return Vector3.forward;
			default: return Vector3.back;
		}
	}

	public static Axis VectorToAxis(Vector2 vector)
	{
		Axis axis;
		if (vector.ApproxEqual(Vector2.up))
			axis = Axis.Up;
		else if (vector.ApproxEqual(-Vector2.up))
			axis = Axis.Down;
		else if (vector.ApproxEqual(Vector2.right))
			axis = Axis.Right;
		else if (vector.ApproxEqual(-Vector2.right))
			axis = Axis.Left;
		else axis = Axis.Forward;
		//else throw new InvalidOperationException("Couldn't determine axis from vector `" + vector + "`");
		return axis;
	}

}