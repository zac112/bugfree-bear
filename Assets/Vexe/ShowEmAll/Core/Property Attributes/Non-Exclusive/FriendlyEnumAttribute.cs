using UnityEngine;
using System;

namespace ShowEmAll
{
	[AttributeUsage(AttributeTargets.Field)]
	public class FriendlyEnumAttribute : PropertyAttribute
	{
	}
}