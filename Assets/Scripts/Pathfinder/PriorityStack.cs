using System;
using System.Collections.Generic;

public class PriorityStack<T> where T : IComparable<T>
{
	private List<T> stack;

	public T First
	{
		get { return stack.GetLast(); }
		set { stack.MaintainDescending(value); }
	}

	public PriorityStack()
	{
		stack = new List<T>();
	}

	public void RemoveFirst()
	{
		stack.RemoveAt(stack.Count - 1);
	}

	public bool IsEmpty()
	{
		return stack.Count == 0;
	}
}