using uFAction;
using UnityEngine;

public class Touch : Sense
{
	[ShowDelegate("On Collision", CanSetArgsFromEditor = false), SerializeField]
	private CollisionAction onCollision = new CollisionAction();

	[ShowDelegate("On Touch", CanSetArgsFromEditor = false), SerializeField]
	private ColliderAction onTouch = new ColliderAction();

	public ColliderAction OnTouch { get { return onTouch; } }
	public CollisionAction OnCollision { get { return onCollision; } }

	private void OnCollisionEnter(Collision other)
	{
		//Debug.Log("CollisionEnter: " + other.gameObject.name);
		onCollision.Invoke(other);
	}

	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("TriggerEnter: " + other.name);
		onTouch.Invoke(other);
	}
}