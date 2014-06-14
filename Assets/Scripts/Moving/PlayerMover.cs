using UnityEngine;
using ShowEmAll;

public class PlayerMover : Mover
{
	[SerializeField, RequiredFromChildren("Footsteps")]
	private SoundEmitter footsteps;

	[SerializeField, RequiredFromThis(true)]
	private Animator animator;

	private void OnEnable()
	{
		animator = GetComponent<Animator> ();
		EnableMovement();
	}

	public void Move() // We wouldn't need this if the Move methods in InputHandler pass the input vector. An idea. Not sure about it yet...
	{
		Move(InputHandler.InputVector);
	}

	public override void Move(Vector2 direction)
	{
		animator.SetInteger("HorizontalDirection", (int)direction.x);
		animator.SetInteger("VerticalDirection", (int)direction.y);
		animator.SetBool("Idle", false);
		MoveBy(direction);
		footsteps.Emit();
	}

	public void EnableMovement()
	{
		InputHandler.SubscribeToMovement(Move);
		InputHandler.NotMoving += BecomeIdle;
	}

	public void DisableMovement()
	{
		InputHandler.UnsubscribeFromMovement(Move);
		InputHandler.NotMoving -= BecomeIdle;
	}

	private void BecomeIdle()
	{
		animator.SetBool("Idle", true);
	}

	private void OnDisable()
	{
		DisableMovement();
	}
}