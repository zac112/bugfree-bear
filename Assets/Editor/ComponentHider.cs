using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ComponentHider : EditorWindow
{

	[MenuItem("ComponentHider/Start")]
	public static void Init ()
	{
		GetWindow<ComponentHider> ();
	}

	[MenuItem("ComponentHider/HideTransform #&T")]
	public static void HideTransform(){
		Transform t = Selection.activeGameObject.transform;
		if(t.hideFlags == HideFlags.HideInInspector)
			t.hideFlags = HideFlags.None;
		else
			t.hideFlags = HideFlags.HideInInspector;
	}

	GameObject target;
	Dictionary<Component, bool> componentDic;
	Component[] components = new Component[0];
	HideFlags[] values = new HideFlags[0];
	string[] componentNames;
	GameObject prefab;

	private void OnEnable ()
	{
		target = null;
		UpdateTarget ();
		EditorApplication.update += RefreshWindow;
	}

	int counter = 0;

	private void RefreshWindow ()
	{
		// refresh about ten times a second
		if (target != null && counter >= 200) {
			UpdateList ();
			counter = 0;
			Repaint ();
		}
		counter++;
	}

	public void OnGUI ()
	{
		UpdateTarget ();

		if(!IsValidTarget())
			return;

		DrawComponents();

		GUILayout.Label (string.Empty);

		ApplyButton("Show all", HideFlags.None);
		ApplyButton("Hide all",HideFlags.HideInInspector);

	}

	private bool IsValidTarget(){
		if (target == null) {
			GUILayout.Label ("Please select a GameObject.");
			return false;
		}else if(PrefabUtility.GetPrefabType(target) == PrefabType.Prefab){
			GUILayout.Label("Editing of Prefabs is not supported.");
			return false;
		}
		return true;
	}

	private void DrawComponents(){
		GUILayout.Label ("Select Components to show:");

		int i = 0;
		foreach (Component c in components) {
			GUILayout.BeginHorizontal ();
			{
				if (c != null) {
					c.hideFlags = values[i];
					values[i] = HideToggle (componentNames [i], c);
				} else {
					GUILayout.Label ("Missing Component");
				}
			}
			GUILayout.EndHorizontal ();
			i++;
		}
	}
	private HideFlags HideToggle (string label, Object obj)
	{

		EditorGUI.BeginChangeCheck ();
		bool visible = EditorGUILayout.ToggleLeft (label, obj.hideFlags == HideFlags.None);
		if (EditorGUI.EndChangeCheck ()) {
			if (visible) {
				//obj.hideFlags = HideFlags.None;
				return HideFlags.None;
			} else {
				return HideFlags.HideInInspector;
				//obj.hideFlags = HideFlags.HideInInspector;
			}
		}
		return obj.hideFlags;
	}

	private void ApplyButton(string buttonName, HideFlags state){
		if(GUILayout.Button(buttonName)){
			for(int j=0; j<values.Length; j++){
				values[j] = state;
			}
		}
	}

	private void RefreshComponentNames (Component[] componentList)
	{
		if (componentNames == null || componentNames.Length != componentList.Length)
			componentNames = new string[componentList.Length];

		for (int i=0; i<componentList.Length; i++) {
			if (componentList [i] == null)
				continue;

			string componentName = Regex.Match (componentList [i].ToString (), "\\(.*").Value;
			componentName = componentName.Replace ("(UnityEngine.", "");
			int startIndex = componentName.IndexOf ('(') + 1;
			componentNames [i] = componentName.Substring (startIndex, componentName.Length - startIndex - 1);
		}
	}

	private void UpdateTarget ()
	{
		if (Selection.activeGameObject != null) {
			target = Selection.activeGameObject;

			UpdateList ();
		}else
			target = null;
	}

	private void UpdateList ()
	{
		Component[] newComponents = target.GetComponents<Component> ();
		HideFlags[] newValues = new HideFlags[newComponents.Length];

		for(int i=0; i< newComponents.Length; i++){
			if(newComponents[i] == null)
				continue;

			newValues[i] = newComponents[i].hideFlags;
			for(int j=0;j<components.Length;j++){
				if(components[j] == null) 
					continue;
				if(components[j].Equals(newComponents[i])){
					newValues[i] = values[j];
					break;
				}
			}
		}

		components = newComponents;
		values = newValues;

		RefreshComponentNames (components);
	}

	private void OnDisable ()
	{
		EditorApplication.update -= RefreshWindow;
	}
}
