using UnityEngine;
using System;
using System.Linq;

[AttributeUsage(AttributeTargets.Field)]
public class PopupAttribute : PropertyAttribute
{
	public readonly string fromMethod = string.Empty;
	public readonly string[] values;

	public PopupAttribute(string fromMethod)
	{
		this.fromMethod = fromMethod;
	}

	public PopupAttribute(params string[] strings)
	{
		values = strings;
	}

	public PopupAttribute(params int[] ints)
	{
		values = ints.Select(i => i.ToString()).ToArray();
	}

	public PopupAttribute(params float[] floats)
	{
		values = floats.Select(f => f.ToString()).ToArray();
	}
}