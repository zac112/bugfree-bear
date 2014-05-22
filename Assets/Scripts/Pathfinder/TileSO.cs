using UnityEngine;

[System.Serializable]
public class TileSO : ScriptableObject {

	[SerializeField]
	private GameObject walkableParent; //for serialization
	public GameObject WalkableParent{
		get{ return walkableParent; }
		set{ walkableParent = value; }
	}
	[SerializeField]
	private GameObject unwalkableParent; //for serialization
	public GameObject UnwalkableParent{
		get{ return unwalkableParent; }
		set{ unwalkableParent = value; }
	}

	void OnEnable(){
		hideFlags = HideFlags.DontSave;
	}
}
