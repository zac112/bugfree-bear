﻿using UnityEngine;
using Vexe.RuntimeExtensions;
using System;

namespace ShowEmAll
{
	/// <summary>
	/// Inherit from this instead of MonoBehaviour, to get all the cool stuff
	/// </summary>
	public abstract class StereoBehaviour : MonoBehaviour, IUniquelyIdentifiedObject
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

	public interface IUniquelyIdentifiedObject
	{
		string ID { get; }
	}
}