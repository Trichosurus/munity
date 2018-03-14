using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformController : MonoBehaviour {
	private Platform platform = null;
	private bool fromTop = false;
	private bool hasPartner = false;

	private Vector3 max, min;
	// Use this for initialization
	void Start () {
		if (platform == null) {
			platform = gameObject.transform.parent.GetComponent<MapSegment>().platform;
		}

		Vector3 height = gameObject.transform.parent.GetComponent<MapSegment>().height;
		// Debug.Log(platform.MaximumHeight);
		// Debug.Log(platform.MinimumHeight);
		// Debug.Log(height);

		if (platform.ComesFromCeiling == true && platform.ComesFromFloor == true) {
			hasPartner = true;
			height.y = getMidVector().y;
		}
		
		if (gameObject.name == "upperPlatform") {
			fromTop = true;
			if (hasPartner) {
				min = getMidVector();
			} else {
				min = getMinVector();
			}
			max = getMaxVector();
		} else { //lower platform

			if (hasPartner) {
				max = getMidVector();
			} else {
				max = getMaxVector();
			}
			min = getMinVector();
			min.y = min.y - height.y;
			max.y = max.y - height.y;
		}
		// Debug.Log(min);
		// Debug.Log(max);


		if (platform.InitiallyExtended) {
			if (fromTop) {
				gameObject.transform.localPosition = min;
			} else {
				gameObject.transform.localPosition = max;
			}
		} else {
			if (fromTop) {
				gameObject.transform.localPosition = max;
			} else {
				gameObject.transform.localPosition = min;
			}
	
		}
	}

	Vector3 getMaxVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos.y = platform.MaximumHeight - gameObject.transform.parent.position.y;
		return relativePos;
	}
	Vector3 getMinVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos.y = platform.MinimumHeight - gameObject.transform.parent.position.y;
		return relativePos;
	}
	Vector3 getMidVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos = (getMaxVector() - getMinVector()) * 0.5f + getMinVector();
		return relativePos;
	}


	// Update is called once per frame
	void Update () {
		
	}
}
