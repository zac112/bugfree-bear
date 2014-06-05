using UnityEngine;
using ShowEmAll;

public class PlayerMover : Mover
{
	[SerializeField, RequireFromChildren("Footsteps")]
	private SoundEmitter footsteps;

	private void OnEnable()
	{
		EnableMovement ();
	}

	public void Move()
	{
		Move(InputHandler.InputVector);
		footsteps.Emit();
	}

	public override void Move(Vector2 direction)
	{
		cachedTransform.position += ((Vector3)direction).normalized * SmoothedMovement;
	}

	public void EnableMovement ()
	{
		InputHandler.SubscribeToMovement(Move);
	}

	public void DisableMovement ()
	{
		InputHandler.UnsubscribeFromMovement(Move);
	}

	private void OnDisable()
	{
		DisableMovement ();
	}
}