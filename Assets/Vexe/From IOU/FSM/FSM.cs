using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Vexe.RuntimeExtensions;
using System;
using ShowEmAll;

/// <summary>
/// Notes:
/// - Click right click on a state to set at as the current state.
///		notice that at edit-time, current state == start state.
/// - The highlighted state == current state
/// - The state with the unique foldout == start state
/// - Hold Ctrl and click the middle mouse button while hovering
///		over a state/transition field to switch between an draggable label field and a text field
///		(you can drag a draggable field around - but you can't drop to it)
///	- To set things up in your states/transitions gameObjects, you don't have to select them and
///		inspect them manually, just click on "Inspect" to show up a new inspector window inspecting
///		your state/transition
///	- If you toggle debug mode, you get a 't' button besides the transitions of the current state
///		which acts as "MakeTransition" - available in playmode only
///	- Removing stuff can't be undone atm - I'll see if I can do something about it with DEVBUS
///		I totally gave up hope solving it with Unity's Undo http://answers.unity3d.com/questions/642425/get-undodestroyobjectimmediate-to-work-nicely-with.html
///	- I'll slowly but surely get rid of the editor data in the runtime classes and use BetterPrefs instead
///	- Everything should be done from the FSM editor, that is, setting what state a tranition goes to,
///		adding/removing states, inspecting stuff, etc.
///		That's why toState in FMSTranstiion and states in FSMState are readonly (but yet visible in the inspector)
/// </summary>
public class FSM : BetterBehaviour
{
	[SerializeField, HideInInspector]
	private List<FSMState> states = new List<FSMState>();

	[SerializeField, HideInInspector]
	private FSMState currentState;

	[SerializeField, HideInInspector]
	private FSMState startState;

	/// <summary>
	/// Returns a list of all the FSM's states
	/// </summary>
	public List<FSMState> States { get { return states; } }

	/// <summary>
	/// Gets/Sets the current state
	/// </summary>
	public FSMState CurrentState
	{
		get { return currentState; }
		set
		{
			print(name + ": current state: " + (value == null ? "null" : value.name));
			currentState = value;
			if (Application.isPlaying) EnableCurrentDisableRest();
		}
	}

	/// <summary>
	/// Gets/Sets the start state
	/// </summary>
	public FSMState StartState
	{
		get { return startState; }
		set
		{
			if (startState != null)
			{
				startState.name = startState.name.Remove(0, startState.name.IndexOf(' ') + 1);
				startState.StateType = FSMStateType.Other;
			}

			startState = value;
			startState.StateType = FSMStateType.Start;
			startState.name = "(Start) " + startState.name;
		}
	}

	public void ResetFSM()
	{
		CurrentState = startState;
	}

	private void EnableCurrentDisableRest()
	{
		currentState.gameObject.SetActiveIfNot(true);

		var others = States.Where(s => s != CurrentState);
		foreach (var state in others)
			state.gameObject.SetActiveIfNot(false);
	}

	private void ReInit()
	{
		startState = States.FirstOrDefault(s => s.StateType == FSMStateType.Start);
		if (startState == null)
			throw new UnityException("[FSMTrigger] " + this + " No start state specified. Init failed");
		ResetFSM();
	}

	public void Populate()
	{
		states.Clear();
		states = GetComponentsInChildren<FSMState>().ToList();
		ReInit();
	}

	[ShowMethod]
	public void Test()
	{
	}

	/* <<< Editor Stuff ⚡ TREAD LIGHTLY ⚡ >>> */
	#region
#if UNITY_EDITOR
	[HideInInspector, SerializeField]
	protected FSMTriggerEditorData mEditorData = new FSMTriggerEditorData();
	[Serializable]
	protected class FSMTriggerEditorData
	{
		public int maxFoldDepth = 2;
		public int currentFoldDepth;

		public bool fold_main;
		public bool fold_newState;
		public string stateName;
		public bool debug;
	}
#endif
	#endregion
}