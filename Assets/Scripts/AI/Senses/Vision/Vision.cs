using UnityEngine;
using System.Collections.Generic;
using System;
using ShowEmAll;

public abstract class Vision : Sense
{
	[Min(0f)] public float updateRate = 0.02f;
	[Min(0f)] public float quality = 1f;
	[Min(1f)] public float fovMaxDistance = 15;
	[NumericClamp(1f, 360f)] public float fovAngle = 90f;
	public LayerMask layerMask = -1;

	[SerializeField, RequiredFromThis(true)]
	protected MeshRenderer fovMeshRenderer;

	[SerializeField, RequiredFromThis(true)]
	protected MeshFilter fovMeshFilter;

	protected List<GameObject> hits = new List<GameObject>();
	protected List<Vector3> renderLocations = new List<Vector3>();
	protected Action<GameObject, List<GameObject>> onSeen = delegate { };
	protected Transform cachedTransform;

	private float timer;
	private FOVRenderer fov;
	private bool clearedFov;

	public Action<GameObject, List<GameObject>> OnSeen
	{
		get { return onSeen; }
		set { onSeen = value; }
	}

	protected override void Initialize()
	{
		cachedTransform = transform;
	}

	[ShowMethod]
	public void SetFOVColor(Color to)
	{
		fovMeshRenderer.material.color = to;
	}

	protected override void UpdateSense()
	{
		timer += Time.deltaTime;
		if (timer > updateRate)
		{
			timer = 0;
			CastRays();
		}
	}

	private void LateUpdate()
	{
		if (renderFov)
		{
			if (fov == null)
				fov = new FOVRenderer(cachedTransform, fovMeshFilter.mesh);
			fov.RenderLocations = renderLocations;
			fov.Update();
			clearedFov = false;
		}
		else if (!clearedFov)
		{
			clearedFov = true;
			fovMeshFilter.mesh.Clear();
		}
	}

	private void CastRays()
	{
		int numRays = Mathf.CeilToInt(fovAngle * quality);
		float currentAngle = fovAngle / -2;

		hits.Clear();
		renderLocations.Clear();

		for (int i = 0; i < numRays; i++)
		{
			Vector3 dir = Quaternion.Euler(0, 0, -currentAngle) * cachedTransform.up;
			CastSingleRay(dir);
			currentAngle += 1f / quality;
		}
	}

	protected abstract void CastSingleRay(Vector3 dir);

	private void OnDrawGizmos()
	{
		if (drawGizmos)
		{
			Gizmos.color = gizmosColor;
			foreach (Vector3 point in renderLocations)
			{
				Gizmos.DrawLine(transform.position, point);
			}
		}
	}

	public class FOVRenderer
	{
		private Transform transform;
		private Mesh mesh;

		private Vector3[] vertices;
		private int[] triangles;

		public List<Vector3> RenderLocations { private get; set; }

		public FOVRenderer(Transform transform, Mesh mesh)
		{
			this.transform = transform;
			this.mesh = mesh;
		}

		public void Update()
		{
			int nHits = RenderLocations.Count;

			if (nHits == 0)
				return;

			if (mesh.vertices.Length != nHits + 1 || vertices == null)
			{
				mesh.Clear();
				vertices = new Vector3[nHits + 1];
				triangles = new int[(nHits) * 3];

				for (int i = 0, v = 0; i < triangles.Length; i += 3)
				{
					triangles[i] = 0;
					triangles[i + 1] = v;
					triangles[i + 2] = v + 1;
					v++;
				}
			}

			vertices[0] = Vector3.zero;
			for (int i = 0; i < nHits; i++)
			{
				vertices[i + 1] = transform.InverseTransformPoint(RenderLocations[i]);
			}

			Vector2[] uvs = new Vector2[vertices.Length];
			for (int i = 0; i < uvs.Length; i++)
			{
				uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
			}

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;

			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}
	}

	[CategoryMember(DBug)]
	public bool drawGizmos = true;
	[CategoryMember(DBug)]
	public bool renderFov = true;
	[CategoryMember(DBug)]
	public Color gizmosColor;
}