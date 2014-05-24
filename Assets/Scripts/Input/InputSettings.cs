using UnityEngine;
using System.Collections;

[System.Serializable]
public class InputSettings : ScriptableObject {

	[SerializeField]
	private KeyCode moveLeft;
	public KeyCode Left {
		get{ return moveLeft; }
		set{ moveLeft = value; }
	}

	[SerializeField]
	private KeyCode moveRight;
	public KeyCode Right{
		get{ return moveRight; }
		set{ moveRight = value; }
	}

	[SerializeField]
	private KeyCode moveUp;
	public KeyCode Up{
		get{ return moveUp; }
		set{ moveUp = value; }
	}

	[SerializeField]
	private KeyCode moveDown;
	public KeyCode Down{
		get{ return moveDown; }
		set{ moveDown = value; }
	}

	[SerializeField]
	private KeyCode run;
	public KeyCode Run{
		get{ return run; }
		set{ run = value; }
	}

	[SerializeField]
	private KeyCode interact;
	public KeyCode Interact{
		get{ return interact; }
		set{ interact = value; }
	}

}
