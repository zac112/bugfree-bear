using UnityEngine;
using UnityEditor;

public class GridSnapper{

	[MenuItem("BugFreeBear/Grid Snapper/Start")]
	public static void Start(){
		SceneView.onSceneGUIDelegate += SceneUpdate;
	}

	[MenuItem("BugFreeBear/Grid Snapper/Stop")]
	public static void Stop(){
		SceneView.onSceneGUIDelegate -= SceneUpdate;
	}

	public static void SceneUpdate(SceneView view){
		if (Selection.activeTransform != null) {
			Transform tform = Selection.activeTransform;
			Vector3 temp = new Vector3(Mathf.FloorToInt(tform.position.x),Mathf.FloorToInt(tform.position.y), tform.position.z);
			Selection.activeTransform.position = temp;
		}
	}

}
