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
		public const bf Public = bf.Public | bf.Instance;
		public const bf AllBindings = Public | bf.NonPublic;

		private string key { get { return RTHelper.GetTargetID(target); } }

		List<MembersCategory> membersCategories;

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

			membersCategories = new List<MembersCategory>();
			var targetType = target.GetType();
			var targetAtts = targetType.GetCustomAttributes<DefineCategoryAttribute>(@inherit: true);
			var allMembers = targetType.GetMembers(AllBindings);
			var nonCategorizedMembers = allMembers.ToList();
			var mapper = new DMCMapper(serializedObject, gui)
			{
				IsVisibleField = field => field != null && SerializationLogic.IsSerializableField(field),
				IsVisibleProperty = property => property != null && property.IsDefined<ShowPropertyAttribute>(),
				IsVisibleMethod = method => method != null && method.IsDefined<ShowMethodAttribute>(),
				MethodBindings = AllBindings
			};

			foreach (var catAtt in targetAtts)
			{
				string catName = catAtt.name;
				var catMembers = from member in allMembers
								 let cats = member.GetCustomAttributes<CategoryMemberAttribute>()
								 where !cats.IsNullOrEmpty()
								 let catMem = cats[0]
								 where catMem.name == catName
								 orderby catMem.displayOrder
								 select member;

				// catMembers are categorized, thus must be removed from nonCategorizedMembers
				nonCategorizedMembers.BatchRemove(catMembers);

				var newCategory = new MembersCategory(key, gui)
				{
					Name = catName,
					Order = catAtt.displayOrder,
				};

				mapper.MapMembersToCategory(catMembers, newCategory);
				membersCategories.Add(newCategory);
			}

			// Default categories
			{
				var fieldsCat = new MembersCategory(key, gui) { Name = "Fields", Order = 0 };
				var propertiesCat = new MembersCategory(key, gui) { Name = "Properties", Order = 1 };
				var methodsCat = new MembersCategory(key, gui) { Name = "Methods", Order = 2 };
				mapper.MapMembersToCategories(nonCategorizedMembers, fieldsCat, propertiesCat, methodsCat);
				membersCategories.AddMultiple(fieldsCat, propertiesCat, methodsCat);
			}

			// Finally, sort the cats based on their Order
			membersCategories.Sort();
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

	internal class DMCMapper // not Devil May Cry... but Drawer-Member-Category
	{
		private SerializedObject serializedObject;
		private GLWrapper gui;

		public Predicate<FieldInfo> IsVisibleField { get; set; }
		public Predicate<PropertyInfo> IsVisibleProperty { get; set; }
		public Predicate<MethodInfo> IsVisibleMethod { get; set; }
		public BindingFlags MethodBindings { get; set; }

		private Object target { get { return serializedObject.targetObject; } }

		public DMCMapper(SerializedObject serializedObject, GLWrapper gui)
		{
			this.serializedObject = serializedObject;
			this.gui = gui;
		}

		public void MapMembersToCategories(IEnumerable<MemberInfo> members,
			MembersCategory fieldsCat, MembersCategory propertiesCat, MembersCategory methodsCat)
		{
			foreach (var member in members)
			{
				var field = member as FieldInfo;
				if (IsVisibleField(field))
					fieldsCat[field] = MapField(field);
				else
				{
					var property = member as PropertyInfo;
					if (IsVisibleProperty(property))
						propertiesCat[property] = MapProperty(property);
					else
					{
						var method = member as MethodInfo;
						if (IsVisibleMethod(method))
							methodsCat[method] = MapMethod(method);
					}
				}
			}
		}

		public void MapMembersToCategory(IEnumerable<MemberInfo> members, MembersCategory category)
		{
			MapMembersToCategories(members, category, category, category);
		}

		public Action MapMethod(MethodInfo method)
		{
			var methodDrawer = new MethodDrawer<GLWrapper, GLOption>(gui, target)
			{
				bindings = MethodBindings,
				method = method,
			};
			return methodDrawer.Draw;
		}

		public Action MapProperty(PropertyInfo property)
		{
			var propDrawer = new CSPropertyDrawer<GLWrapper, GLOption>(gui, target)
			{
				property = property
			};
			return propDrawer.Draw;
		}

		public Action MapField(FieldInfo field)
		{
			BaseDrawer<GLWrapper, GLOption> drawer = null;
			var spField = serializedObject.FindProperty(field.Name);
			if (field.IsDefined<InlineAttribute>())
			{
				var inlineDrawer = new InlineDrawer(gui, target);
				inlineDrawer.property = spField;
				inlineDrawer.label = field.Name.SplitPascalCase();
				drawer = inlineDrawer;
			}
			else if (field.FieldType.IsIList())
			{
				var listDrawer = new IListDrawer<GLWrapper, GLOption>(gui, target);
				listDrawer.awesomeCollection = field.IsDefined<AwesomeCollectionAttribute>();
				listDrawer.readonlyCollection = field.IsDefined<ReadonlyAttribute>();
				listDrawer.fieldInfo = field;
				drawer = listDrawer;
			}
			else
			{
				drawer = new DefaultDrawer<GLWrapper, GLOption>(
					spField, field.Name.SplitPascalCase(), gui);
			}
			return drawer.Draw;
		}
	}

	internal class MembersCategory : IComparable<MembersCategory>
	{
		private readonly string key;
		private readonly GLWrapper gui;
		private readonly Dictionary<MemberInfo, Action> mapping; // TODO: Does it really have to be a dict?

		public string Name { get; set; }
		public float Order { get; set; }

		public Action this[MemberInfo member]
		{
			get { return mapping[member]; }
			set { mapping[member] = value; }
		}

		public MembersCategory(string key, GLWrapper gui)
		{
			this.key = key;
			this.gui = gui;
			mapping = new Dictionary<MemberInfo, Action>();
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
			int count = mapping.Count;
			if (count == 0 || !DoHeader()) return;

			bool insideBox = (Settings.DisplayOptions & Settings.MembersDisplay.DisplayInsideBox) > 0;
			bool showNumbers = (Settings.DisplayOptions & Settings.MembersDisplay.ShowNumbers) > 0;
			bool showSplitter = (Settings.DisplayOptions & Settings.MembersDisplay.ShowSplitter) > 0;

			gui.Space(-5f);
			gui.IndentedBlock(insideBox ? GUI.skin.textArea : GUIStyle.none, .25f, () =>
			{
				gui.Space(5f);
				int i = 0;
				foreach (var kvp in mapping)
				{
					var drawer = kvp.Value;
					gui.HorizontalBlock(() =>
					{
						if (showNumbers)
						{
							gui.NumericLabel(i + 1);
						}
						else gui.Space(10f);
						gui.VerticalBlock(drawer.Invoke);
					});

					if (showSplitter && i < count - 1)
						gui.Splitter();
					else gui.Space(2f);
					i++;
				}
			});
		}

		public int CompareTo(MembersCategory other)
		{
			return Order.CompareTo(other.Order);
		}
	}

	internal static class SerializationLogic
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