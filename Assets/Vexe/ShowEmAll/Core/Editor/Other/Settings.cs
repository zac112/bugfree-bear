using System;
using UnityEngine;
using Vexe.EditorHelpers;
using Vexe.RuntimeHelpers;
using UnityEditor;
using System.IO;

#pragma warning disable 0414

namespace ShowEmAll
{
	[Serializable]
	public class Settings : ScriptableObject
	{
		[Flags]
		public enum MembersDisplay { ShowSplitter = 1, ShowNumbers = 2, DisplayInsideBox = 4 };

		public const string PackageName = "ShowEmAll";
		public static string PackageDirectory = DirectoryHelper.GetDirectoryPath(PackageName);
		private static string SettingsPath = EditorHelper.ScriptableAssetsPath + "/ShowEmAllSettings.asset";
		private static Settings instance;

		[EnumMask, SerializeField]
		private MembersDisplay displayOptions = MembersDisplay.DisplayInsideBox | MembersDisplay.ShowSplitter;

		public static MembersDisplay DisplayOptions { get { return Instance.displayOptions; } }
		private static string _currentPath;
		private static string currentPath { get { return DirectoryHelper.LazyGetDirectoryPath(ref _currentPath, "ScriptableAssets"); } }
		public static Settings Instance
		{
			get { return EditorHelper.LazyLoadScriptableAsset<Settings>(ref instance, currentPath + "/Settings.asset", true); }
		}

		[MenuItem("Component/" + PackageName + "/Settings")]
		public static void SelectSettings()
		{
			EditorHelper.SelectObject(Instance);
		}
	}
}