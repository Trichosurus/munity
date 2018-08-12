using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetConfig : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseUp () {
		GlobalData.inputController = new InputController();
		GlobalData.inputController.setDefaultKeys();
		GlobalData.skipOcclusion = false;
		GlobalData.occlusionOverDraw = 1;
		GlobalData.occlusionDensity = 0.5f;
		GlobalData.captureMouse = true;
		GlobalData.globalLighting = true;
		GlobalData.globalLightingIntensity = 0.15f;
		GlobalData.playerLight = true;
		GlobalData.playerLightType = 0;
		GlobalData.playerLightIntensity = 0.7f;
		GlobalData.playerLightRange = 3f;
		GlobalData.playerLightPosition = 0;
		GlobalData.playerLightDelay = 0;
		GlobalData.playerFOV = 60f;
		GlobalData.spriteType = 2;
		GlobalData.forceSpriteMultivews = false;
		GlobalData.landscapeType = 5;
		GlobalData.alwaysRun = true;
		GlobalData.graviyScaleFactor = 2f;
		GlobalData.antiGraviyScaleFactor = 1f;
		GlobalData.accellerationScaleFactor = 2f;
		GlobalData.decellerationScaleFactor = 2f;
		GlobalData.deBounceFactor = 4f;
		GlobalData.mapToLoad = 0;
		
		GlobalData.writeSettings();
	}
}
