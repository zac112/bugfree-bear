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
}
