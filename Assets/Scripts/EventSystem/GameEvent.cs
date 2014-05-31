using UnityEngine;

public abstract class GameEvent : MonoBehaviour
{
	[SerializeField]
	public GameTime startTime;

	[SerializeField]
	public GameTime endTime;

	protected void Start()
	{
		GameTimeManager.ReceiveNotificationAt(startTime, BeginEvent);
		GameTimeManager.ReceiveNotificationAt(endTime, EndEvent);
	}

	protected abstract void BeginEvent();
	protected abstract void EndEvent();
}