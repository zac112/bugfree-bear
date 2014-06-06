using UnityEngine;

public class Nav : MonoBehaviour
{
	public static NavMap map;
	public GameObject tiles;

	void Awake()
	{
		float maxX = float.MinValue;
		float maxY = float.MinValue;
		float minX = float.MaxValue;
		float minY = float.MaxValue;

		Tile[] tileList = tiles.GetComponentsInChildren<Tile>();
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

		map = new NavMap((int)Mathf.Round(maxX + 1), (int)Mathf.Round(maxY + 1), minX, minY);
	}
}
