using UnityEngine;

namespace ShowEmAll.Tests
{
	public class RequireFromChildrenTest : MonoBehaviour
	{
		[RequiredFromChildren]
		public BoxCollider col;

		[RequiredFromChildren("Child")]
		public AudioSource source;

		[RequiredFromChildren("Child/Grandchild")]
		public Rigidbody rb;
	}
}