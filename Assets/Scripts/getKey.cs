using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getKey : MonoBehaviour {
	public string inputName;
	private bool listening = false; 
	// Use this for initialization
	void Start () {
		drawString();
	}
	
	// Update is called once per frame
	void Update () {
		if (listening) {
			foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
				if(Input.GetKey(vKey)){
				//your code here
					InputController.button button = new InputController.button();
					bool found = false;
					foreach (InputController.button b in GlobalData.inputController.buttons) {	
						if (b.name == inputName) {button = b;found = true;}
					}
					if (found) {
						GlobalData.inputController.buttons.Remove(button);
						button.type = "button";
						button.keyCode = vKey;
						button.buttonName = vKey.ToString();
						button.name = inputName;	
						GlobalData.inputController.buttons.Add(button);			
					}
					listening = false;
					drawString();
					GlobalData.writeSettings();
				}
			}
			if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				Debug.Log("scrUp");
				InputController.button button = new InputController.button();
				bool found = false;
				foreach (InputController.button b in GlobalData.inputController.buttons) {	
					if (b.name == inputName) {button = b;found = true;}
				}
				if (found) {
					GlobalData.inputController.buttons.Remove(button);
					button.type = "axis";
					button.buttonName = "scrUp";
					button.name = inputName;	
					button.axisName = "Mouse ScrollWheel";
					button.axisValue = 0.0001f;		
					GlobalData.inputController.buttons.Add(button);			
				}
				listening = false;
				drawString();
				GlobalData.writeSettings();

			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				Debug.Log("scrDn");
				bool found = false;
				InputController.button button = new InputController.button();
				foreach (InputController.button b in GlobalData.inputController.buttons) {	
					if (b.name == inputName) {button = b;found = true;}
				}
				if (found) {
					GlobalData.inputController.buttons.Remove(button);
					button.type = "axis";
					button.buttonName = "scrDn";
					button.name = inputName;	
					button.axisName = "Mouse ScrollWheel";
					button.axisValue = -0.0001f;		
					GlobalData.inputController.buttons.Add(button);			
				}
				listening = false;
				drawString();
				GlobalData.writeSettings();
			}
		}
	}


	void drawString(string text = null) {
		GameObject textObj = gameObject.transform.Find("text").gameObject;
		if (text == null){
			foreach (InputController.button b in GlobalData.inputController.buttons) {	
				if (b.name == inputName) {text = b.buttonName;}
			}
		}
		if (text != null) {
			textObj.GetComponent<TextMesh>().text = text;
		}
	}


	void OnMouseUp()
	{
		listening = true;
		drawString("");
	}




}
