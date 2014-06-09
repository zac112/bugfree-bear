using System;
using UnityEngine;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ReadonlyAttribute : PropertyAttribute
	{
		public bool AssignAtEditTime { get; set; }
	}
}