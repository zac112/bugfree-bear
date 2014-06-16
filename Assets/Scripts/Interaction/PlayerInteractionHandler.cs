using ShowEmAll;
using uFAction;
using UnityEngine;
using Vexe.RuntimeExtensions;

public class PlayerInteractionHandler : MonoBehaviour
{
	GameObject target;

	[ShowDelegate]
	public UnityAction action = new UnityAction();

	void Interact()
	{
		action.Invoke();
		/*IInteractable[] interactables = gameObject.GetInterfaces<IInteractable>();
		print(gameObject.GetInterface<IInteractable>());*/
		//interactables[0].Interact(target);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			InputHandler.OnInteract += Interact;
			target = other.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{

		if (other.gameObject.tag == "Player")
		{
			InputHandler.OnInteract -= Interact;
			target = null;
		}
	}
}