using UnityEngine;
using System;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	public class CategoryMemberAttribute : PropertyAttribute
	{
		public readonly string name;
		public readonly float displayOrder;

		public CategoryMemberAttribute(string name, float displayOrder = -1)
		{
			this.name = name;
			this.displayOrder = displayOrder;
		}
	}
}