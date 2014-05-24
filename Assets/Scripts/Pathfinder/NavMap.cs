//#define DEBUGLOGS

using UnityEngine;
using System.Collections.Generic;

public class NavMap {

	private bool[,,] navMap;
	private int[] offset;

	public NavMap(int width, int height, int offsetX, int offsetY){
		offset = new int[]{offsetX,offsetY};
		navMap = new bool[width-offset[0],height-offset[1],1];
	}

	public void Register(Transform position, bool walkable){
		Coordinate c = new Coordinate(position,offset);
		Register(c.x,c.y,walkable);
	}

	public void Register(int x, int y, bool walkable){
#if UNITY_EDITOR && DEBUGLOGS
		if(!IsValidCoordinate(x,y)){
			Debug.LogError("Attempting to register coordinate out of NavMap bounds ("+x+","+y+")! Do you have a Tile with negative coordinates?");
			return;
		}
#endif
		navMap[x,y,0] = walkable;
	}

	private bool IsValidCoordinate(int x, int y){
		return x>=navMap.GetLowerBound(0) && y>=navMap.GetLowerBound(1) && x<=navMap.GetUpperBound(0) && y<=navMap.GetUpperBound(1);
	}

	private bool IsWalkable(Coordinate coordinate){
		return IsWalkable(coordinate.x, coordinate.y);
	}

	private bool IsWalkable(int x, int y){
		return navMap[x,y,0];
	}

	private List<Coordinate> GetAvailableNeighbors(Coordinate position, Dictionary<Coordinate,byte> closedSet, List<Coordinate> openSet){
		List<Coordinate> result = GetWalkableNeighbors(position);
		for(int i=0; i<result.Count; i++){
			if(closedSet.ContainsKey(result[i]) || openSet.Contains(result[i])){
				result.RemoveAt(i);
			}
		}
		return result;
	}

	private List<Coordinate> GetWalkableNeighbors(Coordinate position){
		List<Coordinate> result = new List<Coordinate>(8);
		/*for(int x=position.x-1; x<=position.x+1; x++){
			for(int y=position.y-1; y<=position.y+1; y++){
				if(IsValidCoordinate(x,y) && IsWalkable(x,y) && !position.Equals(x,y)){
					result.Add(new Coordinate(x,y));
				}
			}
		}*/
		for(int x=position.x-1; x<=position.x+1; x++){
			for(int y=position.y-1; y<=position.y+1; y++){
					if(IsValidCoordinate(x,y) && !position.IsDiagonalto(x, y) && IsWalkable(x,y) && !position.Equals(x,y)){
						result.Add(new Coordinate(x,y));
				}
			}
		}
		return result;
	}

	public Vector2[] FindPath(Transform startPosition, Transform endPosition){

		Coordinate start = new Coordinate(startPosition, offset);
		Coordinate end = new Coordinate(endPosition, offset);

		if(start == end){ //already there..
			return new Vector2[]{end.GetAsMapPosition(offset)};
		}

#if UNITY_EDITOR && DEBUGLOGS
		if(!IsWalkable(start)){
			Debug.LogError("Invalid start position("+startPosition+")! Unable to find route.");
			return null;
		}
		if(!IsWalkable(end)){
			Debug.LogError("Invalid end position("+endPosition+")! Unable to find route.");
			return null;
		}
#endif 
		List<Coordinate> path = new List<Coordinate>();
		List<Coordinate> openSet = new List<Coordinate>();
		Dictionary<Coordinate,byte> closedSet = new Dictionary<Coordinate,byte>();

		openSet.Add(start.CalculateDistanceTo(end));
		path.Add(start);
		closedSet.Add(start,0);

		int counter = 0;
		while(openSet.Count > 0){
			counter++;
			if(counter>1000){
				Debug.Log("infinite loop?");
				break;
			}
			Coordinate closest = Coordinate.Zero;
			List<Coordinate> unvisitedNeighbors = GetAvailableNeighbors(path.GetLast(),closedSet, openSet);
			for(int i=0; i<unvisitedNeighbors.Count; i++){
				unvisitedNeighbors[i] = unvisitedNeighbors[i].CalculateDistanceTo(end);
				openSet.MaintainAscending(unvisitedNeighbors[i]);
				if(end.Equals(unvisitedNeighbors[i])){
					#if UNITY_EDITOR
					foreach(Coordinate v in path){
						Debug.Log(v);
					}
					Debug.Log("----------------------");
					#endif					
					//found path
					path.Add(end);
					return findShortestPath(path);
				}
			}

			while(closedSet.ContainsKey(openSet[0])){
				openSet.RemoveAt(0);
			}

			closest = GetClosestToEnd(openSet);
			//Debug.Log(closest+" is closest");
			if(!closedSet.ContainsKey(closest)){
				closedSet.Add(closest,0);
			}
			if(!path.Contains(closest))
				path.Add(closest);
		}

		return null;
	}

