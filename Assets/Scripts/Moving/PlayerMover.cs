using UnityEngine;

public class PlayerMover : Mover
{
	void Start()
	{
		EnableMovement ();
	}

	public void EnableMovement ()
	{
		InputHandler.OnMoveDown += Move;
		InputHandler.OnMoveUp += Move;
		InputHandler.OnMoveRight += Move;
		InputHandler.OnMoveLeft += Move;
	}

	public void Move()
	{
		Move(InputHandler.GetInputVector());
	}

	public override void Move(Vector2 direction)
	{
		cachedTransform.position += ((Vector3)direction).normalized * SmoothedMovement;
	}

	public void DisableMovement ()
	{
		InputHandler.OnMoveDown -= Move;
		InputHandler.OnMoveUp -= Move;
		InputHandler.OnMoveRight -= Move;
		InputHandler.OnMoveLeft -= Move;
	}

	void OnDisable()
	{
		DisableMovement ();
	}
}