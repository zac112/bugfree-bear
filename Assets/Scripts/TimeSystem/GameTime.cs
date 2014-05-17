using UnityEngine;

//Damn Unity for not serializing structs properly; this class should be a struct
[System.Serializable]
public class GameTime {

	[SerializeField]
	private float time;
	public float SinceBeginning{
		get{ return time; }
		set{ time = value; }
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
		this.SinceBeginning = time;
	}

	public static bool operator < (GameTime thisTime, GameTime other){
		return thisTime.SinceBeginning < other.SinceBeginning;
	}

	public static bool operator > (GameTime thisTime, GameTime other){
		return thisTime.SinceBeginning > other.SinceBeginning;
	}

	public static bool operator == (GameTime thisTime, GameTime other){
		return thisTime.SinceBeginning == other.SinceBeginning;
	}

	public static bool operator != (GameTime thisTime, GameTime other){
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
