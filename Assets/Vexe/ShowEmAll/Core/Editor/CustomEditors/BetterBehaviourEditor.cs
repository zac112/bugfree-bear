using UnityEngine;
using UnityEditor;
using Vexe.RuntimeExtensions;
using System.Linq;
using System.Reflection;
using bf = System.Reflection.BindingFlags;
using System.Collections.Generic;
using EditorGUIFramework;
using ShowEmAll.DrawMates;
using sp = UnityEditor.SerializedProperty;
using System;
using Vexe.RuntimeHelpers;
using Vexe.EditorExtensions;

namespace ShowEmAll
{
	[CustomEditor(typeof(BetterBehaviour), true)]
	public class BetterBehaviourEditor : BetterEditor<BetterBehaviour>
	{
		private const bf Public = bf.Public | bf.Instance;
		private const bf AllBindings = Public | bf.NonPublic;
		private FieldInfo[] fields;
		private PropertyInfo[] properties;
		private MethodInfo[] methods;
		private IListDrawer<GLWrapper, GLOption> listDrawer;
		private Dictionary<PropertyInfo, CSPropertyDrawer<GLWrapper, GLOption>> propertyDrawers;
		private Dictionary<MethodInfo, MethodDrawer<GLWrapper, GLOption>> methodDrawers;
		private Dictionary<string, sp> spDic = new Dictionary<string, sp>();

		// Foldouts / keys
		#region
		private string key { get { return RTHelper.GetTargetID(target); } }
		private string fieldsKey { get { return key + "Fields"; } }
		private bool fieldsFoldout
		{
			get { return StuffHelper.GetFoldoutValue(fieldsKey); }
			set { StuffHelper.SetFoldoutValue(fieldsKey, value); }
		}
		private string propertiesKey { get { return key + "Properties"; } }
		private bool propertiesFoldout
		{
			get { return StuffHelper.GetFoldoutValue(propertiesKey); }
			set { StuffHelper.SetFoldoutValue(propertiesKey, value); }
		}
		private string methodsKey { get { return key + "Methods"; } }
		private bool methodsFoldout
		{
			get { return StuffHelper.GetFoldoutValue(methodsKey); }
			set { StuffHelper.SetFoldoutValue(methodsKey, value); }
		}

		private bool DoFoldout(string label, Func<bool> get, Action<bool> set)
		{
			bool current = get();
			gui.HorizontalBlock(GUI.skin.box, () =>
			{
				gui.Space(10f);
				gui.Foldout(label, current, new GLOption { ExpandWidth = true }, newValue =>
				{
					if (newValue != current)
						set(newValue);
				});
			});
			return get();
		}
		private bool DoFieldsFoldout()
		{
			return DoFoldout("Fields", () => fieldsFoldout, newValue => fieldsFoldout = newValue);
		}
		private bool DoPropertiesFoldout()
		{
			return DoFoldout("Properties", () => propertiesFoldout, newValue => propertiesFoldout = newValue);
		}
		private bool DoMethodsFoldout()
		{
			return DoFoldout("Methods", () => methodsFoldout, newValue => methodsFoldout = newValue);
		}
		#endregion

		// 1- get all public fields, or fields mocked with [SerializeField]
		// 1.5- get all properties that have a custom property attribute defined
		// 2- draw each found field (via PropertyField)
		// 3- draw the properties fetched in 1.5

