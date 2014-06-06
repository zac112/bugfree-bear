using UnityEngine;

namespace ShowEmAll.Tests
{
	[DefineCategory("Strings", 2.5f)]
	[DefineCategory("Custom", 1.5f)]
	public class MemberCategoriesTest : BetterBehaviour
	{
		[CategoryMember("Strings")]
		public string s1, s2, s3;

		public Vector2 v2;
		public Vector3 v3;

		[CategoryMember("Custom"), ShowProperty]
		public float AutoProp { get; set; }
		[CategoryMember("Custom"), ShowMethod]
		public void Custom()
		{
			log("Custom");
		}

		[ShowMethod]
		public void Ping()
		{
			log("Ping");
		}

		[CategoryMember(DBug)]
		public bool dbg;
		[CategoryMember(DBug)]
		public Color gizmos;
	}
}