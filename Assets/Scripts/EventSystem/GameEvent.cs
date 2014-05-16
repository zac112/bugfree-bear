using UnityEngine;

public class GameEvent : MonoBehaviour {

	[SerializeField]
	public GameTime startTime;
	[SerializeField]
	public GameTime endTime;

	void Start(){
		GameTimeManager.ReceiveNotificationAt(startTime, BeginEvent);
	}

	private void BeginEvent(){
		Debug.Log("works");
	}
}
