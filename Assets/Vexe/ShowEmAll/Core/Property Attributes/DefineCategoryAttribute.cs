using UnityEngine;
using System;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DefineCategoryAttribute : Attribute
	{
		public readonly string name;
		public readonly float displayOrder;

		public DefineCategoryAttribute(string name, float displayOrder = -1)
		{
			this.name = name;
			this.displayOrder = displayOrder;
		}
	}
}