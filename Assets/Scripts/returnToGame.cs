using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnToGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void OnMouseUp () {
		GlobalData.map.menu.SetActive(false);
		GlobalData.map.player.SetActive(true);

	}
}
