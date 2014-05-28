using UnityEngine;
using System;

namespace ShowEmAll
{
	[Serializable]
	public class ScaledCurve
	{
		public float scale = 1;
		public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
	}
}