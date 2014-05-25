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
			if( (gameTime == null || gameTime.SinceBeginning == 0f) && Time.timeSinceLevelLoad != 0f )
				Debug.LogWarning("No time has passed in-game. There might not be a GameTimeManager in the scene.");
#endif
			return (float)gameTime.SinceBeginning; 
		}
		private set { gameTime.SinceBeginning = value; }
	}

	private static MutableGameTime gameTime = new MutableGameTime(0f);
	/// <summary>
	/// Gets the current in-game time of day in 24h format. E.g. 1.5 is 1:30 am, 13.5 is 1:30 pm
	/// </summary>
	/// <value>The current time.</value>
	public static GameTime CurrentTime{
		get{ return (GameTime)gameTime; }
	}

	private static List<NotificationReceiver> notifications = new List<NotificationReceiver>();
	public static void ReceiveNotificationAt(GameTime time, System.Action action){
		notifications.MaintainDescending(new NotificationReceiver(time,action));
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
		if(gameTime == null)
			gameTime = new MutableGameTime(Time.timeSinceLevelLoad);
		AppropriateUpdate = StaticUpdate;
		StaticUpdate = () => {};
	}

	void Update(){
		AppropriateUpdate();
	}

	private class NotificationReceiver : System.IComparable<NotificationReceiver>{

		public GameTime time;
		public System.Action action;

		public NotificationReceiver(GameTime time, System.Action action){
			this.time = time;
			this.action = action;
		}

		public bool BeginsLaterThan(NotificationReceiver other){
			return time > other.time;
		}

		public int CompareTo (NotificationReceiver other)
		{
			if(other.time == this.time)
				return 0;
			return other.time<this.time ? 1 : -1;
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
