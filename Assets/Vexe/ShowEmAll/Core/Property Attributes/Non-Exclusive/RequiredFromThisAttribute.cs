using UnityEngine;
using System;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class RequiredFromThisAttribute : RequiredAttribute
	{
		/// <summary>
		/// Adds the component if it didn't exist on the gameObject.
		/// </summary>
		public readonly bool addIfNotExist;

		public RequiredFromThisAttribute(bool addIfNotExist = false)
		{
			this.addIfNotExist = addIfNotExist;
		}
	}
}