using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour{

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
	public static float PassedGameTime{
		get{ return (float)passedTime; }
		private set { passedTime = value; }
	}

	/// <summary>
	/// Gets the current in-game time of day in 24h format. E.g. 1.5 is 1:30 am, 13.5 is 1:30 pm
	/// </summary>
	/// <value>The current time.</value>
	public static float CurrentTime{
		//1440 = 24*60
		get{ return (PassedGameTime%1440*24)/1440f; }
	}

	public static event System.Action OnGameTimeUpdate;

	void Awake(){
		OnGameTimeUpdate += () => {};
		StartCoroutine("GameLoop");
	}

	void Update(){
		PassedTime += (Time.deltaTime*TimeScale);
	}

	private IEnumerator GameLoop(){

		float timesCallable = 0f;

		while(true){
			timesCallable += TimeScale;

			while(timesCallable >= 1f){
				OnGameTimeUpdate();
				timesCallable -= 1f;
			}
			Debug.Log("Game Time: "+PassedGameTime+" Real time: "+Time.time);
			yield return null;
		}
	}
}
