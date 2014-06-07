using System;
using UnityEngine;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field)]
	public class BetterCollectionAttribute : Attribute
	{
		public bool advanced { get; set; }
	}
}