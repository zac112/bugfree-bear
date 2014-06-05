using UnityEngine;
using ShowEmAll;
using Vexe.RuntimeHelpers.Helpers;
using Vexe.RuntimeExtensions;

/// <summary>
/// The sound data algo is creited to aldonatelo http://answers.unity3d.com/questions/165729/editing-height-relative-to-audio-levels.html
/// so I'm not exactly sure what's rms, refValue, etc.
/// I left them as readonly fields just to see how their values change.
/// </summary>
public class SoundEmitter : BetterBehaviour, ISoundEmitter
{
	public bool drawGizmos = true;
	public Color gizmosColor;

	[SerializeField, RequireFromThis(true)]
	private AudioSource source;

	public int numSamples = 1024;
	public float refValue = 0.1f;

	[SerializeField, Readonly] // I pretty much don't care about both these fields being serialized but just want them visible as readonly in the inspector. I might try some attributes hack
	private float rmsValue;

	[SerializeField, Readonly]
	private float rmsPeak;

	[SerializeField, Readonly]
	private float dbValue;

	[SerializeField, HideInInspector]
	private float multiplier = 1f;

	[SerializeField]
	private LayerMask layerMask = -1;

	private float[] samples;
	private Collider[] hits;

	/// <summary>
	/// The higher the further the sound travels (the more stronger it gets)
	/// </summary>
	[ShowProperty]
	public float Multiplier { get { return multiplier; } set { multiplier = value; } }

	/// <summary>
	/// The audio source volume (0 -> 1)
	/// </summary>
	[ShowProperty]
	public float Volume { get { return source.volume; } set { source.volume = value; } }

	/// <summary>
	/// The sound's strength. Linearly depends on the Volume, Multiplier and the sound rms
	/// </summary>
	public float Strength { get { return Volume * rmsValue * Multiplier; } }

	/// <summary>
	/// Whether or not we're emitting sound (the audio source is playing)
	/// </summary>
	public bool IsEmitting { get { return source.isPlaying; } }

	private float maxStrength { get { return Volume * rmsPeak * Multiplier; } }

	private void Start()
	{
		samples = new float[numSamples];

		// TODO: Find a better way to ignore unwanted collisions
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	// These should really be implemented as buttons...
	[ShowMethod]
	public void ForceEmit()
	{
		source.Play();
	}

	[ShowMethod]
	public void Emit()
	{
		if (!IsEmitting)
			source.Play();
	}

	[ShowMethod]
	public void Stop()
	{
		source.Stop();
	}

	[ShowMethod]
	public void Pause()
	{
		source.Pause();
	}

	[ShowMethod]
	public void SetClip(AudioClip clip)
	{
		source.clip = clip;
		ResetRMSPeak();
	}

	[ShowMethod]
	private void ResetRMSPeak()
	{
		rmsPeak = 0f;
	}

	private void Update()
	{
		if (IsEmitting)
		{
			UpdateSoundData();
			hits = Physics.OverlapSphere(transform.position, Strength, layerMask);

			foreach (var h in hits)
			{
				Debug.Log("Hit: " + h.name);
				var reciever = h.gameObject.GetInterface<ISoundReciever>();
				if (reciever != null)
					reciever.Recieve(this, gameObject);
			}
		}
	}

	private void UpdateSoundData()
	{
		source.GetOutputData(samples, 0); // fill array with samples
		int i;
		float sum = 0;
		for (i = 0; i < numSamples; i++)
		{
			sum += samples[i] * samples[i]; // sum squared samples
		}
		rmsValue = Mathf.Sqrt(sum / numSamples); // rms = square root of average
		rmsPeak = Mathf.Max(rmsPeak, rmsValue);
		dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
		if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
	}

	private void OnDrawGizmos()
	{
		if (drawGizmos && source != null)
		{
			var from = transform.position;
			if (IsEmitting)
			{
				GizHelper.DrawWireSphere(from, Strength, Color.red);
				foreach (var h in hits)
				{
					Vector3 to;
					if (h.isTrigger) // TODO: Find a better way
					{
						to = h.transform.position;
					}
					else
					{
						// For some reason this returns a value close to our transform position if the collider was a trigger
						// If that's the case, we draw a line from our position to the collider object itself
						// Otherwise to the closest point we hit
						to = h.ClosestPointOnBounds(from);
					}
					Gizmos.DrawLine(from, to);
				}
			}
			GizHelper.DrawWireSphere(from, maxStrength, Color.green);
		}
	}
}