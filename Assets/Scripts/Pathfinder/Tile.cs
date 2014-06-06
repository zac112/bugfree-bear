using UnityEngine;
using System;
using ShowEmAll;

public class Tile : BetterBehaviour
{
	public bool isWalkable;

	private void Start()
	{
		Nav.map.Register(transform, isWalkable);
	}

	private SpriteRenderer spriteRenderer
	{
		get
		{
			var r = GetComponent<SpriteRenderer>();
			if (r == null)
				throw new NullReferenceException("No SpriteRenderer attached to tile `" + name + "`");
			return r;
		}
	}

	[ShowProperty]
	public Color color
	{
		get { return spriteRenderer.color; }
		set { spriteRenderer.color = value; }
	}
}