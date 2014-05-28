using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class SpaceAttribute : PropertyAttribute
{
	public readonly float pixels;
	public SpaceAttribute(float pixels)
	{
		this.pixels = pixels;
	}
}