using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EditorGUIFramework.Helpers;

namespace EditorGUIFramework
{
	public abstract class GUIControlBlock : GUIControl, IPositionableBlock
	{
		protected List<GUIControlEntry> entries = new List<GUIControlEntry>();
		protected Rectangle start;

		protected GUIControl[] controls { get { return entries.Select(e => e.control).ToArray(); } }
		public List<GUIControlEntry> Entries { get { return entries; } }
		public Rectangle Start { get { return start; } set { start = value; } }
		public GUIControl LastControl { get { return controls.Last(); } }
		public override bool State
		{
			get { return base.State; }
			set
			{
				foreach (var c in controls) c.State = value;
				base.State = value;
			}
		}

		public override Color Color
		{
			get { return base.Color; }
			set
			{
				foreach (var c in controls) c.Color = value;
				base.Color = value;
			}
		}

		public override float LabelWidth
		{
			get { return base.LabelWidth; }
			set
			{
				foreach (var c in controls) c.LabelWidth = value;
				base.LabelWidth = value;
			}
		}

		public void AddEntry(GUIControlEntry entry)
		{
			entries.Add(entry);
		}

		public void AddControl(GUIControl control, GUIOption option)
		{
			AddEntry(new GUIControlEntry(control, option));
		}

		protected Rect GetGroupRect()
		{
			var rect = new Rect(Start);
			rect.height = Height.Value;
			if (width.HasValue) rect.width = width.Value;
			return rect;
		}

		protected void DrawGroupBox()
		{
			GUI.Box(GetGroupRect(), "", Style);
		}

		public override void Draw(float x, float y)
		{
			start.x = x;
			start.y = y;
			base.Draw(x, y);
		}

		protected void DrawControl(GUIControl c, float x, float y)
		{
			Blocks.StateBlock(c.State, () =>
				Blocks.ColorBlock(c.Color, () =>
					Blocks.LabelWidthBlock(c.LabelWidth, () =>
						c.Draw(x, y)
					)
				)
			);
		}

		public abstract EmptyControl CreateSpace(float pixels);
	}
}