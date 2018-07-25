using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnToGame : MonoBehaviour {
	// Use this for initialization
	void Update () {
		if (Input.GetKey("return")) {
			goToGame();
		}
	}
	

	void OnMouseUp () {
		goToGame();
	}

	void goToGame() {
		GlobalData.map.menu.SetActive(false);
		GlobalData.map.player.SetActive(true);
		GlobalData.map.player.GetComponent<playerController>().updatePlayerSettings();
	}
}
