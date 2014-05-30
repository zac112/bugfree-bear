using UnityEngine;
using Vexe.RuntimeExtensions;

public class FSMTrigger : FSM, IBaseTrigger, Interaction // duh... if only multiple inheritance is allowed
{
	[SerializeField, HideInInspector]
	protected bool isPlayerInSight;

	[SerializeField, HideInInspector]
	protected GameObject lastCollidingObject;

	public GameObject LastCollidingObject { get { return lastCollidingObject; } }
	public bool IsPlayerInSight { get { return isPlayerInSight; } }

	public void OnTriggerEnter(Collider other)
	{
		print("Colliding with " + other.name);
		if (other.tag == Tags.player)
		{
			isPlayerInSight = true;
			lastCollidingObject = other.gameObject;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.tag == Tags.player)
			isPlayerInSight = false;
		if (other == lastCollidingObject)
			lastCollidingObject = null;
	}

	public void Interact(GameObject actor)
	{
		var current = CurrentState;
		if (current == null) return;
		var interaction = current.gameObject.GetInterface<Interaction>();
		if (interaction != null)
			interaction.Interact(actor);
	}

	void Update()
	{
		if (isPlayerInSight && Input.GetKeyDown(Keys.action))
		{
			Interact(lastCollidingObject);
		}
	}
}