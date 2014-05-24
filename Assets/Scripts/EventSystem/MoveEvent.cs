using UnityEngine;

public class MoveEvent : GameEvent {

	protected override void BeginEvent ()
	{
		GetComponent<Seeker>().enabled = true;
	}

	protected override void EndEvent ()
	{
		GetComponent<Seeker>().enabled = false;
	}
}
