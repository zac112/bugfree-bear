using Vexe.RuntimeExtensions;
using UnityEngine;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Reflection;
using Vexe.RuntimeHelpers;
using EditorGUIFramework;

namespace uFAction.Editors
{
	public class AdvancedEditor<TWrapper, TOption> : BaseEditor<TWrapper, TOption>, ISubHeadedEditor
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		// Member fields
		#region
		private bool hasRemovedTarget;
		private bool hasRemovedMethod;
		#endregion

		/* <<< STRUCTURE >>> */
		#region
		// InternalDraw
		// >
		// DrawGOs
		// --- DrawSubHeader
		// --- DrawGOsEntries
		// ----- DrawGOField
		// ----- DrawTargets
		// >
		// DrawTargets
		// --- DrawTargetsHeader
		// --- DrawTargetsEntries
		// ----- DrawTargetField
		// ----- DrawMethods
		// >
		// DrawMethods
		// --- DrawMethodsHeader
		// --- DrawMethodsEntries
		// ----- DrawMethodField
		#endregion

		// Others
		#region
		protected override void InternalDraw()
		{
			gui.IndentedBlock(GUI.skin.box,
				@indentLevel: HeaderIndent,
				@beginningVerticalSpace: 0f,
				@endingVerticalSpace: 5f,
				@block: DrawGOs);
		}

		private void DrawGOs()
		{
			gui.Space(2.5f);
			DrawSubHeader(
				@showInvoke: IsParameterlessDelegate || CanSetArgsFromEditor,
				@enableInvoke: true,
				@showClear: true,
				@enableClear: true,
				@clear: ClearGOs
			);

			gui.Splitter();

			DrawGOEntries();
		}

		private void DrawGOEntries()
		{
			Theme.GameObjectsColors.Reset();
			for (int iLoop = 0; iLoop < goEntries.Count; )
			{
				int i = iLoop;
				var goEntry = goEntries[i];
				if (goEntry.go == null) { iLoop++; continue; }
				string gKey = GetKey(goEntry) + i;

				hasRemovedGo = false;

				DrawField(
					@color: Theme.GameObjectsColors.NextColor,
					@index: i,
					@field: () => DoGOField(goEntry, newGo => SetGoEntry(goEntry, i, newGo)),
					@moveDown: () => MoveDown(goEntries, i),
					@moveUp: () => MoveUp(goEntries, i),
					@showFoldout: true,
					@enableFoldout: true,
					@foldout: () => Foldout(gKey),
					@enableRemove: true,
					@whatToRemove: "game object",
					@remove: () => RemoveGo(i)
				);

				if (GetBool(gKey))
					DrawTargets(goEntry, gKey);

				if (!hasRemovedGo) iLoop++;
			}
		}

		private void DrawTargets(GOEntry goEntry, string gKey)
		{
			goEntry.TargetEntries = goEntry.TargetEntries.Filter(e => e.Target != null);
			var tEntries = goEntry.TargetEntries;
			Object[] targets = goEntry.Targets;

			Action addNew = () =>
			{
				var t = new TargetEntry(targets[0]);
				BetterPrefs.sSetBool(gKey + (goEntry.TargetEntries.Count), true);
				undo.RecordAddToList(() => goEntry.TargetEntries, t);
			};

			if (tEntries.IsEmpty())
			{
				Log("TargetEntries was empty - adding a new entry");
				//tEntries.Add(new TargetEntry(targets[0]));
				addNew();
			}

			gui.IndentedBlock(GUI.skin.box, IndentLevel, () =>
			{
				DrawNormalHeader(
					@label: "Targets",
					@enableClear: tEntries.Count > 1,
					@enableAdd: !goEntry.IsNa,
					@whatToClear: "targets (there has to be at least one target)",
					@clear: () =>
					{
						undo.RecordClearRangeFromTillEnd(() => goEntry.TargetEntries, 1);
					},
					@whatToAdd: "target object",
					@add: addNew
				);

				DrawTargetEntries(tEntries, targets, goEntry, gKey);
			});
		}

