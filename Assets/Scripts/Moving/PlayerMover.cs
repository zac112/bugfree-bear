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
		//Move();
		var forward = transform.TransformDirection(Vector3.forward);
		var input = Input.GetAxis("Vertical");
		transform.position += forward * input * Time.deltaTime;
		transform.Rotate(0, Input.GetAxis("Horizontal") * 45 * Time.deltaTime, 0);
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