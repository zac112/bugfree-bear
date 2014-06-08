using UnityEngine;
using System;

public class Vision2D : Vision
{
	protected override void CastSingleRay(Vector3 dir)
	{
		RaycastHit2D hit = Physics2D.Raycast(cachedTransform.position, dir, fovMaxDistance, layerMask);
		Action addRenderLocation = () => renderLocations.Add(cachedTransform.position + (dir * fovMaxDistance));
		if (hit.collider == null)
		{
			addRenderLocation();
		}
		else
		{
			var tile = hit.collider.GetComponent<Tile>();
			if (tile != null && !tile.isWalkable)
			{
				var hitGo = hit.collider.gameObject;
				renderLocations.Add(hit.point);
				onSeen(hitGo, hits);
				hits.Add(hitGo);
			}
			else
			{
				addRenderLocation();
			}
		}
	}
}