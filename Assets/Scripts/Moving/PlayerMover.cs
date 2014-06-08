using UnityEngine;
using ShowEmAll;

public class PlayerMover : Mover
{
	private enum Direction
	{
		UP,
		LEFT,
		DOWN,
		RIGHT,
		IDLE
	}

	[SerializeField, RequiredFromChildren("Footsteps")]
	private SoundEmitter footsteps;
	[SerializeField]
	private Direction currentDirection = Direction.IDLE;
	private Animator animator;

	private void OnEnable()
	{
		animator = GetComponent<Animator>();
		EnableMovement();
	}

	public void Move()
	{
		animator.SetInteger("Direction", (int)currentDirection);
		Move(InputHandler.InputVector);
		footsteps.Emit();
	}

	public override void Move(Vector2 direction)
	{
		var right = Vector2.right;
		var left = -right;
		var up = Vector2.up;
		var down = -up;

		System.Func<Vector2, Vector2, float> angle = Vector2.Angle;
		System.Func<float, float> abs = Mathf.Abs;

		if (abs(angle(right, direction)) <= 45)
			log("right");
		if (abs(angle(left, direction)) <= 45)
			log("left");
		if (abs(angle(up, direction)) <= 45)
			log("up");
		if (abs(angle(down, direction)) <= 45)
			log("down");

		cachedTransform.position += ((Vector3)direction).normalized * SmoothedMovement;
	}

	public void EnableMovement()
	{
		InputHandler.OnMoveDown += MoveDown;
		InputHandler.OnMoveUp += MoveUp;
		InputHandler.OnMoveRight += MoveRight;
		InputHandler.OnMoveLeft += MoveLeft;
		InputHandler.NotMoving += BecomeIdle;
	}

	void MoveRight()
	{
		currentDirection = Direction.RIGHT;
		Move();
	}

	void MoveLeft()
	{
		currentDirection = Direction.LEFT;
		Move();
	}

	void MoveUp()
	{
		currentDirection = Direction.UP;
		Move();
	}

	void MoveDown()
	{
		currentDirection = Direction.DOWN;
		Move();
	}

	void BecomeIdle()
	{
		animator.SetInteger("Direction", (int)Direction.IDLE);
	}

	public void DisableMovement()
	{
		InputHandler.OnMoveDown -= MoveDown;
		InputHandler.OnMoveUp -= MoveUp;
		InputHandler.OnMoveRight -= MoveRight;
		InputHandler.OnMoveLeft -= MoveLeft;
		InputHandler.NotMoving -= BecomeIdle;
	}

	private void OnDisable()
	{
		DisableMovement();
	}
}