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
using Object = UnityEngine.Object;

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
		private Dictionary<string, sp> spDic = new Dictionary<string, sp>();

		private string key { get { return RTHelper.GetTargetID(target); } }

		// 1- get all public fields, or fields mocked with [SerializeField]
		// 1.5- get all properties that have a custom property attribute defined
		// 2- draw each found field (via PropertyField)
		// 3- draw the properties fetched in 1.5

		List<MembersCategory> membersCategories = new List<MembersCategory>();

		protected virtual void OnEnable()
		{
			if (TypedTarget == null)
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

			//foreach (var f in mb.GetFields(AllBindings))
			//{
			//	bool b = IsSerializableField(f);
			//}

			// - get fields, props and methods
			// - concat them and get the ones that have a Category attribute on them
			// - create categories for these specified ones
			// - throw what's left of the fields, props and methods to the default categories

			//var members = fields.Concat<MemberInfo>(properties).Concat(methods);

			//var userCategories = members.Select(m => new { m, cat = m.GetCustomAttributes(typeof(CategoryAttribute), false)[0] as CategoryAttribute })
			//							.Where(x => x.cat != null)
			//							.GroupBy(x => x.cat.name)
			//							.OrderBy(x => x.First().cat.order);




			// get what categories are defined on the target type
			// foreach of those categories:
			//		get all the members that have the CategoryMember with the same cat name
			//		pass those members to the category creator to assosiate each member with a drawer
			// sort the categories based on their displayOrder
			// foreach category
			//		sort the members based on their category member display order
			// and then finally


			var fieldsCat = new MembersCategory(key, gui) { Name = "Fields", Order = 0 };
			{
				fields = TypedTarget.GetFields(AllBindings)
								    .Where(f => RefializactionHelper.IsSerializableField(f))
								    .ToArray();

				spDic = fields.ToDictionary(f => f.Name,
											f => serializedObject.FindProperty(f.Name));

				foreach (var f in fields)
				{
					BaseDrawer<GLWrapper, GLOption> drawer = null;
					if (f.IsDefined(typeof(InlineAttribute)))
					{
						var inlineDrawer = new InlineDrawer(gui, target);
						inlineDrawer.property = spDic[f.Name];
						inlineDrawer.label = f.Name.SplitPascalCase();
						drawer = inlineDrawer;
					}
					else if (f.FieldType.IsIList())
					{
						var listDrawer = new IListDrawer<GLWrapper, GLOption>();
						listDrawer.advancedCollection = f.IsDefined<AdvancedCollectionAttribute>();
						listDrawer.readonlyCollection = f.IsDefined<ReadonlyAttribute>();
						listDrawer.fieldInfo = f;
						drawer = listDrawer;
					}
					else
					{
						drawer = new DefaultDrawer<GLWrapper, GLOption>(
							spDic[f.Name], f.Name.SplitPascalCase(), gui);
					}
					fieldsCat[f] = drawer.Draw;
				}
			}

			var propertiesCat = new MembersCategory(key, gui) { Name = "Properties", Order = 1 };
			{
				properties = TypedTarget.GetProperties(AllBindings)
										.Where(p => p.IsDefined<ShowProperty>())
										.ToArray();

				foreach (var p in properties)
				{
					var propDrawer = new CSPropertyDrawer<GLWrapper, GLOption>(gui, target)
					{
						property = p
					};
					propertiesCat[p] = propDrawer.Draw;
				}
			}

			var methodsCat = new MembersCategory(key, gui) { Name = "Methods", Order = 2 };
			{
				methods = TypedTarget.GetMethods(typeof(void), null, AllBindings, false)
									 .Where(m => m.IsDefined<ShowMethodAttribute>())
									 .ToArray();

				foreach (var m in methods)
				{
					var methodDrawer = new MethodDrawer<GLWrapper, GLOption>(gui, target)
					{
						bindings = AllBindings,
						method = m,
					};
					methodsCat[m] = methodDrawer.Draw;
				}
			}

			membersCategories.AddMultiple(fieldsCat, propertiesCat, methodsCat);
		}

		public override void OnInspectorGUI()
		{
			DoScriptHeaderField();

			StuffHelper.SerializedObjectBlock(serializedObject, () =>
			{
				foreach (var category in membersCategories)
					category.Draw();
			});
		}

		private void DoScriptHeaderField()
		{
			var script = serializedObject.FindProperty("m_Script");
			gui.ObjectField("Script", script.objectReferenceValue, value => script.objectReferenceValue = value);
		}
	}

	internal class MembersCategory
	{
		private readonly string key;
		private readonly GLWrapper gui;
		private readonly Dictionary<MemberInfo, Action> dict;

		public string Name { get; set; }
		public float Order { get; set; }

		public Action this[MemberInfo member]
		{
			get { return dict[member]; }
			set { dict[member] = value; }
		}

		public MembersCategory(string key, GLWrapper gui)
		{
			this.key = key;
			this.gui = gui;
			dict = new Dictionary<MemberInfo, Action>();
		}

		// Keys & Foldouts
		#region
		private string GetSubKey()
		{
			return key + Name;
		}
		private void SetFoldout(bool value)
		{
			StuffHelper.SetFoldoutValue(GetSubKey(), value);
		}
		private bool GetFoldout()
		{
			return StuffHelper.GetFoldoutValue(GetSubKey());
		}
		private bool DoHeader(string label, Func<bool> get, Action<bool> set)
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
		private bool DoHeader()
		{
			return DoHeader(Name, () => GetFoldout(), SetFoldout);
		}
		#endregion

		public void Draw()
		{
			int count = dict.Count;
			if (count == 0 || !DoHeader()) return;

			bool insideBox = (Settings.DisplayOptions & Settings.MembersDisplay.DisplayInsideBox) > 0;
			bool showNumbers = (Settings.DisplayOptions & Settings.MembersDisplay.ShowNumbers) > 0;
			bool showSplitter = (Settings.DisplayOptions & Settings.MembersDisplay.ShowSplitter) > 0;

			gui.Space(-5f);
			gui.IndentedBlock(insideBox ? GUI.skin.textArea : GUIStyle.none, .25f, () =>
			{
				gui.Space(5f);
				int i = 0;
				foreach (var kvp in dict)
				{
					var member = kvp.Key;
					var drawer = kvp.Value;
					gui.HorizontalBlock(() =>
					{
						if (showNumbers)
						{
							gui.NumericLabel(i + 1);
						}
						else gui.Space(10f);
						drawer.Invoke();
					});

					if (showSplitter && i < count - 1)
						gui.Splitter();
					else gui.Space(2f);
					i++;
				}
			});
		}
	}

	internal static class RefializactionHelper
	{
		public static bool IsSerializableCollection(Type type)
		{
			if (type.IsArray && type.GetArrayRank() == 1)
			{
				Type elementType = type.GetElementType();
				return !elementType.IsArray && IsSerializableType(elementType);
			}
			return type.GetGenericTypeDefinition() == typeof(List<>) && IsSerializableType(type.GetGenericArguments()[0]);
		}

		public static bool IsSerializableType(Type type)
		{
			return !typeof(Delegate).IsAssignableFrom(type) &&
					type.IsEnum ||
					IsSimpleType(type) ||
					IsStruct(type) ||
					typeof(Object).IsAssignableFrom(type);
		}

		public static bool IsStruct(Type type)
		{
			return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
		}

		public static bool IsSerializableField(FieldInfo field)
		{
			bool isConst = field.IsLiteral && !field.IsInitOnly;
			Type fType = field.FieldType;

			return !isConst &&
				   !field.IsStatic &&
				   (field.IsPublic || field.IsDefined<SerializeField>() && !field.IsDefined<HideInInspector>()) &&
				   (IsSerializableType(fType) || IsCollection(fType) && IsSerializableCollection(fType));
		}

		public static bool IsCollection(Type type)
		{
			return type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
		}

		private static bool IsSimpleType(Type type)
		{
			return type.IsPrimitive || type == typeof(string);
		}
	}
}