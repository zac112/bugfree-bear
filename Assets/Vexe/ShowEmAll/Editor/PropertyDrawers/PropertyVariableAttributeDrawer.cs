using UnityEngine;
using UnityEditor;
using System.Reflection;
using Vexe.RuntimeExtensions;
using System.Linq;
using bf = System.Reflection.BindingFlags;
using System;
using Object = UnityEngine.Object;
using Vexe.EditorHelpers;
using EditorGUIFramework;

namespace ShowEmAll.PropertyDrawers
{
	// Not finished
	[CustomPropertyDrawer(typeof(PropertyVariableAttribute))]
	public class PropertyVariableAttributeDrawer : BetterPropertyDrawer<PropertyVariableAttribute>
	{
		private PropertyInfo pinfo;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			string fieldName = fieldInfo.Name;
			pinfo = fieldInfo.DeclaringType
					.GetProperties(fieldInfo.FieldType, bf.Instance | bf.Public)
					.FirstOrDefault(p => p.Name == fieldName.ToUpperAt(0));

			if (pinfo == null)
				throw new Vexe.RuntimeHelpers.Exceptions.MemberNotFoundException("Couldn't find a property for field " + fieldName + ". Are you sure there's a property? it must have the same return type, public and follow the setup naming convension");

			base.Init(property, label);
		}

		protected override void Code()
		{

		}


