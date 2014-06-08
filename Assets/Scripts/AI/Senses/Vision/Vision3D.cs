using UnityEngine;

public class Vision3D : Vision
{
	protected override void CastSingleRay(Vector3 dir)
	{
		RaycastHit hit;
		if (!Physics.Raycast(cachedTransform.position, dir, out hit, fovMaxDistance, layerMask))
		{
			renderLocations.Add(cachedTransform.position + (dir * fovMaxDistance));
		}
		else
		{
			var go = hit.collider.gameObject;
			renderLocations.Add(hit.point);
			onSeen(go, hits);
			hits.Add(go);
		}
	}
}