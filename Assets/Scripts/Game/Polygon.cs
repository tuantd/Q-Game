using UnityEngine;
using System.Collections;

public class Polygon : MonoBehaviour {
	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = rigidbody2D.velocity;
		if (velocity.magnitude == 0f){
			rigidbody2D.fixedAngle = true;
		}
	}


}
