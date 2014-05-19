using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Game time manager. Read-Only global access to the in-game time. Allows to modify the rate at which time passes in-game.
/// </summary>
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
	public static float PassedTime{
		get{
#if UNITY_EDITOR
			if(gameTime.SinceBeginning == 0.0 && Time.timeSinceLevelLoad != 0f )
				Debug.LogWarning("No time has passed in-game. There might not be a GameTimeManager in the scene.");
#endif
			return (float)gameTime.SinceBeginning; 
		}
		private set { gameTime.SinceBeginning = value; }
	}

	private static MutableGameTime gameTime = new MutableGameTime(Time.timeSinceLevelLoad);
	/// <summary>
	/// Gets the current in-game time of day in 24h format. E.g. 1.5 is 1:30 am, 13.5 is 1:30 pm
	/// </summary>
	/// <value>The current time.</value>
	public static GameTime CurrentTime{
		get{ return (GameTime)gameTime; }
	}

	private static List<NotificationReceiver> notifications = new List<NotificationReceiver>();
	public static void ReceiveNotificationAt(GameTime time, System.Action action){
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

	//Only the first one will get the real update method, rest will get an empty anonymous one.
	//This way, even if there are more than one time manager, game time moves at the correct rate.
	private static System.Action StaticUpdate = UpdateMethod;
	private System.Action AppropriateUpdate;

	static void UpdateMethod(){
		PassedTime += (Time.deltaTime*TimeScale);

		SendNotifications();
	}

	static void SendNotifications ()
	{
		int lastIndex = notifications.Count-1;
		while(notifications.Count > 0 && CurrentTime > notifications[lastIndex].time){
			//Moving backwards to minimize amount of memory operations on the array
			notifications[lastIndex].action();
			notifications.RemoveAt(lastIndex);
			lastIndex--;
		}
	}

	void Start(){
		AppropriateUpdate = StaticUpdate;
		StaticUpdate = () => {};
	}

	void Update(){
		AppropriateUpdate();
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

	[System.Serializable]
	private class MutableGameTime : GameTime{

		public MutableGameTime(float gameTime) : base(gameTime){

		}

		public new float SinceBeginning{
			get{ return (float)time; }
			set{ time = value; }
		}
	}
}
