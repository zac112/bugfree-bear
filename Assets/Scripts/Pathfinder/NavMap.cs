//#define DEBUGLOGS

using UnityEngine;
using System.Collections.Generic;
using System;

public class NavMap
{
	private bool[, ,] navMap;
	private int[] offset;

	public NavMap(int width, int height, int offsetX, int offsetY)
	{
		offset = new int[] { offsetX, offsetY };
		navMap = new bool[width - offset[0], height - offset[1], 1];
	}

	public void Register(Transform position, bool walkable)
	{
		Coordinate c = Coordinate.GetCoordinate(position, offset);
		Register(c.x, c.y, walkable);
	}

	public void Register(int x, int y, bool walkable)
	{
#if UNITY_EDITOR && DEBUGLOGS
		if(!IsValidCoordinate(x,y)){
			Debug.LogError("Attempting to register coordinate out of NavMap bounds ("+x+","+y+")! Do you have a Tile with negative coordinates?");
			return;
		}
#endif
		navMap[x, y, 0] = walkable;
	}

	private bool IsValidCoordinate(int x, int y)
	{
		return x >= navMap.GetLowerBound(0) && y >= navMap.GetLowerBound(1) && x <= navMap.GetUpperBound(0) && y <= navMap.GetUpperBound(1);
	}

	private bool IsWalkable(Coordinate coordinate)
	{
		return IsWalkable(coordinate.x, coordinate.y);
	}

	private bool IsWalkable(int x, int y)
	{
		return IsValidCoordinate(x,y) && navMap[x, y, 0];
	}

	private List<Coordinate> GetAvailableNeighbors(Coordinate position, Dictionary<Coordinate, Coordinate> closedSet)
	{
		List<Coordinate> result = GetWalkableNeighbors(position);
		for (int i = result.Count - 1; i >= 0; i--)
		{
			if (closedSet.ContainsKey(result[i]))
			{
				result.RemoveAt(i);
			}
		}
		return result;
	}

	private List<Coordinate> GetWalkableNeighbors(Coordinate position)
	{
		List<Coordinate> result = new List<Coordinate>(8);
		for (int x = position.x - 1; x <= position.x + 1; x++)
		{
			for (int y = position.y - 1; y <= position.y + 1; y++)
			{
				if (IsValidCoordinate(x, y) && !position.IsDiagonalto(x, y) && IsWalkable(x, y) && !position.Equals(x, y))
				{
					result.Add(Coordinate.GetCoordinate(x, y));
				}
			}
		}
		return result;
	}

	public void FindPath(Vector2 startPos, Vector2 endPos, List<Vector2> resultList)
	{
		FindPath((Coordinate)startPos, (Coordinate)endPos, resultList);
	}

	public void FindPath(Transform startTrans, Transform endTrans, List<Vector2> resultList)
	{
		Coordinate start = Coordinate.GetCoordinate(startTrans, offset);
		Coordinate end = Coordinate.GetCoordinate(endTrans, offset);
		FindPath(start, end, resultList);
	}

	private void FindPath(Coordinate startCoord, Coordinate endCoord, List<Vector2> resultList)
	{
		if (startCoord == endCoord) //already there..
		{
			resultList.Clear();
			resultList.Add(endCoord.GetAsMapPosition(offset));
		}


		if(!IsWalkable(startCoord)){
#if UNITY_EDITOR && DEBUGLOGS			
			Debug.LogError("Invalid start position("+startPosition+")! Unable to find route.");
#endif
			return;
		}
		if(!IsWalkable(endCoord)){
#if UNITY_EDITOR && DEBUGLOGS			
			Debug.LogError("Invalid end position("+endPosition+")! Unable to find route.");
#endif			
			return;
		}
		List<Coordinate> path = new List<Coordinate>();
		PriorityStack<Coordinate> openSet = new PriorityStack<Coordinate>();
		//dictionary from coordinate to the previous coordinate in the path
		Dictionary<Coordinate, Coordinate> closedSet = new Dictionary<Coordinate, Coordinate>();

		openSet.First = startCoord;
		openSet.First.CalculateDistanceTo(endCoord);
		path.Add(startCoord);
		closedSet.Add(startCoord, Coordinate.Null);

		int counter = 0;
		while (!openSet.IsEmpty())
		{
			counter++;
			if (counter > 1000)
			{
				Debug.Log("infinite loop?");
				break;
			}
			Coordinate closest = Coordinate.Null;
			Coordinate current = path.GetLast();
			List<Coordinate> unvisitedNeighbors = GetAvailableNeighbors(path.GetLast(), closedSet);
			for (int i = 0; i < unvisitedNeighbors.Count; i++)
			{
				unvisitedNeighbors[i].CalculateDistanceTo(endCoord);
				unvisitedNeighbors[i].Previous = current;
				openSet.First = unvisitedNeighbors[i];
				if (endCoord.Equals(unvisitedNeighbors[i]))
				{
					//found path
					closedSet.Add(unvisitedNeighbors[i], current);
					ResolveShortestPath(closedSet, startCoord, endCoord, resultList);
					return;
				}
			}

			while (!openSet.IsEmpty() && closedSet.ContainsKey(openSet.First))
			{
				openSet.RemoveFirst();
			}

			if (openSet.IsEmpty()) //unable to find path
				break;

			closest = openSet.First;

			if (!closedSet.ContainsKey(closest))
			{
				closedSet.Add(closest, closest.Previous);
			}

			path.Add(closest);
		}

		Coordinate.ClearPool();
	}

