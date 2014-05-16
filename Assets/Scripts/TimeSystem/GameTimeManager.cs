using UnityEngine;
using System.Collections.Generic;

public class GameTimeManager : MonoBehaviour{

	/// <summary>
	/// The time scale. How fast time moves.
	/// </summary>
	public static float TimeScale{
		get{ return Time.timeScale; }
		set{ Time.timeScale = value; }
	}

	/// <summary>
	/// The passed game time in minutes.
	/// </summary>
	private static double passedTime = 0;
	public static float PassedTime{
		get{ return (float)passedTime; }
		private set { passedTime = value; }
	}

	/// <summary>
	/// Gets the current in-game time of day in 24h format. E.g. 1.5 is 1:30 am, 13.5 is 1:30 pm
	/// </summary>
	/// <value>The current time.</value>
	public static GameTime CurrentTime{
		get{ return new GameTime(PassedTime); }
	}

	private static List<NotificationReceiver> notifications = new List<NotificationReceiver>();
	public static void ReceiveNotificationAt(GameTime time, System.Action action){
		Debug.Log("received "+time);
		notifications.Add(new NotificationReceiver(time,action));
		OrganizeList();
	}

	/// <summary>
	/// Organizes the list to descending order (latest at index 0).
	/// Assumes the list is otherwise in order and the element to be moved is at the last index.
	/// </summary>
	private static void OrganizeList(){
		for(int i=notifications.Count-1; i>0; i--){
			if(notifications[i].BeginsLaterThan(notifications[i-1]))
				Swap(i,i-1);
			else
				break;
		}
	}

	private static void Swap (int index1, int index2)
	{
		NotificationReceiver temp = notifications[index1];
		notifications[index1] = notifications[index2];
		notifications[index2] = temp;
	}

	void Update(){
		PassedTime += (Time.deltaTime*TimeScale);
		Debug.Log(CurrentTime);
		int lastIndex = notifications.Count-1;
		while(notifications.Count > 0 && CurrentTime > notifications[lastIndex].time){
			notifications[lastIndex].action();
			notifications.RemoveAt(lastIndex);
		}
	}

	private class NotificationReceiver{

		public GameTime time;
		public System.Action action;

		public NotificationReceiver(GameTime time, System.Action action){
			this.time = time;
			this.action = action;
		}

		public bool BeginsLaterThan(NotificationReceiver other){
			return time > other.time;
		}
	}
}
