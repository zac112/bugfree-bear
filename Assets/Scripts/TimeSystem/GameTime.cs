using UnityEngine;

/// <summary>
/// Game time. An Immutable class representing game time.
/// </summary>
[System.Serializable]
public class GameTime {

	[SerializeField]
	protected double time;
	public float SinceBeginning{
		get{ return (float)time; }
	}

	public int Day{
		get{ return (int)(SinceBeginning/1440f); }
	}

	public int Hours{
		get{ return (int)((SinceBeginning/60f)%24); }
	}

	public int Minutes{
		get{ return (int)( (SinceBeginning%60) ); }
	}

	public override string ToString (){
		return "Day "+Day+ " "+Hours+":"+Minutes;
	}

	public GameTime(float time){
		this.time = time;
	}

	public static bool operator < (GameTime thisTime, GameTime other){
		return thisTime.SinceBeginning < other.SinceBeginning;
	}

	public static bool operator > (GameTime thisTime, GameTime other){
		return thisTime.SinceBeginning > other.SinceBeginning;
	}

	public static bool operator == (GameTime thisTime, GameTime other){
		if(ReferenceEquals(thisTime,null) || ReferenceEquals(other,null)) return false;
		return thisTime.SinceBeginning == other.SinceBeginning;
	}

	public static bool operator != (GameTime thisTime, GameTime other){
		if(ReferenceEquals(thisTime,null) || ReferenceEquals(other,null)) return true;
		return thisTime.SinceBeginning != other.SinceBeginning;
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
		return SinceBeginning == other.SinceBeginning;
	}
	
	public override int GetHashCode ()
	{
		unchecked {
			return SinceBeginning.GetHashCode ();
		}
	}
	

}
