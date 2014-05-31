using UnityEngine;
using Vexe.RuntimeExtensions;
using Vexe.RuntimeHelpers.Classes;
using System;

namespace ShowEmAll
{
	/// <summary>
	/// Inherit from this instead of MonoBehaviour, to get all the cool stuff
	/// </summary>
	public abstract class BetterBehaviour : MonoBehaviour, IUniquelyIdentifiedObject
	{
		[SerializeField, HideInInspector]
		private string id;

		public string ID
		{
			get
			{
				if (id.IsNullOrEmpty())
					id = Guid.NewGuid().ToString();
				return id;
			}
		}
	}
}