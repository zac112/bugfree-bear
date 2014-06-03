using UnityEngine;
using System.Linq;
using ShowEmAll;
using Vexe.RuntimeHelpers;
using Vexe.RuntimeExtensions;

public class EnemySample : BetterBehaviour
{
	public Color alert = Color.red;
	public Color idle = Color.green;
	public Color suspicious = Color.yellow;

	[SerializeField]
	private Vision eyes;

	[SerializeField]
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
		AssertionHelper.AssertNotNullAfterAssignment(ref eyes, GetComponentInChildren<Vision>, "eyes");
		AssertionHelper.AssertNotNullAfterAssignment(ref touch, GetComponentInChildren<Touch>, "touch");
	}

	private void Start()
	{
		eyes.AddVisionSubscriber((current, all) =>
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
		});

		touch.OnTouch.Add(OnTriggerEnter);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == Tags.player)
			Debug.Log("Touching player");
	}
}