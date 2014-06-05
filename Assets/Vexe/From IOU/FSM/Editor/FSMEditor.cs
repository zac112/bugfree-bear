using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Vexe.EditorHelpers;
using Vexe.EditorExtensions;
using Vexe.RuntimeExtensions;
using EditorGUIFramework;
using Vexe.RuntimeHelpers;
using ShowEmAll;
using System;
using sp = UnityEditor.SerializedProperty;

[CustomEditor(typeof(FSM), true)]
public class FSMEditor : BetterBehaviourEditor
{
	bool hasInitStyles;
	public void InitStyles()
	{
		TransitionsLabel = GuiHelper.CreateLabel(13, new Vector2(1, -2));
		StatesLabel = GuiHelper.CreateLabel(16, new Vector2(0, -3));
	}

	private GUIStyle StatesLabel;
	private GUIStyle TransitionsLabel;

	private sp spFsmData;
	private sp spStates;
	private sp spDebug;
	private sp spFoldoutMain;
	private FSM fsm;
	private List<FSMState> states;
	private bool hasRemovedTransition;
	private bool hasRemovedState;

	protected override void OnEnable()
	{
		base.OnEnable();
		spFsmData = serializedObject.FindProperty("mEditorData");
		spStates = serializedObject.FindProperty("states");
		spFoldoutMain = spFsmData.FindPropertyRelative("fold_main");
		spDebug = spFsmData.FindPropertyRelative("debug");
		fsm = target as FSM;
		states = fsm.States;
	}

	private void OnDisable()
	{
		GuiHelper.GreenStyleDuo.DestroyTextures();
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (!hasInitStyles)
		{
			hasInitStyles = true;
			InitStyles();
		}

		EventsHelper.OnUndoRedoPerformed(Repaint);

		serializedObject.Update();

		gui.Space(3f);
		DoHeader();
		DoStates();

		serializedObject.ApplyModifiedProperties();
	}

	private void DoHeader()
	{
		var spFoldoutNewstate = spFsmData.FindPropertyRelative("fold_newState");
		gui.VerticalBlock(GUI.skin.button, () =>
		{
			gui.HorizontalBlock(() =>
			{
				gui.Space(10f);
				gui.Foldout(spFoldoutMain);
				gui.Space(-7f);
				gui.Label("States", StatesLabel);
				gui.Space(7f);
				gui.Foldout("+s", spFoldoutNewstate);
				gui.FlexibleSpace();

				gui.CheckButton(spDebug, "debug mode", MiniButtonStyle.Left);

				//SetDebugMode(spDebug.boolValue));

				gui.MiniButton(GuiHelper.Folds.DefaultFoldSymbol, "Fold all", () =>
					SetFoldAll(false));

				gui.MiniButton(GuiHelper.Folds.DefaultExpandSymbol, "Expand all", () =>
					SetFoldAll(true));

				////gui.MiniButton("s", "Shift states", () =>
				////	states.Shift(true));

				gui.MiniButton("p", "Populate states from children", () =>
				{
					fsm.Populate();
					serializedObject.Update();
					spFoldoutMain.SetBoolIfNot(true);
				});

				gui.MiniButton("r", "Reset FSM", () =>
					fsm.ResetFSM());

				gui.EnabledBlock(spStates.arraySize > 0, () =>
					gui.ClearButton("states (Can't be undone)", MiniButtonStyle.Right, () =>
					{
						bool sure = EditorUtility.DisplayDialog("Confirm action", "Are you sure you want to clear all states? (Can't be undone!)", "Yes", "No");
						if (sure)
						{
							foreach (sp s in spStates)
								DestroyImmediate(s.gameObject());
							spStates.ClearArray();
						}
					})
				);
			});
			// Add new state
			#region
			if (spFoldoutNewstate.boolValue)
			{
				gui.HorizontalBlock(() =>
				{
					var spFsmStateName = spFsmData.FindPropertyRelative("stateName");

					gui.ChangeBlock(
						() => spFsmStateName.DrawProperty("New state"),
						() => serializedObject.ApplyModifiedProperties()
					);

					gui.EnabledBlock(!string.IsNullOrEmpty(spFsmStateName.stringValue), () =>
						gui.AddButton("state", MiniButtonStyle.Right, () =>
						{
							var newState = fsm.CreateNewState(spFsmStateName.stringValue);
							Undo.RegisterCreatedObjectUndo(newState.gameObject, "Created new state");
							serializedObject.Update();
							spFoldoutMain.SetBoolIfNot(true);
						})
					);
				});
			}
			else gui.Space(-3f);
			#endregion
		});
	}

