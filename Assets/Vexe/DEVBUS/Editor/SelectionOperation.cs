using UnityEngine;
using Vexe.EditorHelpers;

namespace DEVBUS
{
	public class SelectionOperation : BasicOperation
	{
		public Object[] ToSelect { get; set; }
		public Object[] ToGoBackTo { get; set; }

		public override void Perform()
		{
			EditorHelper.SelectObjects(ToSelect);
			base.Perform();
		}

		public override void Undo()
		{
			EditorHelper.SelectObjects(ToGoBackTo);
			base.Undo();
		}
	}
}