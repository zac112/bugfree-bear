using System;
using UnityEngine;
using uFAction;
using ShowEmAll;

public class FSMTransition : BetterBehaviour
{
	[SerializeField, Readonly]
	private FSMState toState;

	[ShowDelegate("On Transition", @canSetArgsFromEditor: false), SerializeField]
	private TransitionAction onTransition = new TransitionAction();

	/// <summary>
	/// The state to transit to
	/// </summary>
	public FSMState ToState { get { return toState; } set { toState = value; } }

	/// <summary>
	/// The delegate to invoke when making a transition.
	/// </summary>
	public TransitionAction OnTransition { get { return onTransition; } }

	/// <summary>
	/// Makes the transition. Invokes OnTransition passing 'this' as the argument
	/// </summary>
	[ShowMethod]
	public void MakeTransition()
	{
		print(this + " making transition...");
		onTransition.Invoke(this);
	}

	[Serializable]
	public class TransitionAction : UnityAction<FSMTransition>{}

	// TODO: Refactor & Clean up
	/* <<< Editor Stuff ⚡ TREAD LIGHTLY ⚡ >>> */
	#region
#if UNITY_EDITOR
	[HideInInspector]
	[SerializeField]
	private FSMTriggerTransitionEditorData mEditorData = new FSMTriggerTransitionEditorData();
	[Serializable]
	public class FSMTriggerTransitionEditorData
	{
		public bool fold_transition;
		public bool isObjectField = true;
	}
#endif
	#endregion
}