	void DoStates()
	{
		if (!spFoldoutMain.boolValue || spStates.arraySize == 0)
			return;

		gui.Space(-4);
		gui.IndentedBlock(GUI.skin.box, .3f, () =>
		{
			// Draw states
			GuiHelper.GreenStyleDuo.Reset();
			for (int iLoop = 0; iLoop < spStates.arraySize; )
			{
				int i = iLoop;
				var spState = spStates.GetAt(i);

				// Just in case one deletes a state manually
				if (spState.objectReferenceValue == null)
				{
					spStates.RemoveAt(i);
					continue;
				}

				hasRemovedState = false;

				SerializedObject soState;
				var spStateEditorData = spState.FindPropertyRelativeInMB("mEditorData", out soState);
				var spTransitions = soState.FindProperty("transitions");
				var spFoldoutState = spStateEditorData.FindPropertyRelative("fold_state");

				if (i != 0) gui.Splitter();

				soState.Update();

				// State field and Util buttons
				DoState(i, spState, soState, spStateEditorData, spTransitions, spFoldoutState, () =>
				{
					fsm.RemoveState(i);
					serializedObject.Update();
					hasRemovedState = true;
				});

				if (!hasRemovedState)
					iLoop++;
			}
		});

		gui.Space(5f);
	}

	private void DoState(int i, sp spState, SerializedObject soState, sp spStateEditorData, sp spTransitions, sp spFoldoutState, Action removeState)
	{
		var nextColor = GuiHelper.GreenStyleDuo.NextColor;
		gui.HorizontalBlock(states[i] == fsm.CurrentState ? GuiHelper.GreenStyleDuo.CurrentStyle : GUIStyle.none, () =>
		{
			string expand, fold;
			if (states[i] != fsm.StartState)
			{
				expand = GuiHelper.Folds.DefaultExpandSymbol;
				fold = GuiHelper.Folds.DefaultFoldSymbol;
			}
			else
			{
				expand = GuiHelper.Folds.AlternateExpandSymbol;
				fold = GuiHelper.Folds.AlternateFoldSymbol;
			}

			gui.CustomFoldout(spFoldoutState, expand, fold);

			var spStateIsObjectField = spStateEditorData.FindPropertyRelative("isObjectField");

			gui.ColorBlock(nextColor, () =>
			{
				string label = (i + 1) + "- ";

				gui.MutablePropertyObjectField(
				   spState,
				   spStateIsObjectField,
				   label,
					//Repaint,
				   delegate { },
				   GLWrapper.NUMERIC_LABEL_WIDTH
				);

				if (spStateIsObjectField.boolValue)
				{
					gui.GetLastRect(fieldRect =>
					{
						GuiHelper.PingField(fieldRect, spState.gameObject(), MouseCursor.Link);
						Action setState = () =>
						{
							fsm.StartState = fsm.CurrentState = states[i];
							//Repaint();
						};
						GuiHelper.CustomActionField(fieldRect, setState, true, EventsHelper.MouseEvents.IsRMB_MouseDown(), MouseCursor.Link);
					});
				}
			});

			gui.InspectButton(spState.gameObject());
			gui.RemoveButton("state (Can't be undone!)", MiniButtonStyle.Right, removeState);
		});

		if (hasRemovedState)
			return;

		soState.ApplyModifiedProperties(); // if we don't do this, then newly added transitions won't appear

		if (spFoldoutState.boolValue)
		{
			gui.IndentedBlock(GUI.skin.box, 1.8f, () =>
			{
				var spNewTransitionName = spStateEditorData.FindPropertyRelative("newTransitionName");
				DoTransitionsHeader(
					@apply: () => soState.ApplyModifiedProperties(),
					@clear: () =>
					{
						bool sure = EditorUtility.DisplayDialog("Confirm action", "Are you sure you want to clear all transitions? (Can't be undone!)", "Yes", "No");
						if (sure)
						{
							foreach (sp t in spTransitions)
								DestroyImmediate(t.gameObject());
							spTransitions.ClearArray();
						}
					},
					@enableClear: spTransitions.arraySize > 0,
					@add: () =>
					{
						var newTransition = fsm.CreateNewTransition(spNewTransitionName.stringValue, spState.GetValue<FSMState>());
						soState.Update();
						Undo.RegisterCreatedObjectUndo(newTransition.gameObject, "Created new transition");
					},
					@enableAdd: !string.IsNullOrEmpty(spNewTransitionName.stringValue),
					@spFoldoutNewTransition: spStateEditorData.FindPropertyRelative("fold_newTransition"),
					@spNewTransitionName: spNewTransitionName
				);

				DoTransitions(spTransitions, states[i], soState);
			});
		}
	}

	private void DoTransitions(sp spTransitions, FSMState fromState, SerializedObject soState)
	{
		GuiHelper.BlueColorDuo.Reset();
		for (int jLoop = 0; jLoop < spTransitions.arraySize; )
		{
			int j = jLoop;
			var spTransition = spTransitions.GetAt(j);
			hasRemovedTransition = false;

			if (spTransition.objectReferenceValue == null)
			{
				spTransitions.RemoveAt(j);
				continue;
			}

			DoTransition(j, spTransition, () =>
			{
				fsm.RemoveTransition(fromState, j);
				hasRemovedTransition = true;
			}, fromState);

			if (!hasRemovedTransition)
				jLoop++;
		}
	}

