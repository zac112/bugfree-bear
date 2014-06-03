using UnityEngine;
using System.Collections.Generic;
using System;
using Vexe.RuntimeHelpers;
using ShowEmAll;

public class Vision : Sense
{
	public bool drawGizmos = true;
	public Color gizmosColor;
	public bool renderFov = true;
	public float updateRate = 0.02f;
	public float quality = 1f;
	public float fovAngle = 90f;
	public float fovMaxDistance = 15;
	public LayerMask cullingMask;
	public MeshRenderer fovMeshRenderer;
	public MeshFilter fovMeshFilter;

	private List<RaycastHit> hits = new List<RaycastHit>();
	private List<Vector3> renderLocations = new List<Vector3>();
	private float timer;
	private Action<RaycastHit, List<RaycastHit>> onHit = delegate { };
	private FOVRenderer fov;
	private bool clearedFov;
	private Transform cachedTransform;

	protected override void Initialize()
	{
		cachedTransform = transform;
	}

	private void OnEnable()
	{
		AssertionHelper.AssertNotNullAfterAssignment(ref fovMeshFilter, GetComponent<MeshFilter>, "fovMeshFilter");
		AssertionHelper.AssertNotNullAfterAssignment(ref fovMeshRenderer, GetComponent<MeshRenderer>, "fovMeshRenderer");
	}

	public void AddVisionSubscriber(Action<RaycastHit, List<RaycastHit>> action)
	{
		onHit += action;
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
			fov.RenderLocations = renderLocations.ToArray();
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
			Vector3 dir = Quaternion.AngleAxis(currentAngle, -cachedTransform.forward) * cachedTransform.up;
			RaycastHit hit;

			if (!Physics.Raycast(cachedTransform.position, dir, out hit, fovMaxDistance, cullingMask))
			{
				renderLocations.Add(cachedTransform.position + (dir * fovMaxDistance));
			}
			else
			{
				renderLocations.Add(hit.point);
				onHit(hit, hits);
				hits.Add(hit);
			}

			currentAngle += 1f / quality;
		}
	}

	private void OnDrawGizmosSelected()
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

		public Vector3[] RenderLocations { get; set; }

		public FOVRenderer(Transform transform, Mesh mesh)
		{
			this.transform = transform;
			this.mesh = mesh;
		}

		public void Update()
		{
			int nHits = RenderLocations.Length;

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
}