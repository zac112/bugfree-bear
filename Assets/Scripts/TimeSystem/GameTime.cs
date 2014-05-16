using UnityEngine;

//Damn Unity for not serializing structs properly
[System.Serializable]
public class GameTime {

	[SerializeField]
	private float time;

	public float SinceBeginning{
		get{ return time; }
		set{ time = value; }
	}

	public int Day{
		get{ return (int)(time/1440f); }
	}

	public int Hours{
		get{ return (int)((time/60f)%24); }
	}

	public int Minutes{
		get{ return (int)( (time%60) ); }
	}

	public override string ToString (){
		return "Day "+Day+ " "+Hours+":"+Minutes;
	}

	public GameTime(float time){
		//1440 = 24*60
		this.time = time;
	}

	public static bool operator < (GameTime thisTime, GameTime other){
		return thisTime.time < other.time;
	}

	public static bool operator > (GameTime thisTime, GameTime other){
		return thisTime.time > other.time;
	}

	public static bool operator == (GameTime thisTime, GameTime other){
		return thisTime.time == other.time;
	}

	public static bool operator != (GameTime thisTime, GameTime other){
		return thisTime.time != other.time;
	}

	public override bool Equals (object obj)
	{
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(GameTime))
			return false;
		GameTime other = (GameTime)obj;
		return time == other.time;
	}
	
	public override int GetHashCode ()
	{
		unchecked {
			return time.GetHashCode ();
		}
	}
	

}
