using ShowEmAll;
using UnityEngine;
using Vexe.RuntimeHelpers;

public class Nav : BetterBehaviour
{
	[SerializeField, HideInInspector]
	private GameObject tiles;

	[SerializeField, HideInInspector]
	private NavMap map;

	[ShowProperty]
	public GameObject Tiles
	{
		get { return tiles; }
		set
		{
			tiles = value;
			RecreateMap();
		}
	}

	public static NavMap Map
	{
		get { return RTHelper.LazyValue(ref instance.map, instance.CreateMap); }
	}

	private static Nav mInstance;
	private static Nav instance
	{
		get { return RTHelper.LazyValue(ref mInstance, FindObjectOfType<Nav>); }
	}

	[ShowMethod]
	public void RecreateMap()
	{
		map = CreateMap();
	}

	private NavMap CreateMap()
	{
		float maxX = float.MinValue;
		float maxY = float.MinValue;
		float minX = float.MaxValue;
		float minY = float.MaxValue;

		Tile[] tileList = Tiles.GetComponentsInChildren<Tile>();
		foreach (Tile t in tileList)
		{
			if (t.transform.position.x > maxX)
				maxX = t.transform.position.x;
			if (t.transform.position.y > maxY)
				maxY = t.transform.position.y;
			if (t.transform.position.x < minX)
				minX = t.transform.position.x;
			if (t.transform.position.y < minY)
				minY = t.transform.position.y;
		}

		return new NavMap(Mathf.RoundToInt(maxX + 1), Mathf.RoundToInt(maxY + 1), minX, minY);
	}
}