		//private void DoArgDirect(Property property, Type type)
		//{
		//	if (type == typeof(int))
		//	{
		//		gui.IntField(property.intValue, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(float))
		//	{
		//		gui.FloatField(property.floatValue, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(bool))
		//	{
		//		gui.Toggle(property.boolValue, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(string))
		//	{
		//		gui.TextField(property.stringValue, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(Vector3))
		//	{
		//		gui.Vector3Field(property.vector3Value, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(Vector2))
		//	{
		//		gui.Vector2Field(property.vector2Value, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(Bounds))
		//	{
		//		gui.BoundsField(property.boundsValue, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(Rect))
		//	{
		//		gui.RectField(property.rectValue, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(Color))
		//	{
		//		gui.ColorField(property.colorValue, newValue => property.value = newValue);
		//	}
		//	else if (type.IsEnum)
		//	{
		//		property.SetEnums(type);
		//		gui.Popup(property.enumIndex, property.enumOptions, newIndex => property.enumIndex = newIndex);
		//	}
		//	else if (typeof(Object).IsAssignableFrom(type))
		//	{
		//		gui.ObjectField(property.objectRefValue, type, newValue => property.value = newValue);
		//	}
		//	else if (type == typeof(Quaternion))
		//	{
		//		gui.Vector3Field(property.quaternionValue.eulerAngles, property.SetQuaternionFromVector3);
		//	}
		//	else
		//	{
		//		gui.ColorBlock(GuiHelper.RedColorDuo.FirstColor, () =>
		//			gui.TextFieldLabel("Can't set value directly. Select from a source instead ->"));
		//	}
		//}

	}

	//[Serializable]
	//public class Property
	//{
	//	//[SerializeField]
	//	//private SerializedType typeUsed = new SerializedType();

	//	//public Type TypeUsed { get { return typeUsed.Get(); } }
	//	public Property(PropertyInfo pinfo, object target)
	//	{
	//		this.pinfo = pinfo;
	//		this.target = target;
	//	}

	//	public object value
	//	{
	//		get
	//		{
	//			var uType = typeUsed.Get();
	//			if (uType == typeof(int))
	//			{
	//				return _intValue;
	//			}
	//			if (uType == typeof(float))
	//			{
	//				return _floatValue;
	//			}
	//			if (uType == typeof(bool))
	//			{
	//				return _boolValue;
	//			}
	//			if (uType == typeof(string))
	//			{
	//				return _stringValue;
	//			}
	//			if (uType == typeof(Vector3))
	//			{
	//				return _vector3Value;
	//			}
	//			if (uType == typeof(Vector2))
	//			{
	//				return _vector2Value;
	//			}
	//			if (uType == typeof(Bounds))
	//			{
	//				return _boundsValue;
	//			}
	//			if (uType == typeof(Rect))
	//			{
	//				return _rectValue;
	//			}
	//			if (uType == typeof(Color))
	//			{
	//				return _colorValue;
	//			}
	//			if (typeof(Enum).IsAssignableFrom(uType))
	//			{
	//				return Enum.Parse(uType, _enumOptions[_enumIndex]);
	//			}
	//			if (uType == typeof(Object))
	//			{
	//				return _objectRefValue;
	//			}
	//			if (uType == typeof(Quaternion))
	//			{
	//				return _quaternionValue;
	//			}
	//			return null;
	//		}
	//		set
	//		{
	//			var type = value == null ? typeof(Object) : value.GetType();
	//			typeUsed.Set(type);

	//			if (type == typeof(int))
	//			{
	//				_intValue = (int)value;
	//			}
	//			else if (type == typeof(float))
	//			{
	//				_floatValue = (float)value;
	//			}
	//			else if (type == typeof(bool))
	//			{
	//				_boolValue = (bool)value;
	//			}
	//			else if (type == typeof(string))
	//			{
	//				_stringValue = (string)value;
	//			}
	//			else if (type == typeof(Vector3))
	//			{
	//				_vector3Value = (Vector3)value;
	//			}
	//			else if (type == typeof(Vector2))
	//			{
	//				_vector2Value = (Vector2)value;
	//			}
	//			else if (type == typeof(Bounds))
	//			{
	//				_boundsValue = (Bounds)value;
	//			}
	//			else if (type == typeof(Rect))
	//			{
	//				_rectValue = (Rect)value;
	//			}
	//			else if (type == typeof(Color))
	//			{
	//				_colorValue = (Color)value;
	//			}
	//			else if (type == typeof(Quaternion))
	//			{
	//				_quaternionValue = (Quaternion)value;
	//			}
	//			else if (type.IsEnum)
	//			{
	//				SetEnums(type);
	//				_enumIndex = 0;
	//			}
	//			else if (typeof(Object).IsAssignableFrom(type))
	//			{
	//				_objectRefValue = value as Object;
	//			}
	//			else throw new InvalidOperationException(string.Format("Can't set value - Type `{0}` is not supported", type));
	//		}
	//	}

	//	//[SerializeField]
	//	private int _enumIndex;
	//	//[SerializeField]
	//	private string[] _enumOptions;
	//	private readonly PropertyInfo pinfo;
	//	private readonly object target;

	//	public int intValue { get { return (int)pinfo.GetValue(target, null); } }
	//	public float floatValue { get { return (float)pinfo.GetValue(target, null); } }
	//	public bool boolValue { get { return (bool)pinfo.GetValue(target, null); } }
	//	public string stringValue { get { return (string)pinfo.GetValue(target, null); } }
	//	public Vector2 vector2Value { get { return (Vector2)pinfo.GetValue(target, null); } }
	//	public Vector3 vector3Value { get { return (Vector3)pinfo.GetValue(target, null); } }
	//	public Bounds boundsValue { get { return (Bounds)pinfo.GetValue(target, null); } }
	//	public Object objectRefValue { get { return pinfo.GetValue(target, null) as Object; } }
	//	public Rect rectValue { get { return (Rect)pinfo.GetValue(target, null); } }
	//	public Color colorValue { get { return (Color)pinfo.GetValue(target, null); } }
	//	public Quaternion quaternionValue { get { return (Quaternion)pinfo.GetValue(target, null); } }
	//	//public string[] enumOptions { get { return _enumOptions; } }
	//	//public int enumIndex { get { return _enumIndex; } set { _enumIndex = value; } }

	//	public void SetQuaternionFromVector3(Vector3 v)
	//	{
	//		var newQuat = new Quaternion();
	//		newQuat.eulerAngles = v;
	//		_quaternionValue = newQuat;
	//	}

	//	public void SetEnums(Type type)
	//	{
	//		typeUsed.Set(type);
	//		_enumOptions = Enum.GetNames(type);
	//	}
	//}
}
