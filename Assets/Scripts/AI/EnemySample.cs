using UnityEngine;
using System.Linq;
using ShowEmAll;
using System.Collections.Generic;

public class EnemySample : BetterBehaviour
{
	[SerializeField, RequireFromChildren("Senses/Vision")]
	private Vision eyes;

	[SerializeField, RequireFromChildren("Senses/Hearing")]
	private Hearing ears;

	[SerializeField, RequireFromChildren("Senses/Touch")]
	private Touch touch;

	private bool mPlayerInSight;

	private bool playerInSight
	{
		get { return mPlayerInSight; }
		set
		{
			mPlayerInSight = value;
			eyes.SetFOVColor(value ? alert : idle);
		}
	}

	private void OnEnable()
	{
		// Sub to eyes
		eyes.OnSeen += OnSeen;

		// Sub to touch
		touch.OnTouch.Add(OnTouch);

		// Sub to ears
		ears.OnHeard += OnHeard;
	}

	private void OnDisable()
	{
		eyes.OnSeen -= OnSeen;
		touch.OnTouch.Remove(OnTouch);
		ears.OnHeard -= OnHeard;
	}

	private void OnSeen(RaycastHit current, List<RaycastHit> all)
	{
		// TODO: Find a better way than using tags
		if (current.collider.tag == Tags.player)
		{
			playerInSight = true;
		}
		else if (playerInSight)
		{
			playerInSight = all.Select(hit => hit.collider.tag).Contains("Player");
		}
	}

	private void OnHeard(ISoundEmitter source, GameObject noiseMaker)
	{
		if (noiseMaker.tag == Tags.player)
			Debug.Log("Heard player moving");
	}

	public void OnTouch(Collider other)
	{
		if (other.tag == Tags.player)
			Debug.Log("Touching player");
	}

	[CategoryMember(DBug)]
	public Color
		alert = Color.red,
		idle = Color.green,
		suspicious = Color.yellow;
}