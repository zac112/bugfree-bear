using Vexe.RuntimeExtensions;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Linq;
using Vexe.RuntimeHelpers;
using Vexe.RuntimeHelpers.Classes;

namespace uFAction
{
	/// <summary>
	/// A container class for a gameObject to hook up targets from, and a list of ComponentEntries
	/// </summary>
	[Serializable]
	public class GOEntry : UniqueEntry
	{
		[SerializeField]
		private GameObject _go;

		[SerializeField]
		private List<TargetEntry> _targetEntries;

		public List<TargetEntry> TargetEntries { get { return _targetEntries; } set { _targetEntries = value; } }
		public GameObject go { get { return _go; } set { _go = value; } }
		public Object[] Targets
		{
			get
			{
				return IsNa ? _targetEntries.Select(e => e.Target).ToArray() : go.GetAllComponentsIncludingSelf();
			}
		}
		public bool IsNa { get { return _go == GOHelper.EmptyGO; } }

		public GOEntry()
		{
			_targetEntries = new List<TargetEntry>();
		}

		public GOEntry(GameObject go)
			: this()
		{ _go = go; }

		public GOEntry(GameObject go, string id) : this(go)
		{
			this.id = id;
		}

		public GOEntry(TargetEntry target)
			: this(target.gameObject)
		{ _targetEntries.Add(target); }

		public GOEntry(TargetEntry target, string id) : this(target)
		{
			this.id = id;
		}

		public GOEntry(Object target, MethodInfo minfo) :
			this(new TargetEntry(target, minfo))
		{ }
	}
}