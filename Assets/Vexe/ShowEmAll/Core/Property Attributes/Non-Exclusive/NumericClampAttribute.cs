using UnityEngine;
using System;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field)]
	public class NumericClampAttribute : ConstrainValueAttribute
	{
		public readonly int iMin;
		public readonly int iMax;
		public readonly float fMax;
		public readonly float fMin;

		public NumericClampAttribute(int min, int max)
		{
			iMin = min;
			iMax = max;
		}

		public NumericClampAttribute(float min, float max)
		{
			fMin = min;
			fMax = max;
		}
	}
}