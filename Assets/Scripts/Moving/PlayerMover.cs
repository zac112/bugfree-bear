using UnityEngine;

public class PlayerMover : Mover
{

	private void OnEnable()
	{
		EnableMovement ();
	}

	public void Move()
	{
		Move(InputHandler.GetInputVector());
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