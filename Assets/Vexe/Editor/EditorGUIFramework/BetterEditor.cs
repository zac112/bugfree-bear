using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using Vexe.RuntimeExtensions;
using Vexe.RuntimeHelpers.Exceptions;
using Object = UnityEngine.Object;

namespace EditorGUIFramework
{
	public class BetterEditor<T> : Editor where T : Object
	{
		protected GLWrapper gui = new GLWrapper();
		public T TypedTarget { get { return target as T; } }

		public FieldInfo GetFieldInfo(string name)
		{
			return target.GetType().GetFields(BindingFlags.Instance| BindingFlags.Public | BindingFlags.NonPublic)
								   .FirstOrDefault(f => f.Name == name && f.IsPublic ||
												   f.IsDefined(typeof(SerializeField)));
		}

		public object GetFieldValue(string name)
		{
			var field = GetFieldInfo(name);
			if (field == null)
				throw new MemberNotFoundException(name);
			return field.GetValue(target);
		}

		public TValueType GetFieldValue<TValueType>(string name)
		{
			return (TValueType)GetFieldValue(name);
		}
	}
}