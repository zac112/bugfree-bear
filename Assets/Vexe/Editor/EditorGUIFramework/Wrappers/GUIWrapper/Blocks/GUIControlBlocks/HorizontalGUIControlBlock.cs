using System.Linq;
using EditorGUIFramework.Helpers;
using System;
using Vexe.RuntimeExtensions;

namespace EditorGUIFramework
{
	public class HorizontalGUIControlBlock : GUIControlBlock
	{
		public override float? Height
		{
			set { throw new InvalidOperationException("Can't set horizontal block height"); }
			get
			{
				if (entries.IsEmpty()) return 0;
				height = controls.Select(c => c.Height.Value).Max() + Style.margin.vertical;
				return base.Height;
			}
		}

		public override void Draw()
		{
			if (entries.IsEmpty()) return;

			var margin = Style.margin;

			int totalControls = entries.Count;
			float totalSpace = entries.Take(totalControls - 1).Sum(e => e.control.HorizontalOffset);

			var defWidthControls = controls.Where(c => c.Width.HasValue);
			var nonDefWidthControls = controls.Except(defWidthControls);

			float totalDefinedWidth = defWidthControls.Select(e => e.Width.Value).Sum();

			var flexibles = controls.Where(c => c is GUIFlexibleSpace);
			int nFlexibles = flexibles.Count();
			if (nFlexibles > 0)
			{
				nonDefWidthControls = nonDefWidthControls.Except(flexibles);

				float totalWidthTaken = 0;
				foreach (var c in nonDefWidthControls)
				{
					float w = c.Style.CalcSize(c.Content).x;
					c.Width = w;
					totalWidthTaken += w;
				}
				float leftoverSpace = width.Value - totalSpace - margin.horizontal - totalWidthTaken - totalDefinedWidth;
				float flexibleSpace = leftoverSpace / nFlexibles;
				foreach (var f in flexibles)
					f.Width = flexibleSpace;
			}
			else
			{
				float standardWidth = (width.Value - totalDefinedWidth - totalSpace - margin.horizontal) / (totalControls - defWidthControls.Count());
				foreach (var c in nonDefWidthControls)
				{
					c.Width = standardWidth;
				}
			}

			DrawGroupBox();
			float x = start.x + margin.left;
			float y = start.y + margin.top;
			for (int i = 0; i < controls.Length; i++)
			{
				var c = controls[i];
				DrawControl(c, x, y);
				x += (c.Width.Value);
				x += c.HorizontalOffset;
			}

			base.Draw();
		}

		public override EmptyControl CreateSpace(float pixels)
		{
			return new EmptyControl { Width = pixels, Height = 0 };
		}
	}
}