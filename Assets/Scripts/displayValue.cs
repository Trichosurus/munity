using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayValue : MonoBehaviour {
	public string varName;
	 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		GameObject textObj = gameObject.transform.Find("text").gameObject;
		string displayValue = "";

		if (varName == "shapesFilePath") { displayValue = GlobalData.shapesFilePath;}
		if (varName == "mapsFilePath") { displayValue = GlobalData.mapsFilePath;}
		if (varName == "soundsFilePath") { displayValue = GlobalData.soundsFilePath;}
		if (varName == "physicsFilePath") { displayValue = GlobalData.physicsFilePath;}
		if (varName == "imagesFilePath") { displayValue = GlobalData.imagesFilePath;}
		if (varName == "skipOcclusion") { displayValue = GlobalData.skipOcclusion.ToString();}
		if (varName == "occlusionOverDraw") { displayValue = GlobalData.occlusionOverDraw.ToString();}
		if (varName == "occlusionDensity") { displayValue = GlobalData.occlusionDensity.ToString("0.0");}
		if (varName == "captureMouse") { displayValue = GlobalData.captureMouse.ToString();}
		if (varName == "globalLighting") { displayValue = GlobalData.globalLighting.ToString();}
		if (varName == "globalLightingIntensity") { displayValue = GlobalData.globalLightingIntensity.ToString("0.00");}
		if (varName == "playerLight") { displayValue = GlobalData.playerLight.ToString();}
		if (varName == "playerLightType") { displayValue = GlobalData.playerLightType.ToString();}
		if (varName == "playerLightIntensity") { displayValue = GlobalData.playerLightIntensity.ToString("0.00");}
		if (varName == "playerLightPosition") { displayValue = GlobalData.playerLightPosition.ToString();}
		if (varName == "playerLightDelay") { displayValue = GlobalData.playerLightDelay.ToString("0.00");}
		if (varName == "playerLightRange") { displayValue = GlobalData.playerLightRange.ToString("0.0");}
		if (varName == "spriteType") { displayValue = GlobalData.spriteType.ToString();}
		if (varName == "landscapeType") { displayValue = GlobalData.landscapeType.ToString();}
		if (varName == "alwaysRun") { displayValue = GlobalData.alwaysRun.ToString();}
		if (varName == "graviyScaleFactor") { displayValue = GlobalData.graviyScaleFactor.ToString("0.00");}
		if (varName == "antiGraviyScaleFactor") { displayValue = GlobalData.antiGraviyScaleFactor.ToString("0.00");}
		if (varName == "accellerationScaleFactor") { displayValue = GlobalData.accellerationScaleFactor.ToString("0.00");}
		if (varName == "decellerationScaleFactor") { displayValue = GlobalData.decellerationScaleFactor.ToString("0.00");}
		if (varName == "deBounceFactor") { displayValue = GlobalData.deBounceFactor.ToString("0.00");}
		if (varName == "forceSpriteMultivews") { displayValue = GlobalData.forceSpriteMultivews.ToString();}

		displayValue = displayValue.Replace("True", "Y");
		displayValue = displayValue.Replace("False", "N");

		textObj.GetComponent<TextMesh>().text = displayValue;

	}



}