	private void ResolveShortestPath(Dictionary<Coordinate, Coordinate> closedSet, Coordinate start, Coordinate end, List<Vector2> resultList)
	{
		resultList.Clear();
		if (closedSet.Count <= 1)
		{
			resultList.Add(end);
			Coordinate.ClearPool();
			return;
		}

		Coordinate current = end;
		while (current != start)
		{
			resultList.Add(current.GetAsMapPosition(offset));
			current = closedSet[current];
		}

		resultList.Reverse();
#if UNITY_EDITOR && DEGUBLOG
		foreach(Vector2 v in result){
			Debug.Log(v);
		}
		Debug.Log("----------------------");
#endif
		Coordinate.ClearPool();
		return;
	}

	private class Coordinate : IComparable<Coordinate>, IEqualityComparer<Coordinate>
	{
		public int x;
		public int y;
		public int tentativeDistance;

		private Coordinate previous;
		public Coordinate Previous
		{
			get { return previous; }
			set { previous = value; }
		}

		private static Stack<Coordinate> coordinatePool = new Stack<Coordinate>();
		private static Stack<Coordinate> takenCoordinates = new Stack<Coordinate>();
		private static int count = 0;

		public static Coordinate Null = new Coordinate(-1, -1);

		public static Coordinate GetCoordinate(int x, int y)
		{
			if (coordinatePool.Count == 0)
			{
				coordinatePool.Push(new Coordinate(x, y));
			}
			Coordinate result = coordinatePool.Pop();
			result.x = x;
			result.y = y;
			takenCoordinates.Push(result);
			return result;
		}

		public static Coordinate GetCoordinate(Vector2 v)
		{
			return GetCoordinate((int)v[0], (int)v[1]);
		}

		public static Coordinate GetCoordinate(Transform t, int[] offset)
		{
			return GetCoordinate(
				Mathf.RoundToInt(t.position.x) - offset[0],
				Mathf.RoundToInt(t.position.y) - offset[1]);
		}

		public Vector2 GetAsMapPosition(int[] offset)
		{
			return new Vector2(x + offset[0], y + offset[1]);
		}

		private Coordinate(int x, int y)
		{
			count++;
			//Debug.Log("Created "+count);
			this.x = x;
			this.y = y;
			tentativeDistance = -1;
		}

		public static void ClearPool()
		{
			while (takenCoordinates.Count > 0)
			{
				coordinatePool.Push(takenCoordinates.Pop());
			}
		}

		/// <summary>
		/// Compares the tentative Distance from this Coordinate to the the current execution's endPoint
		/// </summary>
		/// <returns> 0 if equal, 1 if farther, -1 if closer</returns>
		/// <param name="other">Other coordinate.</param>
		public int CompareTo(Coordinate other)
		{
			if (tentativeDistance == other.tentativeDistance)
				return 0;

			return tentativeDistance > other.tentativeDistance ? 1 : -1;
		}

		public bool Equals(Coordinate x, Coordinate y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(Coordinate obj)
		{
			return obj.GetHashCode();
		}

		public bool Equals(int x, int y)
		{
			return this.x == x && this.y == y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			bool result = (obj != null);
			result &= obj is Coordinate;
			if(result){
				Coordinate c = (Coordinate)obj;
				result = (c.x == x && c.y == y);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public void CalculateDistanceTo(Coordinate other)
		{
			tentativeDistance = DistanceTo(other);
		}

		public bool IsDiagonalto(int x, int y)
		{
			return this.x != x && this.y != y;
		}

		public bool IsNeighborTo(Coordinate other)
		{
			return !IsDiagonalto(other.x, other.y) && DistanceTo(other) == 1;
		}

		private int DistanceTo(Coordinate other)
		{
			return Mathf.Abs(this.x - other.x) + Mathf.Abs(this.y - other.y);
		}

		public static bool operator ==(Coordinate c, Coordinate v)
		{
			return c.x == v.x && c.y == v.y;
		}

		public static bool operator !=(Coordinate c, Coordinate v)
		{
			return !(c == v);
		}

		public static implicit operator Vector2(Coordinate other)
		{
			return new Vector2(other.x, other.y);
		}

		public static implicit operator Coordinate(Vector2 other)
		{
			return Coordinate.GetCoordinate(other);
		}

		public override string ToString()
		{
			return string.Format("[Coordinate: x={0}, y={1}, tentativeDistance={2}]", x, y, tentativeDistance);
		}
	}
}