using UnityEngine;
using Vexe.RuntimeExtensions;
using Vexe.RuntimeHelpers.Classes;
using System;

namespace ShowEmAll
{
	/// <summary>
	/// Inherit from this instead of MonoBehaviour, to get all the cool stuff
	/// </summary>
	[DefineCategory("Debug", @displayOrder: 3)]
	public abstract class BetterBehaviour : MonoBehaviour, IUniquelyIdentifiedObject
	{
		[SerializeField, HideInInspector]
		private string id;

		protected const string DBug = "Debug";

		public string ID
		{
			get
			{
				if (id.IsNullOrEmpty())
					id = Guid.NewGuid().ToString();
				return id;
			}
		}

		public void log(object msg)
		{
			Debug.Log(msg);
		}
	}
}