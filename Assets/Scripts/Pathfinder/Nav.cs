using UnityEngine;

public class Nav : MonoBehaviour {

	public static NavMap map;
	public GameObject tiles;

	void Awake(){
		int maxX = 0; 
		int maxY = 0;
		int minX = int.MaxValue;
		int minY = int.MaxValue;

		Tile[] tileList = tiles.GetComponentsInChildren<Tile>();
		foreach(Tile t in tileList){
			if(t.transform.position.x > maxX)
				maxX = (int)t.transform.position.x;
			if(t.transform.position.y > maxY)
				maxY = (int)t.transform.position.y;
			if(t.transform.position.x < minX)
				minX = (int)t.transform.position.x;
			if(t.transform.position.y < minY)
				minY = (int)t.transform.position.y;

		}

		map = new NavMap(maxX+1,maxY+1, minX, minY);
	}
}
