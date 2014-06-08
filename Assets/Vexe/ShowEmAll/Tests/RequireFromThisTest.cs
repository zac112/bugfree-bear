using UnityEngine;

namespace ShowEmAll.Tests
{
	public class RequireFromThisTest : MonoBehaviour
	{
		[RequiredFromThis]
		public BoxCollider col;

		[RequiredFromThis(true)]
		public AudioSource source;
	}
}