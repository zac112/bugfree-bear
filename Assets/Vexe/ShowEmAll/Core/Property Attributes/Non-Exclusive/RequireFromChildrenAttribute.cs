using System;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class RequireFromChildrenAttribute : RequireAttribute
	{
		/// <summary>
		/// The child path to add to (or create and add) if the component was not found
		/// If you want to specify a nested child, you could input its path, ex:
		/// "Senses/Hearing" - in general "Child/GrandChild/GGChild, etc"
		/// </summary>
		public readonly string childPath;

		public RequireFromChildrenAttribute(string childPath = "")
		{
			this.childPath = childPath;
		}
	}
}