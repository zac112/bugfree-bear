using System;
using UnityEngine;

[Serializable]
public struct BoolArray
{
	[SerializeField]
	private bool[] array;

	public BoolArray(int length)
	{
		array = new bool[length];
	}

	public bool this[int index]
	{
		get { return array[index]; }
		set { array[index] = value; }
	}

	public int GetLowerBound(int dim)
	{
		return 0;
	}

	public int GetUpperBound(int dim)
	{
		return array.Length - 1;
	}
}

[Serializable]
public struct Bool2DArray
{
	[SerializeField]
	private BoolArray[] array;

	public Bool2DArray(int l1, int l2)
	{
		array = new BoolArray[l1];
		for (int i = 0; i < l1; i++)
		{
			array[i] = new BoolArray(l2);
		}
	}

	public bool this[int i, int j]
	{
		get { return array[i][j]; }
		set { array[i][j] = value; }
	}

	public BoolArray this[int i]
	{
		get { return array[i]; }
	}

	public int GetLowerBound(int dim)
	{
		return 0;
	}

	public int GetUpperBound(int dim)
	{
		switch (dim)
		{
			case 0: return array.Length - 1;
			case 1: return array[0].GetUpperBound(0);
			default: throw new IndexOutOfRangeException("dim");
		}
	}
}

[Serializable]
public struct Bool3DArray
{
	[SerializeField]
	private Bool2DArray[] array;

	public Bool3DArray(int l1, int l2, int l3)
	{
		array = new Bool2DArray[l1];
		for (int i = 0; i < l1; i++)
		{
			array[i] = new Bool2DArray(l2, l3);
		}
	}

	public bool this[int i, int j, int k]
	{
		get { return array[i][j, k]; }
		set { array[i][j, k] = value; }
	}

	public int GetLowerBound(int dim)
	{
		return 0;
	}

	public int GetUpperBound(int dim)
	{
		switch (dim)
		{
			case 0: return array.Length - 1;
			case 1: return array[0].GetUpperBound(0);
			case 2: return array[0][0].GetUpperBound(0);
			default: throw new IndexOutOfRangeException("dim");
		}
	}
}