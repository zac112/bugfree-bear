using UnityEngine;
using UnityEditor;
using EditorGUIFramework;
using System;
using System.Linq;
using System.Reflection;
using Vexe.RuntimeExtensions;
using Vexe.EditorHelpers;
using bf = System.Reflection.BindingFlags;
using System.Text.RegularExpressions;
using Vexe.RuntimeHelpers;
using Vexe.RuntimeHelpers.Classes;
using Object = UnityEngine.Object;

namespace ShowEmAll.DrawMates
{
	public class MethodDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		private MethodEntry mEntry;
		private bool DEBUG;
		private const string NA = @"N\A";

		protected override string key
		{
			get { return base.key + mEntry.Info.GetFullName(); }
		}

		private Type[] paramTypes { get { return minfo.GetParameters().Select(p => p.ParameterType).ToArray(); } }
		private bf bindings { get { return TypeExtensions.ALL_BINDING; } }
		private MethodInfo minfo { get { return mEntry.Info; } set { mEntry.Info = value; } }

		public MethodDrawer(MethodInfo minfo, BindingFlags bindings)
		{
			mEntry = new MethodEntry(minfo, bindings);
		}

		public void Set(MethodInfo minfo)
		{
			this.minfo = minfo;
		}

		public void Draw(TWrapper gui, MethodInfo minfo, Object target)
		{
			Set(gui, target);
			Set(minfo);
			Draw();
		}

		public override void Draw()
		{
			gui.HorizontalBlock(() =>
			{
				string fullName = minfo.GetFullName();
				bool isParamless = paramTypes.IsEmpty();
				if (isParamless)
				{
					gui.Label(fullName);
				}
				else
				{
					bool current = foldout;
					gui.Foldout(fullName, current, newValue =>
					{
						if (current != newValue)
						{
							foldout = newValue;
							HeightHasChanged();
						}
					});
				}
				gui.FlexibleSpace();
				gui.MiniButton("Invoke", MiniButtonStyle.Right, (TOption)null, () =>
				{
					if (isParamless)
					{
						minfo.Invoke(target, null);
					}
					else
					{
						minfo.Invoke(target, mEntry.ArgumentValues);
					}
				});
			});

			if (!foldout) return;

			DoArgEntries(mEntry);
		}

