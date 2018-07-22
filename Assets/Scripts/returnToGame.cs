using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnToGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void OnMouseUp () {
		GameObject.Find("Menu").SetActive(false);
		GameObject.Find("player").SetActive(true);

	}
}
