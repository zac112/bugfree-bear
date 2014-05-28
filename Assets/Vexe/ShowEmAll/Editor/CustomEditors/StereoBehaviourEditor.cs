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

namespace ShowEmAll
{
	[CustomEditor(typeof(StereoBehaviour), true)]
	public class StereoBehaviourEditor : BetterEditor<StereoBehaviour>
	{
		private const bf PUBLIC = bf.Public | bf.Instance;
		private const bf ALL = PUBLIC | bf.NonPublic;
		private IEnumerable<FieldInfo> fields;
		private IEnumerable<PropertyInfo> properties;
		private IEnumerable<MethodInfo> methods;
		private IListDrawer<GLWrapper, GLOption> listDrawer;
		private Dictionary<PropertyInfo, CSPropertyDrawer<GLWrapper, GLOption>> propertyDrawers;
		private Dictionary<MethodInfo, MethodDrawer<GLWrapper, GLOption>> methodDrawers;
		private Dictionary<string, sp> spDic = new Dictionary<string, sp>();
		//private GUIStyle _bold;
		//GUIStyle bold
		//{
		//	get
		//	{
		//		if (_bold == null)
		//		{
		//			_bold = new GUIStyle(EditorStyles.label);
		//			//_bold.contentOffset = new Vector2(0, -5);
		//		}
		//		return _bold;
		//	}
		//}

		// Foldouts / keys
		#region
		private string key { get { return StuffHelper.GetTargetID(target); } }
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

		private void OnEnable()
		{
			listDrawer = new IListDrawer<GLWrapper, GLOption>();
			propertyDrawers = new Dictionary<PropertyInfo, CSPropertyDrawer<GLWrapper, GLOption>>();
			methodDrawers = new Dictionary<MethodInfo, MethodDrawer<GLWrapper, GLOption>>();

			var mb = TypedTarget;

			fields = mb.GetFields(PUBLIC | bf.NonPublic)
					.Where(f => f.IsPublic ? true : f.IsDefined(typeof(SerializeField)));

			spDic = fields.ToDictionary(f => f.Name,
										f => serializedObject.FindProperty(f.Name));

			properties = mb.GetProperties(PUBLIC)
					.Where(p => p.IsDefined(typeof(ShowProperty)));

			foreach (var p in properties)
			{
				propertyDrawers[p] = new CSPropertyDrawer<GLWrapper, GLOption>(gui, p, target);
			}

			methods = mb.GetMethods(typeof(void), null, ALL, false)
					.Where(m => m.IsDefined(typeof(ShowMethodAttribute)));

			foreach (var m in methods)
			{
				var d = new MethodDrawer<GLWrapper, GLOption>(m, ALL);
				d.Set(gui, target);
				methodDrawers[m] = d;
			}
		}

		void DoMembers<T>(Func<bool> foldout, IEnumerable<T> members, Action<T> perform) where T : MemberInfo
		{
			int count = members.Count();
			if (count == 0 || !foldout()) return;

			bool insideBox = (Settings.DisplayOptions & Settings.MembersDisplay.DisplayInsideBox) > 0;
			bool showNumbers = (Settings.DisplayOptions & Settings.MembersDisplay.ShowNumbers) > 0;
			bool showSplitter = (Settings.DisplayOptions & Settings.MembersDisplay.ShowSplitter) > 0;

			gui.Space(-5f);
			gui.IndentedBlock(insideBox ? GUI.skin.textArea : GUIStyle.none, .25f, () =>
			{
				gui.Space(5f);
				int i = 0;
				foreach (var m in members)
				{
					gui.HorizontalBlock(() =>
					{
						if (showNumbers)
						{
							gui.NumericLabel(i + 1);
						}
						else gui.Space(10f);
						perform(m);
					});
					if (showSplitter && i < count - 1)
						gui.Splitter();
					else gui.Space(2f);
					i++;
				}
			});
		}
		public override void OnInspectorGUI()
		{
			StuffHelper.SerializedObjectBlock(serializedObject, () =>
			{
				DoMembers(DoFieldsFoldout, fields, fieldInfo =>
				{
					if (fieldInfo.FieldType.IsIList())
					{
						gui.VerticalBlock(() =>
						{
							listDrawer.advancedCollection = fieldInfo.IsDefined(typeof(AdvancedCollectionAttribute));
							listDrawer.Draw(gui, fieldInfo, target);
						});
					}
					else
					{
						gui.PropertyField(spDic[fieldInfo.Name]);
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
	}
}