		protected virtual void OnEnable()
		{
			listDrawer = new IListDrawer<GLWrapper, GLOption>(gui, target);
			propertyDrawers = new Dictionary<PropertyInfo, CSPropertyDrawer<GLWrapper, GLOption>>();
			methodDrawers = new Dictionary<MethodInfo, MethodDrawer<GLWrapper, GLOption>>();

			var mb = TypedTarget;

			if (mb == null)
			{
				Debug.LogError(string.Concat(new string[] {
					"Casting target object to BetterBehaviour failed! Something's wrong. ", 
					"Maybe you switched back and inherited MonoBehaviour instead of BetterBehaviour ",
					"and you still had your gameObject selected? ",
					"If that's the case then the BetterBehaviourEditor is still there in memory ",
					"and so this could be resolved by reselcting your gameObject. ",
					"Destroying this BetterBehaviourEditor instance anyway..."
				}));
				DestroyImmediate(this);
				return;
			}

			fields = (from f in mb.GetFields(AllBindings)
					  let isHidden = f.IsDefined(typeof(HideInInspector))
					  where !isHidden && (f.IsPublic || f.IsDefined(typeof(SerializeField)))
					  select f)
					 .ToArray();

			spDic = fields.ToDictionary(f => f.Name,
										f => serializedObject.FindProperty(f.Name));

			properties = mb.GetProperties(AllBindings)
					.Where(p => p.IsDefined(typeof(ShowProperty)))
					.ToArray();

			foreach (var p in properties)
			{
				propertyDrawers[p] = new CSPropertyDrawer<GLWrapper, GLOption>(gui, target)
				{
					property = p
				};
			}

			methods = mb.GetMethods(typeof(void), null, AllBindings, false)
					.Where(m => m.IsDefined(typeof(ShowMethodAttribute)))
					.ToArray();

			foreach (var m in methods)
			{
				methodDrawers[m] = new MethodDrawer<GLWrapper, GLOption>(gui, target)
				{
					bindings = AllBindings,
					method = m,
				};
			}
		}

		void DoMembers<T>(Func<bool> foldout, T[] members, Action<T> perform) where T : MemberInfo
		{
			int len = members.Length;
			if (len == 0 || !foldout()) return;

			bool insideBox = (Settings.DisplayOptions & Settings.MembersDisplay.DisplayInsideBox) > 0;
			bool showNumbers = (Settings.DisplayOptions & Settings.MembersDisplay.ShowNumbers) > 0;
			bool showSplitter = (Settings.DisplayOptions & Settings.MembersDisplay.ShowSplitter) > 0;

			gui.Space(-5f);
			gui.IndentedBlock(insideBox ? GUI.skin.textArea : GUIStyle.none, .25f, () =>
			{
				gui.Space(5f);
				for (int iLoop = 0; iLoop < len; iLoop++)
				{
					int i = iLoop;
					var m = members[i];
					gui.HorizontalBlock(() =>
					{
						if (showNumbers)
						{
							gui.NumericLabel(i + 1);
						}
						else gui.Space(10f);
						perform(m);
					});
					if (showSplitter && i < len - 1)
						gui.Splitter();
					else gui.Space(2f);
				}
			});
		}
		public override void OnInspectorGUI()
		{
			DoScriptHeaderField();

			StuffHelper.SerializedObjectBlock(serializedObject, () =>
			{
				DoMembers(DoFieldsFoldout, fields, fieldInfo =>
				{
					if (fieldInfo.FieldType.IsIList())
					{
						gui.VerticalBlock(() =>
						{
							listDrawer.advancedCollection = fieldInfo.IsDefined(typeof(AdvancedCollectionAttribute));
							listDrawer.readonlyCollection = fieldInfo.IsDefined(typeof(ReadonlyAttribute));
							listDrawer.fieldInfo = fieldInfo;
							listDrawer.Draw();
						});
					}
					else
					{
						gui.PropertyField(spDic[fieldInfo.Name], fieldInfo.Name.SplitPascalCase());
					}
				});

				DoMembers(DoPropertiesFoldout, properties, pinfo =>
				{
					gui.VerticalBlock(() => propertyDrawers[pinfo].Draw());
				});

				DoMembers(DoMethodsFoldout, methods, minfo =>
				{
					gui.VerticalBlock(() => methodDrawers[minfo].Draw());
				});
			});
		}

		private void DoScriptHeaderField()
		{
			var script = serializedObject.FindProperty("m_Script");
			gui.ObjectField("Script", script.objectReferenceValue, value => script.objectReferenceValue = value);
		}
	}
}