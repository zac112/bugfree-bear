using System.Collections.Generic;
using UnityEngine;

namespace ShowEmAll.Tests
{
	public class CollectionsTest : BetterBehaviour
	{
		public int[] ints;

		[AwesomeCollection]
		public Transform[] transforms = new Transform[10];

		[Readonly]
		public List<GameObject> readonlyGos;

		[AwesomeCollection]
		public List<GameObject> gos;
	}
}