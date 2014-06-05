using UnityEngine;

namespace ShowEmAll
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class CommentAttribute : PropertyAttribute
	{
		public readonly string comment;

		public CommentAttribute(string comment)
		{
			this.comment = comment;
		}
	}
}