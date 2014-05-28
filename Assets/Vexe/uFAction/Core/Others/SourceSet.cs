using UnityEngine;

namespace uFAction
{
	public class SourceSet : Tuple<Component, string>
	{
		public SourceSet(Component source, string field)
			: base(source, field) { }
	}
}