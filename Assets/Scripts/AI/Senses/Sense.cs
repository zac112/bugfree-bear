using ShowEmAll;

public abstract class Sense : BetterBehaviour
{
	protected virtual void Initialize() { } // Currently, only Vision override these methods so I might get rid of them...
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