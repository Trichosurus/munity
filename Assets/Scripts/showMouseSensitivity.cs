using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showMouseSensitivity : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject textObj = gameObject.transform.Find("text").gameObject;
		float amount = 1;
		foreach (InputController.axis a in GlobalData.inputController.axes) {
			if (a.name == "Mouse X") {
				amount = a.sensitivity;
			}
		}
		textObj.transform.position = transform.TransformPoint(new Vector3(0, amount * 0.6f - 0.45f, 0));
		textObj.GetComponent<TextMesh>().text = amount.ToString("0.0");

	}
}
