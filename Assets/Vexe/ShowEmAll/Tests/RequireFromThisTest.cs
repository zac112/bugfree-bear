using UnityEngine;

namespace ShowEmAll.Tests
{
	public class RequireFromThisTest : MonoBehaviour
	{
		[RequireFromThis]
		public BoxCollider col;

		[RequireFromThis(true)]
		public AudioSource source;
	}
}