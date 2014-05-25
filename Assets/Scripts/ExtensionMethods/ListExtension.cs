using UnityEngine;
using System.Collections.Generic;

public static class ListExtension{

	/// <summary>
	/// Maintains the list's descending order after adding the given element. Assumes the list is already sorted to descending order.
	/// </summary>
	public static void MaintainDescending<T>(this List<T> list, T element) where T : System.IComparable<T>{
		Organize(list, element, (int i, int i1) => { return list[i].CompareTo(list[i1])>0;});
	}
	/// <summary>
	/// Maintains the list's ascending order after adding the given element. Assumes the list is already sorted to ascending order.
	/// </summary>
	public static void MaintainAscending<T>(this List<T> list, T element) where T : System.IComparable<T>{
		Organize(list, element, (int i, int i1) => { return list[i].CompareTo(list[i1])<0;});
	}

	private static void Organize<T>(List<T> list, T element, System.Func<int, int, bool> predicate){
		list.Add(element);
		for(int i=list.Count-1; i>0; i--){
			if(predicate(i, i-1))
				Swap(list, i, i-1);
			else
				break;
		}
	}

	private static void Swap<T> (List<T> list, int index1, int index2)
	{
		T temp = list[index1];
		list[index1] = list[index2];
		list[index2] = temp;
	}

	/// <summary>
	/// Gets the last element in the list.
	/// </summary>
	/// <returns>The last element in the list. The deafult value for the type stored is returned if the list is null or there are no elements in the list</returns>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T GetLast<T> (this List<T> list){
		if(list == null || list.Count == 0)
			return default(T);
		return list[list.Count-1];
	}
}
