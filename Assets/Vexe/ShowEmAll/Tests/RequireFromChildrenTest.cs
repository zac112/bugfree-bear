using UnityEngine;

namespace ShowEmAll.Tests
{
	public class RequireFromChildrenTest : MonoBehaviour
	{
		[RequireFromChildren]
		public BoxCollider col;

		[RequireFromChildren("Child")]
		public AudioSource source;

		[RequireFromChildren("Child/Grandchild")]
		public Rigidbody rb;
	}
}