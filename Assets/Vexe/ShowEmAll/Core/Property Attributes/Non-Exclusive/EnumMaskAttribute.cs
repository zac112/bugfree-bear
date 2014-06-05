using UnityEngine;

namespace ShowEmAll
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class EnumMaskAttribute : PropertyAttribute
	{
		public readonly string displayName;
		public EnumMaskAttribute() { }
		public EnumMaskAttribute(string displayName)
		{
			this.displayName = displayName;
		}
	}
}