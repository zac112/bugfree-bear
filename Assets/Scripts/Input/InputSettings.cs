using System;
using ShowEmAll;
using UnityEngine;

[Serializable]
public class InputSettings : ScriptableObject
{
	[SerializeField, FriendlyEnum]
	private KeyCode moveLeft;

	[SerializeField, FriendlyEnum]
	private KeyCode moveRight;

	[SerializeField, FriendlyEnum]
	private KeyCode moveUp;

	[SerializeField, FriendlyEnum]
	private KeyCode moveDown;

	[SerializeField, FriendlyEnum]
	private KeyCode run;

	[SerializeField, FriendlyEnum]
	private KeyCode interact;

	public KeyCode Left
	{
		get { return moveLeft; }
		set { moveLeft = value; }
	}

	public KeyCode Right
	{
		get { return moveRight; }
		set { moveRight = value; }
	}

	public KeyCode Up
	{
		get { return moveUp; }
		set { moveUp = value; }
	}

	public KeyCode Down
	{
		get { return moveDown; }
		set { moveDown = value; }
	}

	public KeyCode Run
	{
		get { return run; }
		set { run = value; }
	}

	public KeyCode Interact
	{
		get { return interact; }
		set { interact = value; }
	}
}