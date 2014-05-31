using UnityEngine;
using System.Collections;

public class PlayerMover : Mover
{
	void Start()
	{
		InputHandler.OnMoveDown += DoMove;
		InputHandler.OnMoveUp += DoMove;
		InputHandler.OnMoveRight += DoMove;
		InputHandler.OnMoveLeft += DoMove;
	}

	public void DoMove()
	{
		Move(InputHandler.GetInputVector());
	}

	public override void Move(Vector2 direction)
	{
		cachedTransform.position += ((Vector3)direction).normalized * Time.deltaTime * Speed;
	}

	void OnDisable()
	{
		InputHandler.OnMoveDown -= DoMove;
		InputHandler.OnMoveUp -= DoMove;
		InputHandler.OnMoveRight -= DoMove;
		InputHandler.OnMoveLeft -= DoMove;
	}
}