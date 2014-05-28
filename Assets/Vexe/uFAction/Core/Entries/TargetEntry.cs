using Vexe.RuntimeExtensions;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Reflection;
using Vexe.RuntimeHelpers;

namespace uFAction
{
	/// <summary>
	/// A container class for a target Object to hook up methods from, and a list of MethodEntry
	/// </summary>
	[Serializable]
	public class TargetEntry : UniqueEntry
	{
		[SerializeField]
		protected Object _target;

		[SerializeField]
		protected List<MethodEntry> _methods = new List<MethodEntry>();

		public Object Target { get { return _target; } set { _target = value; } }
		public List<MethodEntry> MethodEntries { get { return _methods; } set { _methods = value; } }
		public MethodInfo[] MethodInfos { get { return _methods.Select(m => m.Info).ToArray(); } }
		public Component ComponentTarget { get { return GetTypedTarget<Component>(); } }
		public GameObject gameObject
		{
			get
			{
				if (_target == null) return null;
				var comp = ComponentTarget;
				if (comp != null) return comp.gameObject;
				return _target as GameObject ?? GOHelper.EmptyGO;
			}
		}

		public TargetEntry() { }
		public TargetEntry(Object target)
		{
			_target = target;
		}
		public TargetEntry(Object target, MethodEntry method)
			: this(target)
		{
			_methods.Add(method);
		}
		public TargetEntry(Object target, MethodInfo minfo) :
			this(target, new MethodEntry(minfo)) { }

		public T GetTypedTarget<T>() where T : Object
		{
			return Target as T;
		}

		public void Invoke()
		{
			foreach (var method in MethodEntries) {
				var info = method.Info;
				if (info == null) continue;
				object[] args;
				if (info.GetParameters().IsEmpty()) {
					args = null;
				}
				else {
					var values = method.argsEntries.Select(arg => arg.value).ToList();
					if (info.IsExtensionMethod()) {
						values.Insert(0, _target);
					}
					args = values.ToArray();
				}
				info.Invoke(_target, args);
			}
		}
	}
}