using UnityEngine;
using ShowEmAll;

public class PlayerMover : Mover
{
	private enum Direction{
		UP,
		LEFT,
		DOWN,
		RIGHT,
		IDLE
	}

	[SerializeField, RequiredFromChildren("Footsteps")]
	private SoundEmitter footsteps;
	[SerializeField]private Direction currentDirection = Direction.IDLE;
	private Animator animator;

	private void OnEnable()
	{
		animator = GetComponent<Animator>();
		EnableMovement ();
	}

	public void Move()
	{
		animator.SetInteger("Direction",(int)currentDirection);
		Move(InputHandler.InputVector);
		footsteps.Emit();
	}

	public override void Move(Vector2 direction)
	{
		cachedTransform.position += ((Vector3)direction).normalized * SmoothedMovement;
	}

	public void EnableMovement ()
	{
		InputHandler.OnMoveDown += MoveDown;
		InputHandler.OnMoveUp += MoveUp;
		InputHandler.OnMoveRight += MoveRight;
		InputHandler.OnMoveLeft += MoveLeft;
		InputHandler.NotMoving += BecomeIdle;
	}

	void MoveRight(){
		currentDirection = Direction.RIGHT;
		Move ();
	}

	void MoveLeft(){
		currentDirection = Direction.LEFT;
		Move ();
	}

	void MoveUp(){
		currentDirection = Direction.UP;
		Move ();
	}

	void MoveDown(){
		currentDirection = Direction.DOWN;
		Move ();
	}

	void BecomeIdle(){
		animator.SetInteger("Direction", (int)Direction.IDLE);
	}

	public void DisableMovement ()
	{
		InputHandler.OnMoveDown -= MoveDown;
		InputHandler.OnMoveUp -= MoveUp;
		InputHandler.OnMoveRight -= MoveRight;
		InputHandler.OnMoveLeft -= MoveLeft;
		InputHandler.NotMoving -= BecomeIdle;
	}

	private void OnDisable()
	{
		DisableMovement ();
	}
}