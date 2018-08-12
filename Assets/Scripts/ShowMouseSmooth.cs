using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMouseSmooth : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (GlobalData.mouseSmooth <= 0) {GlobalData.mouseSmooth = 0.00001f;}

		GameObject textObj = gameObject.transform.Find("text").gameObject;
		float amount = 1;
		amount = GlobalData.mouseSmooth;
		textObj.transform.position = transform.TransformPoint(new Vector3(0, amount * 0.2f - 0.45f, 0));
		textObj.GetComponent<TextMesh>().text = amount.ToString("0.00");

	}
}
