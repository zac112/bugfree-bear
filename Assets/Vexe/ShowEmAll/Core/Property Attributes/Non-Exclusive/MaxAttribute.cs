using System;
using UnityEngine;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field)]
	public class MaxAttribute : ConstrainValueAttribute
	{
		public readonly float fMax;
		public readonly int iMax;

		public MaxAttribute(int max)
		{
			iMax = max;
		}

		public MaxAttribute(float max)
		{
			fMax = max;
		}
	}
}