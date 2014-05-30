using UnityEngine;

public class SceneTransition : BaseTrigger, Interaction
{
	public string scene;
	public string sceneGuid;

	public void Interact(GameObject actor)
	{
		Interact();
	}
	public void Interact()
	{
		if (!Mathf.Approximately(Time.timeScale, 0)) // make sure the game is not paused - a GameManager would be more suitable for this
			Application.LoadLevel(scene);
	}
	void Update()
	{
		if (isPlayerInSight && Input.GetKeyDown(Keys.action)) {
			Interact(lastCollidingObject);
		}
	}
}