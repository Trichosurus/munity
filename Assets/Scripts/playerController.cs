using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {

	public float accelleration = 1;
	public float maxspeed = 3;
	// Use this for initialization
	void Start () {
		
	}
	
		// Update is called once per frame

	void Update () {

		
		
	}


	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		Rigidbody rb = GetComponent<Rigidbody>();


		float z = 0;
		float x = 0;
		if (Input.GetKey("w")){z += 1;}
		if (Input.GetKey("s")){z -= 1;}
		if (Input.GetKey("a")){x -= 1;}
		if (Input.GetKey("d")){x += 1;}

		Vector3 fwspeed = transform.forward * accelleration * z;
		Vector3 rtspeed = transform.right * accelleration * x;
		if (rb.velocity.magnitude < maxspeed) {
			rb.velocity += fwspeed + rtspeed;
		}
	}

}
