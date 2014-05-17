using UnityEngine;

public class GameEvent : MonoBehaviour {

	[SerializeField]
	public GameTime startTime;
	[SerializeField]
	public GameTime endTime;

	void Start(){
		GameTimeManager.ReceiveNotificationAt(startTime, BeginEvent);
		GameTimeManager.ReceiveNotificationAt(endTime, EndEvent);
	}

	void BeginEvent(){
		Debug.Log(this +" Event Begun");
	}

	void EndEvent(){
		Debug.Log(this +" Event over");
	}
}
