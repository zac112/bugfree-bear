using System;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class RequiredFromChildrenAttribute : RequiredAttribute
	{
		/// <summary>
		/// The child path to add to (or create and add) if the component was not found
		/// If you want to specify a nested child, you could input its path, ex:
		/// "Senses/Hearing" - in general "Child/GrandChild/GGChild, etc"
		/// </summary>
		public readonly string childPath;

		public RequiredFromChildrenAttribute(string childPath = "")
		{
			this.childPath = childPath;
		}
	}
}