using UnityEngine;

namespace ShowEmAll
{
	public class StringClampAttribute : RegexAttribute
	{
		public readonly int min;
		public readonly int max;

		public StringClampAttribute(int min, int max)
			: base("^.{" + min.ToString() + "," + max.ToString() + "}$")
		{
			this.min = min;
			this.max = max;
		}
	}
}