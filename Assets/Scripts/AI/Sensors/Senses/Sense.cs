using ShowEmAll;
using UnityEngine;

public abstract class Sense : BetterBehaviour
{
	protected virtual void Initialize() { }
	protected virtual void UpdateSense() { }

	private void Awake()
	{
		Initialize();
	}

	private void Update()
	{
		UpdateSense();
	}
}