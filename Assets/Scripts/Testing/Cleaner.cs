using ShowEmAll;
using UnityEngine;
using Vexe.RuntimeExtensions;

/// <summary>
/// I had a very strange occurance where I had null components on my GOs
/// Don't know where they came from or how they came about, so I wrote this cleaner
/// More accurately it was GetComponents<Component> that gave me some null components
/// </summary>
public class Cleaner : BetterBehaviour
{
	public bool destroyAfterClean;
	public bool cleanOnStart;

	private void Start()
	{
		if (cleanOnStart)
			Clean();
	}

	[ShowMethod]
	public void Clean()
	{
		var comps = gameObject.GetAllComponents();
		for (int i = comps.Length - 1; i > -1; i--)
		{
			var c = comps[i];
			if (c == null || c.Equals(null))
				c.Destroy();
		}

		if (destroyAfterClean)
			Destroy();
	}

	[ShowMethod]
	public void Destroy()
	{
		Destroy(this);
	}

	[ShowMethod]
	public void PrintComps()
	{
		gameObject.GetAllComponents().Foreach(Debug.Log);
	}
}