		// Arg entries stuff
		#region
		protected void DoArgEntries(MethodEntry methodEntry)
		{
			methodEntry.CheckArgs(paramTypes);
			gui.VerticalBlock(GUI.skin.box, () =>
			{
				gui.BoldLabel("Args:");
				gui.Splitter();

				var args = methodEntry.argsEntries;
				var pTypes = paramTypes ?? methodEntry.Info
											.GetActualParams()
											.Select(p => p.ParameterType)
											.ToArray();

				for (int iLoop = 0; iLoop < args.Length; iLoop++)
				{
					int i = iLoop;
					var arg = args[i];
					var type = pTypes[i];

					gui.HorizontalBlock(() =>
					{
						gui.Label((i + 1) + "- (" + ReflectionHelper.TypeNameGauntlet(type) + "): ");

						gui.EnabledBlock(!arg.isUsingSource, () =>
							 DoArgDirect(arg, type));

						gui.Foldout(arg.isUsingSource, newValue =>
							arg.isUsingSource = newValue);
					});

					if (arg.isUsingSource)
					{
						DoArgFromSource(arg, type);
					}
				}
			});
		}
		private void DoArgDirect(ArgEntry arg, Type type)
		{
			var direct = arg.directValue;

			if (type == typeof(int))
			{
				gui.IntField(direct.intValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(float))
			{
				gui.FloatField(direct.floatValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(bool))
			{
				gui.Toggle(direct.boolValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(string))
			{
				gui.TextField(direct.stringValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(Vector3))
			{
				gui.Vector3Field(direct.vector3Value, newValue => direct.value = newValue);
			}
			else if (type == typeof(Vector2))
			{
				gui.Vector2Field(direct.vector2Value, newValue => direct.value = newValue);
			}
			else if (type == typeof(Bounds))
			{
				gui.BoundsField(direct.boundsValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(Rect))
			{
				gui.RectField(direct.rectValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(Color))
			{
				gui.ColorField(direct.colorValue, newValue => direct.value = newValue);
			}
			else if (type.IsEnum)
			{
				direct.SetEnums(type);
				gui.Popup(direct.enumIndex, direct.enumOptions, newIndex => direct.enumIndex = newIndex);
			}
			else if (typeof(Object).IsAssignableFrom(type))
			{
				gui.ObjectField(direct.objectRefValue, type, newValue => direct.value = newValue);
			}
			else if (type == typeof(Quaternion))
			{
				gui.Vector3Field(direct.quaternionValue.eulerAngles, direct.SetQuaternionFromVector3);
			}
			else
			{
				gui.ColorBlock(GuiHelper.RedColorDuo.FirstColor, () =>
					gui.TextFieldLabel("Can't set value directly. Select from a source instead ->"));
			}
		}
		private void DoArgFromSource(ArgEntry arg, Type type)
		{
			gui.HorizontalBlock(() =>
			{
				gui.Label("From: ", new TOption { Width = 40f });

				// Source GO field
				var go = arg.sourceGo;
				DoGOField(
					@currentValue: go,
					@enableField: true,
					@color: null,
					@setNewValue: newValue => arg.sourceGo = newValue
				);

				// Target popup
				var targets = go == null ? null : go.GetAllComponentsIncludingSelf();
				DoTargetsPopup(
					@targets: targets,
					@getCurrent: () => arg.sourceTarget,
					@useLabelField: false,
					@color: null,
					@setTarget: newValue => arg.sourceTarget = newValue
				);

				// Field name popup
				DoArgFieldPopup(arg, type, bindings);
			});
		}
		protected int DoTargetsPopup(Object[] targets, Func<Object> getCurrent, bool useLabelField, Color? color, Action<Object> setTarget)
		{
			int tIndex;
			string[] tNames;
			Object current = getCurrent();

			if (targets == null)
			{

				tIndex = -1;
				tNames = null;
			}
			else
			{
				tIndex = targets.IndexOf(current);
				tNames = GetTargetsNames(targets);
			}

			if (tIndex == -1)
			{
				Log("TargetsPopup: target not found - setting index to 0");
				tIndex = 0;
			}

			if (tNames.IsNullOrEmpty())
				tNames = new[] { NA };

			Action field;

			if (useLabelField)
			{
				field = () => gui.DraggableLabelField(current);
			}
			else field = () =>
					DoPopup(
						@currentIndex: tIndex,
						@names: tNames,
						@values: targets,
						@isNewValue: newValue => newValue != current,
						@setToNewValue: setTarget
					);

			gui.ColorBlock(color, field);

			gui.GetLastRect(lastRect =>
			{
				GuiHelper.PingField(lastRect, current, current != null, EventsHelper.MouseEvents.IsRMB_MouseDown(), MouseCursor.Link);
				GuiHelper.SelectField(lastRect, current, EventsHelper.MouseEvents.IsRMB_MouseDown() && EventsHelper.MouseEvents.IsDoubleClick());
			});

			return tIndex;
		}
		protected void DoGOField(GameObject currentValue, bool enableField, Color? color, Action<GameObject> setNewValue)
		{
			gui.EnabledBlock(enableField, () =>
				gui.ColorBlock(color, () =>
					gui.DraggableObjectField(currentValue, setNewValue)
				)
			);

			gui.GetLastRect(lastRect =>
				GuiHelper.AddCursorRect(lastRect, MouseCursor.Link)
			);
		}

		protected string[] GetTargetsNames(Object[] targets)
		{
			var names = targets.Select(c => c.GetType().Name).ToArray();
			int gIndex = names.IndexOf("GameObject");
			if (gIndex != -1)
			{
				names[gIndex] = names[gIndex].Insert(0, "[");
				names[gIndex] += "]";
			}

			// I had to do this since it seems that Popup doesn't show duplicate entries
			// So if we had two of the same Components on a GO, the popup would only show one of them
			var duplicatesIndicesLists = names.GetDuplicates().Select(d => d.Value);
			foreach (var dupList in duplicatesIndicesLists)
			{
				for (int i = 0; i < dupList.Length; i++)
				{
					names[dupList[i]] += " (" + (i + 1) + ")";
				}
			}
			return names;
		}
		private void DoArgFieldPopup(ArgEntry arg, Type returnType, bf flags)
		{
			var target = arg.sourceTarget;
			string[] membersNames;
			int mIndex;
			if (target == null || arg.sourceGo == null)
			{
				membersNames = null;
				mIndex = -1;
			}
			else
			{
				var extensionMethods = target.GetExtensionMethods(
					@asm: typeof(TypeExtensions).Assembly,
					@higherType: typeof(Object),
					@returnType: returnType,
					@paramTypes: Type.EmptyTypes,
					@modifiers: flags & bf.Public | flags & bf.NonPublic,
					@exactBinding: false
				);

				membersNames = target.GetFields(returnType, flags)
					.Concat(target.GetProperties(returnType, flags).Cast<MemberInfo>())
					.Concat(target.GetMethods(returnType, Type.EmptyTypes, flags, false)
											  .Where(m => !m.IsSpecialName)
											  .Cast<MemberInfo>())
					.Concat(extensionMethods.Cast<MemberInfo>())
					.Select(m =>
					{
						return m.MemberType == MemberTypes.Method ?
							(m as MethodInfo).GetFullName("[ext] ", ReflectionHelper.TypeNameGauntlet) :
							m.Name;
					})
					.ToArray();

				mIndex = membersNames.IndexOf(arg.memberName);
			}

			if (mIndex == -1)
			{
				Log("ArgMember: member not found - setting index to 0");
				mIndex = 0;
			}
			if (membersNames.IsNullOrEmpty())
				membersNames = new[] { NA };

			DoPopup(
				@currentIndex: mIndex,
				@names: membersNames,
				@values: membersNames,
				@isNewValue: newField => arg.memberName != newField,
				@setToNewValue: newField => arg.memberName = newField
			);
		}
		void Log(string msg)
		{
			if (DEBUG) Debug.Log(msg);
		}
		private void DoPopup<T>(int currentIndex, string[] names, T[] values, Predicate<T> isNewValue, Action<T> setToNewValue)
		{
			gui.Popup(currentIndex, names, newIndex =>
			{
				if (values.IsNullOrEmpty()) return;
				var newValue = values[newIndex];
				if (isNewValue(newValue))
				{
					setToNewValue(newValue);
				}
			});
		}
		#endregion
		/// <summary>
		/// An argument entry class that makes it possible to set method arguments via editor directly or from a source
		/// </summary>
		[Serializable]
		public class ArgEntry
		{
			public bool isUsingSource;

			[SerializeField]
			private DirectValue _directValue;
			[SerializeField]
			private SourceValue _sourceValue;

			public SourceValue sourceValue { get { return RTHelper.LazyValue(() => _sourceValue, s => _sourceValue = s); } }
			public DirectValue directValue { get { return RTHelper.LazyValue(() => _directValue, d => _directValue = d); } }
			public Type TypeUsed { get { return directValue.TypeUsed; } }
			public string memberName { get { return sourceValue.memberName; } set { sourceValue.memberName = value; } }
			public GameObject sourceGo { get { return sourceValue.sourceGo; } set { sourceValue.sourceGo = value; } }
			public Object sourceTarget { get { return sourceValue.sourceTarget; } set { sourceValue.sourceTarget = value; } }
			public object value { get { return isUsingSource ? sourceValue.value : directValue.value; } }

			public ArgEntry(bf bindings)
			{
				sourceValue.bindings = bindings;
			}

			[Serializable]
			public class SourceValue
			{
				public GameObject sourceGo { get { return _sourceGo; } set { _sourceGo = value; } }
				public Object sourceTarget
				{
					get { return _sourceTarget; }
					set
					{
						if (_sourceTarget != value)
						{
							_sourceTarget = value;
							if (value == null) return;
							var comp = value as Component;
							if (comp) _sourceGo = comp.gameObject;
							else _sourceGo = value as GameObject ?? GOHelper.EmptyGO;
						}
					}
				}

				[SerializeField]
				private GameObject _sourceGo;

				[SerializeField]
				private Object _sourceTarget;

				[SerializeField]
				private string _memberName;

				private FieldInfo finfo;
				private PropertyInfo pinfo;
				private MethodInfo minfo;

				public bf bindings { get; set; }

				public string memberName
				{
					get { return _memberName; }
					set
					{
						if (_memberName != value)
						{
							_memberName = value;
							pinfo = null;
							finfo = null;
							minfo = null;
						}
					}
				}

				public object value
				{
					get
					{
						if (string.IsNullOrEmpty(_memberName))
							throw new ArgumentNullException("ArgEntry: can't fetch value - field name is null or empty");

						if (sourceTarget == null)
							throw new ArgumentNullException("ArgEntry: can't fetch value - target is null");

						Type t = sourceTarget.GetType();
						var flags = bindings;

						if (finfo == null) finfo = t.GetField(_memberName, flags);
						if (finfo != null) return finfo.GetValue(sourceTarget);
						if (pinfo == null) pinfo = t.GetProperty(_memberName, flags);
						if (pinfo != null) return pinfo.GetValue(sourceTarget, null);
						if (minfo == null) minfo = t.GetMethod(Regex.Replace(_memberName, "[()]", ""), Type.EmptyTypes, flags);
						if (minfo != null) return minfo.Invoke(sourceTarget, null);
						return null;
					}
				}
			}

			[Serializable]
			public class DirectValue // Couldn't have been more ugly! -___-
			{
				[SerializeField]
				private SerializedType typeUsed = new SerializedType();

				public Type TypeUsed { get { return typeUsed.Value; } }

				public object value
				{
					get
					{
						var uType = typeUsed.Value;
						if (uType == typeof(int))
						{
							return _intValue;
						}
						if (uType == typeof(float))
						{
							return _floatValue;
						}
						if (uType == typeof(bool))
						{
							return _boolValue;
						}
						if (uType == typeof(string))
						{
							return _stringValue;
						}
						if (uType == typeof(Vector3))
						{
							return _vector3Value;
						}
						if (uType == typeof(Vector2))
						{
							return _vector2Value;
						}
						if (uType == typeof(Bounds))
						{
							return _boundsValue;
						}
						if (uType == typeof(Rect))
						{
							return _rectValue;
						}
						if (uType == typeof(Color))
						{
							return _colorValue;
						}
						if (typeof(Enum).IsAssignableFrom(uType))
						{
							return Enum.Parse(uType, _enumOptions[_enumIndex]);
						}
						if (typeof(Object).IsAssignableFrom(uType))
						{
							return _objectRefValue;
						}
						if (uType == typeof(Quaternion))
						{
							return _quaternionValue;
						}
						return null;
					}
					set
					{
						var type = value == null ? typeof(Object) : value.GetType();
						typeUsed.Value = type;

						if (type == typeof(int))
						{
							_intValue = (int)value;
						}
						else if (type == typeof(float))
						{
							_floatValue = (float)value;
						}
						else if (type == typeof(bool))
						{
							_boolValue = (bool)value;
						}
						else if (type == typeof(string))
						{
							_stringValue = (string)value;
						}
						else if (type == typeof(Vector3))
						{
							_vector3Value = (Vector3)value;
						}
						else if (type == typeof(Vector2))
						{
							_vector2Value = (Vector2)value;
						}
						else if (type == typeof(Bounds))
						{
							_boundsValue = (Bounds)value;
						}
						else if (type == typeof(Rect))
						{
							_rectValue = (Rect)value;
						}
						else if (type == typeof(Color))
						{
							_colorValue = (Color)value;
						}
						else if (type == typeof(Quaternion))
						{
							_quaternionValue = (Quaternion)value;
						}
						else if (type.IsEnum)
						{
							SetEnums(type);
							_enumIndex = 0;
						}
						else if (typeof(Object).IsAssignableFrom(type))
						{
							_objectRefValue = value as Object;
						}
						else throw new InvalidOperationException(string.Format("Can't set value - Type `{0}` is not supported", type));
					}
				}

				[SerializeField]
				private int _intValue;
				[SerializeField]
				private float _floatValue;
				[SerializeField]
				private bool _boolValue;
				[SerializeField]
				private string _stringValue = string.Empty;
				[SerializeField]
				private Vector3 _vector3Value;
				[SerializeField]
				private Vector2 _vector2Value;
				[SerializeField]
				private Object _objectRefValue;
				[SerializeField]
				private Bounds _boundsValue;
				[SerializeField]
				private Rect _rectValue;
				[SerializeField]
				private Color _colorValue;
				[SerializeField]
				private Quaternion _quaternionValue;
				[SerializeField]
				private int _enumIndex;
				[SerializeField]
				private string[] _enumOptions;

				public int intValue { get { return _intValue; } }
				public float floatValue { get { return _floatValue; } }
				public bool boolValue { get { return _boolValue; } }
				public string stringValue { get { return _stringValue; } }
				public Vector2 vector2Value { get { return _vector2Value; } }
				public Vector3 vector3Value { get { return _vector3Value; } }
				public Bounds boundsValue { get { return _boundsValue; } }
				public Object objectRefValue { get { return _objectRefValue; } }
				public Rect rectValue { get { return _rectValue; } }
				public Color colorValue { get { return _colorValue; } }
				public Quaternion quaternionValue { get { return _quaternionValue; } }
				public string[] enumOptions { get { return _enumOptions; } }
				public int enumIndex { get { return _enumIndex; } set { _enumIndex = value; } }

				public void SetQuaternionFromVector3(Vector3 v)
				{
					var newQuat = new Quaternion();
					newQuat.eulerAngles = v;
					_quaternionValue = newQuat;
				}

				public void SetEnums(Type type)
				{
					typeUsed.Value = type;
					_enumOptions = Enum.GetNames(type);
				}
			}
		}
		/// <summary>
		/// A container class holding a SerializedMethodInfo for a hooked method.
		/// Also contains a list of ArgEntry to be able to set arguments in the editor for each method entry
		/// </summary>
		[Serializable]
		public class MethodEntry
		{
			[SerializeField]
			private SerializedMethodInfo info;

			[SerializeField]
			private ArgEntry[] _argsEntries = new ArgEntry[0];

			[SerializeField]
			private bf bindings;

			public string Name { get { return Info == null ? string.Empty : Info.Name; } }
			public string FullName { get { return Info == null ? null : Info.GetFullName(); } }
			public MethodInfo Info { get { return info.Value; } set { info.Value = value; } }
			public ArgEntry[] argsEntries { get { return _argsEntries; } set { _argsEntries = value; } }
			public object[] ArgumentValues { get { return _argsEntries.Select(e => e.value).ToArray(); } }

			public MethodEntry()
			{
				info = new SerializedMethodInfo();
			}
			public MethodEntry(MethodInfo minfo)
			{
				info = new SerializedMethodInfo(minfo);
			}
			public MethodEntry(MethodInfo minfo, bf bindings)
				: this(minfo)
			{
				this.bindings = bindings;
			}

#if UNITY_EDITOR
			public bool argsToggle = true;
#endif

			public void ReinitArgs(int nArgs)
			{
				_argsEntries = new ArgEntry[nArgs];
				for (int i = 0; i < nArgs; i++)
				{
					_argsEntries[i] = new ArgEntry(bindings);
				}
			}

			/// <summary>
			/// Reinitializes the arg list using the length specified from the passed paramTypes
			/// If paramTypes was null, we re-init using the parameters length of our method info
			/// (Helps Kickass delegate to do what it does)
			/// </summary>
			public void ReinitArgs(Type[] paramTypes)
			{
				ReinitArgs(paramTypes == null ? Info.GetActualParams().Length : paramTypes.Length);
			}

			/// <summary>
			/// Checks the length of our arg list and the length of X*, if they're not equal, re-init the args.
			/// X: the length of paramTypes if it wasn't null, otherwise the length of our method info params.
			/// </summary>
			public void CheckArgs(Type[] paramTypes)
			{
				var _params = paramTypes ?? Info.GetActualParams()
												.Select(p => p.ParameterType).ToArray();
				int nArgs = _params.Length;
				if (_argsEntries == null ||
					_argsEntries.Length != nArgs)
				{
					ReinitArgs(nArgs);
					return;
				}
				for (int i = 0; i < nArgs; i++)
				{
					if (_argsEntries[i] == null)
						_argsEntries[i] = new ArgEntry(bindings);
				}
			}

			public static implicit operator MethodEntry(MethodInfo info)
			{
				return new MethodEntry(info);
			}
		}
	}
}