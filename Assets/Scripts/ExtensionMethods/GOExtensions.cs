using UnityEngine;
using System.Collections.Generic;

public static class GOExtensions {

	public static T GetInterface<T>(this GameObject go) where T : class{
		Component c = go.GetComponent(typeof(T));
		T result = c as T;
		return result;
	}

	public static T[] GetInterfaces<T>(this GameObject go) where T: class{
		Component[] components = go.GetComponents(typeof(T));
		List<T> result = new List<T>();
		
		foreach(Component c in components){
			result.Add(c as T);
		}
		
		return result.ToArray();
	}
}
