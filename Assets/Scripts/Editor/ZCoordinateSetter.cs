using UnityEngine;
using UnityEditor;

public class ZCoordinateSetter {

	[MenuItem("BugFreeBear/Set Z Coordinates for selected objects &#z")]
	public static void SetCoordinates(){
		foreach(GameObject go in Selection.gameObjects){
			Transform tf = go.transform;
			tf.position = new Vector3 (tf.position.x, tf.position.y, -(Nav.MAX_DEPTH-(tf.position.y/10)));
		}
	}
}
