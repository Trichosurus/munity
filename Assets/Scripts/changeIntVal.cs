using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeIntVal : MonoBehaviour {
	public string varName;
	public int amount = 1;
	private float prevMousePos = -1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseUp() {
		adjustValue(amount);
		GlobalData.writeSettings();
	}

	void OnMouseDrag() {
		if (prevMousePos > 0) {
			if (Input.mousePosition.y > prevMousePos) {
				adjustValue(Mathf.Abs(amount));
			} else if (Input.mousePosition.y < prevMousePos) {
				adjustValue(0-Mathf.Abs(amount));
			}
		}
		prevMousePos = Input.mousePosition.y;
	}


	void adjustValue(int amount) {
		if (varName == "occlusionOverDraw") { GlobalData.occlusionOverDraw += amount;}
		if (varName == "playerLightType") { GlobalData.playerLightType += amount;}
		if (varName == "playerLightPosition") { GlobalData.playerLightPosition += amount;}
		if (varName == "spriteType") { GlobalData.spriteType += amount;}
		if (varName == "landscapeType") { GlobalData.landscapeType += amount;}

	}
}
