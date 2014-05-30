using UnityEngine;

public interface IBaseTrigger
{
	bool IsPlayerInSight { get; }
	GameObject LastCollidingObject { get; }
	void OnTriggerEnter(Collider other);
	void OnTriggerExit(Collider other);
}