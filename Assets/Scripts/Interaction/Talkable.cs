using UnityEngine;

public class Talkable : MonoBehaviour, IInteractable
{
	public void Interact(GameObject target)
	{
		Debug.Log("interactive");
	}

	public string GiveActionName()
	{
		return "Talk to " + gameObject.name;
	}
}