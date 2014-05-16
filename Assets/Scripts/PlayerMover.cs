using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour, IMovable {

	[SerializeField]
	private float jumpSpeed = 7f;
	public float JumpSpeed {
		get {return jumpSpeed;}
		set {jumpSpeed = value;}
	}

	[SerializeField]
	private float speed = 2f;
	public float Speed {
		get {return speed;}
		set {speed = value;}
	}

	void FixedUpdate(){
		Move ();
	}


	public void Move ()
	{
		if(Input.GetButtonDown("Jump"))
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,jumpSpeed);
		rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal")*speed,rigidbody2D.velocity.y);

	}

}
