using UnityEngine;
using System.Collections;

public class Doorway : MonoBehaviour {

	public GameObject target;

	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag == "Player")
			other.gameObject.transform.position = target.transform.position;
	}
}
