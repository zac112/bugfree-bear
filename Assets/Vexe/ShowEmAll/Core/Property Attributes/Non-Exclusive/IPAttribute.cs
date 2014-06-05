using UnityEngine;
using System.Collections;

namespace ShowEmAll
{
	public class IPAttribute : RegexAttribute
	{
		public IPAttribute()
			: base(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", "Invalid IP address!\nExample: '127.0.0.1'")
		{

		}
	}
}