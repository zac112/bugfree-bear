using System;
using UnityEngine;

namespace uFAction
{
	/// <summary>
	/// The attribute used to decorate SerializedMBDelegates with to integrate them to the editor
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ShowDelegate : PropertyAttribute
	{
		/// <summary>
		/// The title of the delegate that will appear in the editor It's optional.
		/// If you don't pass a title the delegate field name will be used instead
		/// after converting it to Pascal casing and splitting it.
		/// ex: if your delegate field was 'onPlayerSpotted' the title will be 'On Player Spotted'
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// If true, the delegate header will always be unfolded (collapsed)
		/// </summary>
		public bool ForceExpand { get; set; }

		/// <summary>
		/// If true, you can set the delegate's arguments (if any) from the editor
		/// </summary>
		public bool CanSetArgsFromEditor { get; set; }

		public ShowDelegate()
		{
		}

		public ShowDelegate(string title)
		{
			Title = title;
		}
	}
}