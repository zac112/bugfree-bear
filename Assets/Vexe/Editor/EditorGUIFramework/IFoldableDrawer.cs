using UnityEngine;
using UnityEditor;

namespace EditorGUIFramework
{
	public interface IFoldableDrawer
	{
		string Key { get; }
		bool Foldout { get; set; }
	}
}