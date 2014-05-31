using UnityEngine;
using System.Collections.Generic;
using System;
using ShowEmAll;

public class FSMState : BetterBehaviour
{
	[SerializeField, Readonly]
	private List<FSMTransition> transitions = new List<FSMTransition>();

	[SerializeField]
	private FSMStateType stateType = FSMStateType.Other;

	/// <summary>
	/// The transitions branching from this state
	/// </summary>
	public List<FSMTransition> Transitions { get { return transitions; } }

	/// <summary>
	/// The state's type (Start, End or Other)
	/// </summary>
	public FSMStateType StateType { get { return stateType; } set { stateType = value; } }

	/* <<< Editor Stuff ⚡ TREAD LIGHTLY ⚡ >>> */
	#region
#if UNITY_EDITOR
	[HideInInspector]
	[SerializeField]
	private FSMTriggerStateEditorData mEditorData = new FSMTriggerStateEditorData();

	[Serializable]
	public class FSMTriggerStateEditorData
	{
		public bool fold_state;
		public bool fold_newTransition;
		public string newTransitionName;
		public string tf_stateName;
		public bool isObjectField = true;
	}
#endif
	#endregion
}