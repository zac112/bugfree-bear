using UnityEngine;

namespace ShowEmAll.Tests
{
	public class InlineTest : MonoBehaviour
	{
		[Inline]
		public Transform t;

		[Inline]
		public BoxCollider col;
	}
}