	private Vector2[] findShortestPath(List<Coordinate> path){
		List<Vector2> result = new List<Vector2>();
		Coordinate current = path.GetLast();
		if(path.Count<=1){
			return new Vector2[]{current};
		}

		for(int i= path.Count-2; i>=0;i--){
			result.Add(current.GetAsMapPosition(offset));
			if(path[i].tentativeDistance>current.tentativeDistance){
				current = path[i];
			}
		}
		result.Reverse();
#if UNITY_EDITOR && DEGUBLOG
		foreach(Vector2 v in result){
			Debug.Log(v);
		}
		Debug.Log("----------------------");
#endif
		return result.ToArray();
	}

	/// <summary>
	/// Gets the Coordinate closest to the given one from the openSet. The list is assumed to be sorted into ascending order.
	/// </summary>
	/// <returns>The closest to.</returns>
	/// <param name="endPosition">End position.</param>
	/// <param name="openSet">Open set.</param>
	private Coordinate GetClosestToEnd (List<Coordinate> openSet){
		return openSet[0];
	}

	private struct Coordinate : System.IComparable<Coordinate>, System.Collections.Generic.IEqualityComparer<Coordinate>{

		public int x;
		public int y;
		public int tentativeDistance;

		public static Coordinate Zero = new Coordinate(0,0);

		public Coordinate(Coordinate old, int z){
			this.x = old.x;
			this.y = old.y;
			tentativeDistance = z;
		}

		public Coordinate(int x, int y){
			this.x = x;
			this.y = y;
			tentativeDistance = -1;
		}

		public Coordinate(Vector2 v){
			x = (int)v[0];
			y = (int)v[1];
			tentativeDistance = -1;
		}

		public Coordinate(Transform t, int[] offset){
			x = Mathf.RoundToInt(t.position.x)-offset[0];
			y = Mathf.RoundToInt(t.position.y)-offset[1];
			tentativeDistance = -1;
		}

		public Vector2 GetAsMapPosition(int[] offset){
			return new Vector2(x+offset[0],y+offset[1]);
		}

		/// <summary>
		/// Compares the tentative Distance from this Coordinate to the the current execution's endPoint
		/// </summary>
		/// <returns> 0 if equal, 1 if farther, -1 if closer</returns>
		/// <param name="other">Other coordinate.</param>
		public int CompareTo (Coordinate other){
			if(tentativeDistance==other.tentativeDistance)
				return 0;
			if(tentativeDistance>other.tentativeDistance){
				return 1;
			}else
				return -1;
		}

		public bool Equals (Coordinate x, Coordinate y){
			return x.Equals(y);
		}

		public int GetHashCode (Coordinate obj){
			return obj.GetHashCode();
		}

		public bool Equals(int x, int y){
			return this.x == x && this.y == y;
		}

		public override bool Equals (object obj){
			if(ReferenceEquals(this,obj))
				return true;
			bool result = (obj != null);
			result &= obj is Coordinate;
			if(result){
				Coordinate c = (Coordinate)obj;
				result = (c.x == x && c.y == y);
			}
			return result;
		}

		public override int GetHashCode (){
			return 0;
		}

		public Coordinate CalculateDistanceTo(Coordinate other){
			return new Coordinate(this,Mathf.Abs(this.x-other.x)+Mathf.Abs(this.y-other.y));
		}

		public bool IsDiagonalto(int x, int y){
			return this.x != x && this.y != y;
		}

		public static bool operator==(Coordinate c, Coordinate v){
			return c.x == v.x && c.y==v.y;
		}

		public static bool operator!=(Coordinate c, Coordinate v){
			return c.x != v.x || c.y!=v.y;
		}

		public static implicit operator Vector2 (Coordinate other){
			return new Vector2(other.x,other.y);
		}

		public static implicit operator Coordinate (Vector2 other){
			return new Coordinate(other);
		}
		public override string ToString (){
			return string.Format ("[Coordinate: x={0}, y={1}, tentativeDistance={2}]", x, y, tentativeDistance);
		}
		
		
	}
}
