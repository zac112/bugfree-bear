using UnityEngine;
using System;
using Vexe.RuntimeHelpers;
using Vexe.RuntimeHelpers.Classes;

namespace uFAction
{
	[Serializable]
	public abstract class UniqueEntry : IUniquelyIdentifiedObject
	{
		[SerializeField]
		protected string id;
		public string ID
		{
			get
			{
				return RTHelper.LazyValue(
					() => id,
					() => string.IsNullOrEmpty(id),
					newId => id = newId,
					() => Guid.NewGuid().ToString());
			}
		}
	}
}