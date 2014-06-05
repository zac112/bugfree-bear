using UnityEngine;
using System;

namespace ShowEmAll
{
	/// <summary>
	/// Apply this to your members (fields/properties/methods) to display them in a custom category
	/// The default categories are "Fields", "Properties" and "Methods"
	/// "Fields" will always have the order of 0, "Properties" of 1 and "Methods" of 2
	/// Give your category an order number to determine where it gets drawn
	/// For ex say you create a "Debug" category and you want it after "Methods", you could give it
	/// any order number that's higher than 2
	/// If you wanted it before "Fields", give it a negative number
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
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