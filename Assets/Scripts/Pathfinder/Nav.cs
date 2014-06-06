using UnityEngine;

public class Nav : MonoBehaviour
{
	public static NavMap map;
	public GameObject tiles;

	void Awake()
	{
		int maxX = int.MinValue;
		int maxY = int.MinValue;
		int minX = int.MaxValue;
		int minY = int.MaxValue;

		Tile[] tileList = tiles.GetComponentsInChildren<Tile>();
		foreach (Tile t in tileList)
		{
			if (t.transform.position.x > maxX)
				maxX = (int)Mathf.Round(t.transform.position.x);
			if (t.transform.position.y > maxY)
				maxY = (int)Mathf.Round(t.transform.position.y);
			if (t.transform.position.x < minX)
				minX = (int)Mathf.Round(t.transform.position.x);
			if (t.transform.position.y < minY)
				minY = (int)Mathf.Round(t.transform.position.y);

		}

		map = new NavMap(maxX + 1, maxY + 1, minX, minY);
	}
}
