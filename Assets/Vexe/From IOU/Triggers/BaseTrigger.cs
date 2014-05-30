using UnityEngine;
using ShowEmAll;

public abstract class BaseTrigger : BetterBehaviour, IBaseTrigger
{
	[SerializeField, HideInInspector]
	protected bool isPlayerInSight;

	[SerializeField, HideInInspector]
	protected GameObject lastCollidingObject;

	public GameObject LastCollidingObject { get { return lastCollidingObject; } }
	public bool IsPlayerInSight { get { return isPlayerInSight; } }

	public virtual void OnTriggerEnter(Collider other)
	{
		print("Colliding with " + other.name);
		if (other.tag == Tags.player)
		{
			isPlayerInSight = true;
			lastCollidingObject = other.gameObject;
		}
	}
	public virtual void OnTriggerExit(Collider other)
	{
		if (other.tag == Tags.player)
			isPlayerInSight = false;
		if (other == lastCollidingObject)
			lastCollidingObject = null;
	}
	public virtual void DestroyTriggerComponent()
	{
		Destroy(this);
	}
	public virtual void DestroyTriggerGameObject()
	{
		Destroy(gameObject);
	}
}