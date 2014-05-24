using UnityEngine;

public abstract class Mover : MonoBehaviour {

	[SerializeField]
	private float speed = 2f;
	public float Speed {
		get {return speed;}
		set {speed = value;}
	}

	public virtual void Move (Vector2 direction){
		transform.position = Vector2.MoveTowards(transform.position,direction,Speed/100);
	}
}
