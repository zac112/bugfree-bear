using UnityEngine;
using ShowEmAll;
using System;
using Vexe.RuntimeHelpers.Helpers;
using Vexe.RuntimeExtensions;

public class Hearing : Sense, ISoundReciever
{
	[SerializeField, HideInInspector]
	private float minHearingDistance = 1f;

	[SerializeField, HideInInspector]
	private float maxHearingDistance = 5f;

	[SerializeField, HideInInspector]
	private float hearingSensitivity = 1f;

	[SerializeField, HideInInspector]
	private float soundThreshold = 1f;

	[SerializeField, RequiredFromThis(@addIfNotExist: true)]
	private SphereCollider sphereCollider;

	private Action<ISoundEmitter, GameObject> onHeard = delegate { };

	/// <summary>
	/// Subscribe to get notified of objects being heard
	/// </summary>
	public Action<ISoundEmitter, GameObject> OnHeard // might use [un]Subscribtion methods instead
	{
		get { return onHeard; }
		set { onHeard = value; }
	}

	/// <summary>
	/// Anything within this distance is heard no matter how weak the sound strength is
	/// </summary>
	[ShowProperty]
	public float MinHearingDistance
	{
		get { return minHearingDistance; }
		set { minHearingDistance = value; }
	}

	/// <summary>
	/// The maximum distance we start hearing from - Can't hear anything further
	/// </summary>
	[ShowProperty]
	public float MaxHearingDistance
	{
		get { return maxHearingDistance; }
		set
		{
			maxHearingDistance = value;
			sphereCollider.radius = value;
		}
	}

	/// <summary>
	/// The higher, the better our hearing quality becomes
	/// </summary>
	[ShowProperty]
	public float HearingSensitivity
	{
		get { return hearingSensitivity; }
		set { hearingSensitivity = value; }
	}

	/// <summary>
	/// The minimum sound strength we could hear
	/// </summary>
	[ShowProperty]
	public float SoundThreshold
	{
		get { return soundThreshold; }
		set { soundThreshold = value; }
	}

	// debug vars
	private float dbgPercentage;
	private Vector3 dbgSenderPos;
	public void Recieve(ISoundEmitter emitter, GameObject sender)
	{
		Debug.Log("Recieved sound from: " + sender.name + " with strength: " + emitter.Strength);
		Vector3 senderPos = sender.transform.position;
		dbgSenderPos = senderPos;
		float sqrDistance = senderPos.SqrDistanceToV3(transform.position);
		float sqrMax = MaxHearingDistance * MaxHearingDistance;

		// decrease sound strength based on distance. the further, the weaker the sound even though the sender could be within maxHearingDistance
		float percentage = Mathf.Min(1f, 1f - (sqrDistance / sqrMax));
		dbgPercentage = percentage;
		float strength = percentage * emitter.Strength * HearingSensitivity;
		Debug.Log("Strength after: " + strength);
		if (strength >= soundThreshold)
		{
			var temp = GameObject.Instantiate(spottedPrefab, senderPos, Quaternion.identity);
			Destroy(temp, 1.5f);
			if (onHeard != null)
			{
				onHeard(emitter, sender);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (drawGizmos)
		{
			GizHelper.DrawWireSphere(transform.position, MaxHearingDistance, Color.green);
			GizHelper.DrawWireSphere(transform.position, MinHearingDistance, Color.red);
			GizHelper.DrawLine(dbgSenderPos, transform.position, new Color(0, 0, 0, dbgPercentage));
		}
	}

	[CategoryMember(DBug)]
	public bool drawGizmos;

	[CategoryMember(DBug)]
	public Color gizmoColor = Color.green;

	[CategoryMember(DBug)]
	public GameObject spottedPrefab;
}