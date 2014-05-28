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
		/// The title of the delegate that will appear in the editor
		/// </summary>
		public readonly string title;

		/// <summary>
		/// If true, the delegate header will always be unfolded (collapsed)
		/// </summary>
		public readonly bool forceExpand;

		/// <summary>
		/// If true, you can set the delegate's arguments (if any) from the editor
		/// </summary>
		public readonly bool canSetArgsFromEditor;

		public ShowDelegate(string title, bool canSetArgsFromEditor, bool forceExpand)
		{
			this.title = title;
			this.canSetArgsFromEditor = canSetArgsFromEditor;
			this.forceExpand = forceExpand;
		}

		public ShowDelegate(string title, bool canSetArgsFromEditor) :
			this(title, canSetArgsFromEditor, false) { }

		public ShowDelegate(string title) :
			this(title, true) { }
	}
}