		private void DrawTargetEntries(List<TargetEntry> targetEntries, Object[] targets, GOEntry from, string gKey)
		{
			Theme.TargetColors.Reset();
			for (int iLoop = 0; iLoop < targetEntries.Count; )
			{
				int i = iLoop;
				var tEntry = targetEntries[i];
				int tIndex = targets.IndexOf(tEntry.Target);
				//string tKey = GetKey(tEntry) + i;
				string tKey = gKey + i;

				hasRemovedTarget = false;

				DrawField(
					@color: Theme.TargetColors.NextColor,
					@index: i,
					@field: () => DoTargetsField(
									@currentIndex: tIndex,
									@targets: targets,
									@getCurrent: () => tEntry.Target,
									@useLabelField: tEntry.gameObject == GOHelper.EmptyGO,
									@color: Theme.TargetColors.NextColor,
									@setTarget: newValue =>
										SetTargetEntry(
											@current: tEntry,
											@set: newTarget => from.TargetEntries[i] = newTarget,
											@newValue: newValue
										),
									@setNewIndex: newIndex => tIndex = newIndex
								),
					@moveDown: () => MoveDown(targetEntries, i),
					@moveUp: () => MoveUp(targetEntries, i),
					@showFoldout: true,
					@enableFoldout: true,
					@foldout: () => Foldout(tKey),
					@enableRemove: targetEntries.Count > 1,
					@whatToRemove: "target object",
					@remove: () => undo.RecordRemoveFromList(() => @from.TargetEntries, i, () => hasRemovedTarget = true)
				);

				if (GetBool(tKey))
					DrawMethods(tEntry, GetMethods(targets[tIndex]), tKey);

				if (!hasRemovedTarget) iLoop++;
			}
		}

		private void DrawMethods(TargetEntry tEntry, MethodInfo[] methodInfos, string tKey)
		{
			var mEntries = tEntry.MethodEntries;

			if (mEntries.IsEmpty())
			{
				Log("Methods were empty - Added one");
				mEntries.Add(new MethodEntry());
			}
			bool moreThanOne = mEntries.Count > 1;
			gui.IndentedBlock(GUI.skin.box, IndentLevel, () =>
			{
				DrawNormalHeader(
					@label: "Methods",
					@enableClear: moreThanOne,
					@enableAdd: mEntries[0].Info != null,
					@whatToClear: "Methods (there has to be at least one method)",
					@clear: () =>
					{
						undo.RecordClearRangeFromTillEnd(() => tEntry.MethodEntries, 1);
					},
					@whatToAdd: "method",
					@add: () =>
					{
						//undo.RecordInsertToList(() => tEntry.MethodEntries, 0, new MethodEntry(methodInfos[0]));
						undo.RecordAddToList(() => tEntry.MethodEntries, new MethodEntry(methodInfos[0]));
					});

				DrawMethodEntries(mEntries, methodInfos, tEntry, tKey);
			});
		}

		private void DrawMethodEntries(List<MethodEntry> methodEntries, MethodInfo[] methodInfos, TargetEntry tEntry, string tKey)
		{
			Theme.MethodsColors.Reset();
			for (int iLoop = 0; iLoop < methodEntries.Count; )
			{
				int i = iLoop;
				var mEntry = methodEntries[i];
				bool isValidInfo = mEntry.Info != null;
				bool isParam = !IsParameterlessDelegate;
				//string mKey = GetKey(mEntry) + i;
				string mKey = tKey + i;

				hasRemovedMethod = false;

				if (paramTypes == null && isValidInfo)
					isParam &= mEntry.Info.GetActualParams().Length > 0;

				// Method field
				bool showFoldout = isParam && CanSetArgsFromEditor && isValidInfo;
				DrawField(
					@color: Theme.MethodsColors.NextColor,
					@index: i,
					@field: () => DoMethodsField(
									@mEntry: mEntry,
									@getCurrentInfo: () => tEntry.MethodEntries[i].Info,
									@setInfo: mInfo => SetMethodEntry(@current: mEntry, @set: newValue => tEntry.MethodEntries[i] = newValue, @toValue: mInfo),
									@methods: methodInfos
								),
					@moveDown: () => MoveDown(methodEntries, i),
					@moveUp: () => MoveUp(methodEntries, i),
					@showFoldout: showFoldout,
					@enableFoldout: isValidInfo,
					@foldout: () => Foldout(mKey),
					@enableRemove: methodEntries.Count > 1,
					@whatToRemove: "method",
					@remove: () => undo.RecordRemoveFromList(() => tEntry.MethodEntries, i, () => hasRemovedMethod = true)
				);

				if (showFoldout && GetBool(mKey))
					DoArgEntries(tEntry, i);

				if (!hasRemovedMethod) iLoop++;
			}
		}

