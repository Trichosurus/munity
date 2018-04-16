using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformController : MonoBehaviour {
	public bool inTransit = false;
	private Platform platform = null;
	private bool fromTop = false;
	private bool hasPartner = false;
	public float delayedTime = 0;

	public int platformid = -1;

	private Vector3 max, min;
	// Use this for initialization
	void Start () {
		if (platform == null) {
			platform = gameObject.transform.parent.GetComponent<MapSegment>().platform;
		}
		platformid = platform.parent.id;
		Vector3 height = gameObject.transform.parent.GetComponent<MapSegment>().height;
		// Debug.Log(platform.MaximumHeight);
		// Debug.Log(platform.MinimumHeight);
		// Debug.Log(height);

		if (platform.comesFromCeiling == true && platform.comesFromFloor == true) {
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
		// if (platform.upperPlatform.transform.parent.GetComponent<MapSegment>().id == 48) {
		// Debug.Log(min);
		// Debug.Log(max);
		// }

		if (platform.initiallyExtended) {
			platform.extended = true;
			platform.extending = true;
			if (fromTop) {
				gameObject.transform.localPosition = min;
			} else {
				gameObject.transform.localPosition = max;
			}
		} else {
			platform.extended = false;
			platform.extending = false;

			if (fromTop) {
				gameObject.transform.localPosition = max;
			} else {
				gameObject.transform.localPosition = min;
			}
	
		}
		// if (platform.initiallyActive) {
		// 	platform.activate();
		// }
	}

	Vector3 getMaxVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos.y = platform.maximumHeight - gameObject.transform.parent.position.y;
		return relativePos;
	}
	Vector3 getMinVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos.y = platform.minimumHeight - gameObject.transform.parent.position.y;
		return relativePos;
	}
	Vector3 getMidVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos = (getMaxVector() - getMinVector()) * 0.5f + getMinVector();
		return relativePos;
	}

	// public void playerTouch() {
	// 	//Debug.Log("platform");
	// 	//element.GetComponent<Mesh>().material =
	// 	if (door) {
	// 		if (!locked) {
	// 			activate();
	// 		} else {
	// 			//?? play locked sound??
	// 		}
	// 	}
	// }

	// public void activate() {
	// 		active = !active;
	// 	if (delayedTime >= delay) {
	// 		extending = !extending;
	// 		extended = !extended;
	// 		delayedTime = 0;
	// 		hasActivated = true;
	// 	}
	// }


	// Update is called once per frame
	void Update () {
			// if (platformid == 436 || platformid == 443) {
			// 	;
			// }

		bool transit = true;
		if (platform.active) {

			float speed = platform.speed * Time.deltaTime;


			if (delayedTime <= platform.delay) {
				delayedTime += Time.deltaTime;
				speed = 0;
			}

			// if (speed != 0 && platformid == 0) {
			// 	;
			// }

			// Debug.Log("active");
			// Debug.Log(platform.Speed);
			// Debug.Log(platform.extending);

			if (platform.extending) {
				if (fromTop) {
					if (gameObject.transform.localPosition.y > min.y) {
						gameObject.transform.localPosition -= new Vector3(0,speed,0);
					} else {
						gameObject.transform.localPosition = min;
						transit = false;
					}
				} else {
					if (gameObject.transform.localPosition.y < max.y) {
						gameObject.transform.localPosition += new Vector3(0,speed,0);
					} else {
						gameObject.transform.localPosition = max;
						transit = false;
					}
				}
				
			} else {
				if (platform.contractsSlower) {
					speed = speed*0.66f;
				}
				if (fromTop) {
					if (gameObject.transform.localPosition.y < max.y) {
						gameObject.transform.localPosition += new Vector3(0,speed,0);
					} else {
						gameObject.transform.localPosition = max;
						transit = false;

					}
				} else {
					if (gameObject.transform.localPosition.y > min.y) {
						gameObject.transform.localPosition -= new Vector3(0,speed,0);
					} else {
						gameObject.transform.localPosition = min;
						transit = false;

					}
				}

			}
			// Debug.Log(transit);
			// Debug.Log(transit);

			if (!transit ) {
				inTransit = false;
				// platform.active = false;
				// platform.extended = !platform.extended;
				// platform.extending = !platform.extending;
				platform.deActivate();
			}
		}

		if (fromTop) {
			platform.upperBottom.setLight();
			platform.upperTop.setLight();
			foreach (MapSegmentSide side in platform.upperSides) {
				side.setLight();
			}
		} else {
			platform.lowerBottom.setLight();
			platform.lowerTop.setLight();
			foreach (MapSegmentSide side in platform.lowerSides) {
				side.setLight();
			}
	
		}

	}
}
