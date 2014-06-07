using System.Collections.Generic;
using UnityEngine;

namespace ShowEmAll.Tests
{
	public class CollectionsTest : BetterBehaviour
	{
		public int[] ints;

		[BetterCollection]
		public Transform[] transforms = new Transform[10];

		[Readonly]
		public List<GameObject> readonlyGos;

		[BetterCollection(advanced = true)]
		public List<GameObject> gos;
	}
}