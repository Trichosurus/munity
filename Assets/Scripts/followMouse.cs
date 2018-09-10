using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 p = new Vector3();
        Camera  c = Camera.main;
        Event   e = Event.current;
        Vector2 mousePos = new Vector2();
        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = Input.mousePosition.x;
        mousePos.y = Input.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5.23f));	
		Debug.Log(p);
		transform.position = new Vector3(p.x,p.y,transform.position.z);
		}
}
