using System;
using UnityEngine;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field)]
	public class MinAttribute : ConstrainValueAttribute
	{
		public readonly float fMin;
		public readonly int iMin;

		public MinAttribute(int min)
		{
			iMin = min;
		}

		public MinAttribute(float min)
		{
			fMin = min;
		}
	}
}