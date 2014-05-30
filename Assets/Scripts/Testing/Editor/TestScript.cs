using UnityEngine;
using ShowEmAll;
using System.IO;
using System;
using UnityEditor;

public class TestScript : BetterBehaviour
{
	[ShowMethod]
	public void LogLoadedObjects(string toPath)
	{
		LogLoadedObjects(toPath, typeof(UnityEngine.Object));
	}

	[ShowMethod]
	public void LogLoadedObjects(string toPath, string typeFullName, string asmName)
	{
		LogLoadedObjects(toPath, Type.GetType(GetQualifiedName(typeFullName, asmName)));
	}

	[ShowMethod]
	public void FreeResources()
	{
		Resources.UnloadUnusedAssets();
		EditorUtility.UnloadUnusedAssets();
	}

	[ShowMethod]
	public void DestroyAllOfType(string typeFullName, string typeAsm)
	{
		DestroyAllOfType(Type.GetType(GetQualifiedName(typeFullName, typeAsm)));
	}

	public void DestroyAllOfType(Type type)
	{
		foreach (var o in Resources.FindObjectsOfTypeAll(type))
			DestroyImmediate(o);
	}

	public void LogLoadedObjects(string toPath, Type type)
	{
		using (var writer = new StreamWriter(File.OpenWrite(toPath)))
		{
			var all = Resources.FindObjectsOfTypeAll(type);
			foreach (var o in all)
				writer.WriteLine(o.GetType().FullName);
		}
	}

	private string GetQualifiedName(string typeFullName, string typeAsm)
	{
		return typeFullName + ", " + typeAsm;
	}
}