using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeFloatVal : MonoBehaviour {

	// Use this for initialization
	public string varName;
	public float amount = 1;
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
			} else if (Input.mousePosition.y < prevMousePos){
				adjustValue(0-Mathf.Abs(amount));
			}
		}
		prevMousePos = Input.mousePosition.y;
	}


	void adjustValue(float amount) {
		if (varName == "occlusionDensity") { GlobalData.occlusionDensity += amount;}
		if (varName == "globalLightingIntensity") { GlobalData.globalLightingIntensity += amount;}
		if (varName == "playerLightIntensity") { GlobalData.playerLightIntensity += amount;}
		if (varName == "playerLightDelay") { GlobalData.playerLightDelay += amount;}
		if (varName == "playerLightRange") { GlobalData.playerLightRange += amount;}
		if (varName == "graviyScaleFactor") { GlobalData.graviyScaleFactor += amount;}
		if (varName == "antiGraviyScaleFactor") { GlobalData.antiGraviyScaleFactor += amount;}
		if (varName == "accellerationScaleFactor") { GlobalData.accellerationScaleFactor += amount;}
		if (varName == "decellerationScaleFactor") { GlobalData.decellerationScaleFactor += amount;}
		if (varName == "deBounceFactor") { GlobalData.deBounceFactor += amount;}
		if (varName == "playerFOV") { GlobalData.playerFOV += amount;}
		if (varName == "mouseSmooth") { GlobalData.mouseSmooth += amount;}

	}

}
