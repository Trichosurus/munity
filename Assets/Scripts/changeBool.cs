using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeBool : MonoBehaviour {
	public string varName;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseUp() {
		if (varName == "skipOcclusion") { GlobalData.skipOcclusion = !GlobalData.skipOcclusion;}
		if (varName == "captureMouse") { GlobalData.captureMouse = !GlobalData.captureMouse;}
		if (varName == "globalLighting") { GlobalData.globalLighting = !GlobalData.globalLighting;}
		if (varName == "playerLight") { GlobalData.playerLight = !GlobalData.playerLight;}
		if (varName == "alwaysRun") { GlobalData.alwaysRun = !GlobalData.alwaysRun;}
		if (varName == "forceSpriteMultivews") { GlobalData.forceSpriteMultivews = !GlobalData.forceSpriteMultivews;}
		GlobalData.writeSettings();
	}


}
