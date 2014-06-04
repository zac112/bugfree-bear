using UnityEngine;

public class PlayerMover : Mover
{
	void Start()
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

	void Update()
	{
		Move();
	}

	public override void Move(Vector2 direction)
	{
		cachedTransform.position += ((Vector3)direction).normalized * SmoothedMovement;
	}

	void OnDisable()
	{
		InputHandler.OnMoveDown -= Move;
		InputHandler.OnMoveUp -= Move;
		InputHandler.OnMoveRight -= Move;
		InputHandler.OnMoveLeft -= Move;
	}
}