using Vexe.RuntimeExtensions;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using Vexe.RuntimeHelpers.Classes;

namespace uFAction
{
	/// <summary>
	/// A container class holding a SerializedMethodInfo for a hooked method.
	/// Also contains a list of ArgEntry to be able to set arguments in the editor for each method entry
	/// </summary>
	[Serializable]
	public class MethodEntry : UniqueEntry
	{
		[SerializeField]
		private SerializedMethodInfo info;

		[SerializeField]
		private ArgEntry[] _argsEntries = new ArgEntry[0];

		public string Name { get { return Info == null ? string.Empty : Info.Name; } }
		public string FullName { get { return Info == null ? null : Info.GetFullName(); } }
		public MethodInfo Info { get { return info.Value; } set { info.Value = value; } }
		public ArgEntry[] argsEntries { get { return _argsEntries; } set { _argsEntries = value; } }

		public MethodEntry()
		{
			info = new SerializedMethodInfo();
		}
		public MethodEntry(MethodInfo minfo)
		{
			info = new SerializedMethodInfo(minfo);
		}

		public void ReinitArgs(int nArgs)
		{
			_argsEntries = new ArgEntry[nArgs];
			for (int i = 0; i < nArgs; i++)
			{
				_argsEntries[i] = new ArgEntry();
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
			var _params = paramTypes ?? Info.GetActualParams().Select(p => p.ParameterType).ToArray();
			int nArgs = _params.Length;
			if (_argsEntries == null ||
				_argsEntries.Length != nArgs)// ||
			//!_params.IsEqualTo(_argsEntries.Select(e => e.TypeUsed).ToArray())) {
			{
				ReinitArgs(nArgs);
				return;
			}
			for (int i = 0; i < nArgs; i++)
			{
				if (_argsEntries[i] == null)
					_argsEntries[i] = new ArgEntry();
			}
		}

		public static implicit operator MethodEntry(MethodInfo info)
		{
			return new MethodEntry(info);
		}
	}
}