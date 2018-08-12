using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeMouseSpeed : MonoBehaviour {
	public float amount = 1;
	private float prevMousePos = -1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnMouseUp()
	{
		changeVal(amount);
		GlobalData.writeSettings();
	}

	void OnMouseDrag() {
		if (prevMousePos > 0) {
			if (Input.mousePosition.y > prevMousePos) {
				changeVal(Mathf.Abs(amount));
			} else if (Input.mousePosition.y < prevMousePos){
				changeVal(0-Mathf.Abs(amount));
			}
		}
		prevMousePos = Input.mousePosition.y;
	}



	void changeVal(float amount)
	{
		float sensitivity = 1;
		foreach (InputController.axis a in GlobalData.inputController.axes) {
			if (a.name == "Mouse X") {
				sensitivity = a.sensitivity;
			}
		}
		sensitivity += amount;

		for (int i = 0; i < GlobalData.inputController.axes.Count; i++ ) {
			if (GlobalData.inputController.axes[i].name == "Mouse X" || GlobalData.inputController.axes[i].name == "Mouse Y") {
				InputController.axis a = new InputController.axis();
				a.axisName = GlobalData.inputController.axes[i].axisName;
				a.name = GlobalData.inputController.axes[i].name;
				a.sensitivity = sensitivity;


				GlobalData.inputController.axes[i] = a;
			}
		}

		GlobalData.writeSettings();
	}


}
