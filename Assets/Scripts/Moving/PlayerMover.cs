using UnityEngine;
using System.Collections;

public class PlayerMover : Mover {

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
		Move (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
	}

}
