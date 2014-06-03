using UnityEngine;
using System.Collections;

public class PlayerInteractionHandler : MonoBehaviour {

	GameObject target;

	void Interact(){
		IInteractable[] interactables = gameObject.GetInterfaces<IInteractable>();
		print (gameObject.GetInterface<IInteractable>());
		interactables[0].Interact(target);
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Player"){
			InputHandler.OnInteract += Interact;
			target = other.gameObject;
		}
	}
	
	void OnTriggerExit2D(Collider2D other){
		
		if(other.gameObject.tag == "Player"){
			InputHandler.OnInteract -= Interact;
			target = null;
		}
	}
}