	private void DoTransition(int j, sp spTransition, Action removeTransition, FSMState fromState)
	{
		SerializedObject soTransition;
		var spTransitionEditorData = spTransition.FindPropertyRelativeInMB("mEditorData", out soTransition);
		var spFoldoutTransition = spTransitionEditorData.FindPropertyRelative("fold_transition");

		soTransition.Update();

		// Transition name and Util buttons
		gui.HorizontalBlock(() =>
		{
			gui.CustomFoldout(spFoldoutTransition);

			gui.ColorBlock(GuiHelper.BlueColorDuo.NextColor, () =>
			{
				var spTransitionIsObjectField = spTransitionEditorData.FindPropertyRelative("isObjectField");
				string label = (j + 1) + "- ";

				gui.MutablePropertyObjectField(
				   spTransition,
				   spTransitionIsObjectField,
				   label,
					//Repaint,
				   delegate { },
				   GLWrapper.NUMERIC_LABEL_WIDTH
			   );

				if (spTransitionIsObjectField.boolValue)
					gui.GetLastRect(lastRect => GuiHelper.PingField(lastRect, spTransition.gameObject()));
			});

			var go = spTransition.gameObject();

			gui.InspectButton(go, MiniButtonStyle.Left);

			gui.RemoveButton("transition (Can't be undone)", MiniButtonStyle.Right, removeTransition);
		});

		if (hasRemovedTransition)
			return;

		soTransition.ApplyModifiedProperties();

		// "To state" fold
		if (spFoldoutTransition.boolValue)
		{
			DoToState(j, soTransition, fromState);
		}
	}

	private void DoToState(int j, SerializedObject soTransition, FSMState currentIteratedState)
	{
		gui.HorizontalBlock(() =>
		{
			gui.Indent(1.5f);

			var spToState = soTransition.FindProperty("toState");

			bool isNull = spToState.objectReferenceValue == null;

			gui.DraggableLabelField("To state", isNull ? "Select state -> " : spToState.objectReferenceValue.name,
				spToState.objectReferenceValue,
				70);

			GuiHelper.PingField(spToState.objectReferenceValue, !isNull);

			if (fsm.CurrentState == currentIteratedState)
			{
				gui.EnabledBlock(EditorApplication.isPlaying && !isNull, () =>
					gui.MiniButton("t", "Make transition", MiniButtonStyle.Left,
						currentIteratedState.Transitions[j].MakeTransition)
				);
			}
			else gui.Space(GLWrapper.DEFAULT_MINI_WIDTH);

			gui.SelectionButton("state", () =>
				SelectionWindow.Show<FSMState>(
					() => fsm.States.Disinclude(currentIteratedState).ToArray(),
					spToState.GetValue<FSMState>,
					s => { spToState.objectReferenceValue = s; soTransition.ApplyModifiedProperties(); serializedObject.ApplyModifiedProperties(); },
					s => s.name,
					"States")
			);

			soTransition.ApplyModifiedProperties();
		});
	}

	private void DoTransitionsHeader(Action apply, Action clear, bool enableClear, Action add, bool enableAdd, sp spFoldoutNewTransition, sp spNewTransitionName)
	{
		gui.VerticalBlock(GUI.skin.box, () =>
		{
			// Transitions label, fold and Clear button
			gui.HorizontalBlock(() =>
			{
				gui.Label("Transitions", TransitionsLabel);
				gui.Space(7f);

				gui.ChangeBlock(() =>
				{
					gui.Foldout("+t", spFoldoutNewTransition);
					gui.FlexibleSpace();
					gui.EnabledBlock(enableClear, () =>
						gui.ClearButton("transitions (Can't be undone)", MiniButtonStyle.Right, clear)
					);
				}, apply);
			});

			// Add new transition
			if (spFoldoutNewTransition.boolValue)
			{
				gui.HorizontalBlock(() =>
				{
					gui.ChangeBlock(
						() => spNewTransitionName.DrawProperty("New transition"),
						apply
					);

					gui.EnabledBlock(enableAdd, () =>
						gui.AddButton("transition", MiniButtonStyle.Right, add)
					);
				});
			}
		});
	}

	// old code... could have done much better now
	private void SetFoldAll(bool Expand)
	{
		if (!spFsmData.FindPropertyRelative("fold_main").boolValue || spStates.arraySize == 0)
			return;

		var spCurrentFoldoutDepth = spFsmData.FindPropertyRelative("currentFoldDepth");
		var spMaxFoldoutDepth = spFsmData.FindPropertyRelative("maxFoldDepth");

		if (Expand)
		{
			if (spCurrentFoldoutDepth.intValue >= spMaxFoldoutDepth.intValue)
				return;
			spCurrentFoldoutDepth.intValue++;
		}

		switch (spCurrentFoldoutDepth.intValue)
		{
			case 1:
				spStates.SetFoldAll("mEditorData", "fold_state", Expand);
				break;
			case 2:
				spStates.SetFoldAllInternal("transitions", "mEditorData", "fold_transition", Expand);
				break;
		}

		if (!Expand)
		{
			spCurrentFoldoutDepth.intValue--;
			if (spCurrentFoldoutDepth.intValue < 0) spCurrentFoldoutDepth.intValue = 0;
		}
	}
}