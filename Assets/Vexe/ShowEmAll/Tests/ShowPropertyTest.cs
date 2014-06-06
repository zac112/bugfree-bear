using UnityEngine;

namespace ShowEmAll.Tests
{
	public class ShowPropertyTest : BetterBehaviour
	{
		[SerializeField, HideInInspector]
		private string strValue;

		[ShowProperty]
		public int AutoProp { get; set; }

		[ShowProperty]
		public string StrValue { get { return strValue; } set { strValue = value; } }
	}
}