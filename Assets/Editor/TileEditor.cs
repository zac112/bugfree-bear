using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
[CanEditMultipleObjects]
public class TileEditor: Editor{

	TileSO data;
	string content;
	string walkableEnding = " currently <b>walkable</b>!";
	string unWalkableEnding = " currently <b>NOT walkable</b>!";
	string singularBeginning = "This tile is";
	string pluralBeginning = "These tiles are";
	Tile _target;

	delegate void DrawGUI();

	DrawGUI DrawAppropriateGUI;

	void OnEnable(){
		FindData();

		_target = (Tile)target;
		if(Selection.gameObjects.Length > 1)
			content = singularBeginning;
		else
			content = pluralBeginning;

		DetermineGUI();
	}

	public override void OnInspectorGUI(){
		if(Selection.gameObjects.Length <= 1){
			content = singularBeginning;
			DetermineGUI();
		}else{
			bool allSame = true;
			for(int i=1; i<Selection.gameObjects.Length; i++){
				allSame &= (Selection.gameObjects[i-1].GetComponent<Tile>().IsWalkable() == Selection.gameObjects[i].GetComponent<Tile>().IsWalkable());
			}
			content = pluralBeginning;
			DetermineGUI();
			if(!allSame){
				content = pluralBeginning + " a mix of both.";
				DrawGUI walkable = WalkableGUI;
				DrawGUI unwalkable = UnWalkableGUI;
				DrawAppropriateGUI = walkable+unwalkable;
			}
		}

		GUI.skin.label.richText = true;
		GUILayout.Label(content);

		DrawAppropriateGUI();
	}

	private void FindData(){
		Object[] temp = Resources.FindObjectsOfTypeAll(typeof(TileSO));
		if(temp == null || temp.Length == 0)
			data = ScriptableObject.CreateInstance<TileSO>();
		else
			data = temp[0] as TileSO;
	}

	private void UnWalkableGUI(){
		data.WalkableParent = EditorGUILayout.ObjectField("Parent for walkable tiles", data.WalkableParent, typeof(GameObject), true) as GameObject;
		if(GUILayout.Button("Make walkable")){
			foreach(GameObject go in Selection.gameObjects){
				go.GetComponent<Tile>().transform.parent = data.WalkableParent.transform;
			}
		}
	}

	private void WalkableGUI(){
		data.UnwalkableParent = EditorGUILayout.ObjectField("Parent for unwalkable tiles", data.UnwalkableParent, typeof(GameObject), true) as GameObject;
		if(GUILayout.Button("Make unwalkable")){
			foreach(GameObject go in Selection.gameObjects){
				go.GetComponent<Tile>().transform.parent = data.UnwalkableParent.transform;
			}
		}
	}

	private void DetermineGUI(){
		if(_target.IsWalkable()){
			DrawAppropriateGUI = WalkableGUI;
			content += walkableEnding;
		}else{
			DrawAppropriateGUI = UnWalkableGUI;
			content += unWalkableEnding;
		}
	}
}
