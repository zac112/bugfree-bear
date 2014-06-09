using UnityEngine;

public static class TransformExtensions
{
	public static Vector3 ToLocalVector(this Transform from, Axis axis)
	{
		switch (axis)
		{
			case Axis.Up: return from.up;
			case Axis.Down: return -from.up;
			case Axis.Right: return from.right;
			case Axis.Left: return -from.right;
			case Axis.Forward: return from.forward;
			default: return -from.forward;
		}
	}
}