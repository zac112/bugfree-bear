using UnityEngine;

public abstract class Mover : MonoBehaviour {

	public float movingSpeed = 1f;

	public virtual void Move (Vector2 direction){
		//rigidbody2D.velocity = direction.normalized*movingSpeed;
		transform.position = Vector2.MoveTowards(transform.position,direction,movingSpeed/100);
	}
}
