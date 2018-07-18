using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCameraUpDown : MonoBehaviour {
	Vector3 startPos;
	public float amount = 11f;
	public float travelTime = 1f;
	bool active;
	// Use this for initialization
	void Start () {
		startPos = GameObject.Find("Main Camera").transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (!active) {return;}
		Vector3 move = new Vector3(0,(amount * Time.deltaTime)/travelTime);
		GameObject.Find("Main Camera").transform.position += move;

		if ((GameObject.Find("Main Camera").transform.position.y > startPos.y + amount  && amount > 0)
		||(GameObject.Find("Main Camera").transform.position.y < startPos.y + amount  && amount <= 0)) {
			GameObject.Find("Main Camera").transform.position = new Vector3(startPos.x, startPos.y + amount, startPos.z);
			active = false;
		}

	}


	void OnMouseUp()
	{
		startPos = GameObject.Find("Main Camera").transform.position;
		active = true;
	}

}