		private void MoveDown<T>(IList<T> list, int atIndex)
		{
			list.MoveElementDown(atIndex);
		}

		private void MoveUp<T>(IList<T> list, int atIndex)
		{
			list.MoveElementUp(atIndex);
		}

		protected override void IntegrateDataToEditor()
		{
			base.IntegrateDataToEditor();

			goEntries = new List<GOEntry>();
			var tempList = FilterGOEntries();

			var dupGosPairs = tempList.GetDuplicates(e => e.go);

			foreach (var dupGoPair in dupGosPairs)
			{
				var go = dupGoPair.Key.go;
				var goEntry = new GOEntry(go, dupGoPair.Key.ID);

				var dupGos = tempList.Where((ge, i) => dupGoPair.Value.Contains(i));
				var targets = dupGos.SelectMany(g => g.TargetEntries);
				var dupTargetsPairs = targets.GetDuplicates(t => t.Target);

				foreach (var dupTargetPair in dupTargetsPairs)
				{
					var target = dupTargetPair.Key.Target;
					var tEntry = new TargetEntry(target);

					var dupTargets = targets.Where((me, i) => dupTargetPair.Value.Contains(i));
					var methods = dupTargets.SelectMany(me => me.MethodEntries).ToArray();
					tEntry.MethodEntries.AddRange(methods);
					//goEntry.TargetEntries.Insert(0, tEntry);
					goEntry.TargetEntries.Add(tEntry);
				}

				var nonDupTargets = targets.Where((me, i) => !dupTargetsPairs.SelectMany(p => p.Value).Contains(i));
				//goEntry.TargetEntries.InsertRange(0, nonDupTargets);
				goEntry.TargetEntries.AddRange(nonDupTargets);
				goEntries.Add(goEntry);
			}

			var nonDupGos = tempList.Where((ge, i) => !dupGosPairs.SelectMany(p => p.Value).Contains(i));
			goEntries.AddRange(nonDupGos);
		}
		#endregion

		// Drawings
		#region
		public void DrawSubHeader(bool showInvoke, bool enableInvoke, bool showClear, bool enableClear, Action clear)
		{
			gui.HorizontalBlock(() =>
			{
				gui.BoldLabel("GameObjects");

				InvokeAndClearButtons(showInvoke, enableInvoke, showClear, enableClear, clear);

				gui.CheckButton(
					AdvancedMode,
					value => AdvancedMode = value,
					"advancedMode", MiniButtonStyle.ModRight);
			});
		}

		private void DrawNormalHeader(string label, string whatToClear, bool enableClear, bool enableAdd, Action clear, string whatToAdd, Action add)
		{
			gui.HorizontalBlock(() =>
			{
				// METHODS LABEL
				gui.BoldLabel(label);

				// METHODS CLEAR
				gui.EnabledBlock(enableClear, () =>
					gui.ClearButton(whatToClear, clear));

				// METHODS ADD
				gui.EnabledBlock(enableAdd, () =>
					gui.AddButton(whatToAdd, MiniButtonStyle.ModRight, add));
			});
			gui.Splitter();
		}
		#endregion
	}
}