using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObject : MonoBehaviour {
	public float speed = 1;
	public float delay = 0;
	public float maximumHeight;
	public float minimumHeight;
	public int mapTag;
	public bool initiallyActive = false;
	public bool initiallyExtended = false;
	public bool deactivatesAtEachLevel  = false;
	public bool deactivatesAtInitialLevel  = false;
	public bool extendsFloorToCeiling  = false;
	public bool comesFromFloor  = false;
	public bool comesFromCeiling  = false;
	public bool causesDamage  = false;
	public bool doesNotActivateParent  = false;
	public bool activatesOnlyOnce  = false;
	public bool activatesLight  = false;
	public bool deactivatesLight  = false;
	public bool isPlayerControllable  = false;
	public bool isMonsterControllable  = false;
	public bool reversesDirectionWhenObstructed  = false;
	public bool cannotBeExternallyDeactivated  = false;
	public bool usesNativePolygonHeights = false;
	public bool delaysBeforeActivation  = false;
	public bool activatesAdjacentPlatformsWhenActivating = false;
	public bool deactivatesAdjacentPlatformsWhenActivating  = false;
	public bool activatesAdjacentPlatformsWhenDeactivating  = false;
	public bool deactivatesAdjacentPlatformsWhenDeactivating  = false;
	public bool contractsSlower  = false;
	public bool activatesAdjacantPlatformsAtEachLevel  = false;
	public bool locked = false;
	public bool secret  = false;
	public bool door = false;
	public GameObject lowerPlatform = null;
	public GameObject upperPlatform = null;
	public MapSegment parent = null;
	public List<MapSegmentSide> upperSides = new List<MapSegmentSide>();
	public List<MapSegmentSide> lowerSides = new List<MapSegmentSide>();
	public MapSegmentFloorCeiling upperTop;
	public MapSegmentFloorCeiling lowerTop;
	public MapSegmentFloorCeiling upperBottom;
	public MapSegmentFloorCeiling lowerBottom;
	public float upperMaxHeight = 0;
	public float upperMinHeight = 0;
	public float lowerMaxHeight = 0;
	public float lowerMinHeight = 0;

	public bool active = false;
	public bool extended = true;
	public bool extending = false;
	//public float delayedTime = 0;
	private bool hasActivated = false;

	private int activatedBy = -1;





	private bool inTransit = false;
	private bool fromTop = false;
	public float delayedTime = 0;

	private Vector3 uMax, uMin, lMax, lMin;


	public void init () {
		float travelDistance = (maximumHeight-minimumHeight);
		float lheight = parent.height.y;
		uMax = new Vector3(0,maximumHeight - parent.centerPoint.y,0);
		uMin = new Vector3(0,minimumHeight - parent.centerPoint.y,0);
		lMax = new Vector3(0,maximumHeight - parent.centerPoint.y - lheight,0);
		lMin = new Vector3(0,minimumHeight - parent.centerPoint.y - lheight,0);

		if (comesFromCeiling && comesFromFloor) {
			float midpoint = travelDistance/2f + (minimumHeight - parent.centerPoint.y);
			lheight = parent.height.y-(parent.height.y-midpoint);
			uMin = new Vector3(0,midpoint,0);
			lMax = new Vector3(0,midpoint - lheight,0);
			lMin = new Vector3(0,minimumHeight - parent.centerPoint.y - lheight,0);
		}
			

		if (initiallyExtended) {
			extending = false;
			if (upperPlatform != null) {upperPlatform.transform.localPosition = uMin;}
			if (lowerPlatform != null) {lowerPlatform.transform.localPosition = lMax;}
		} else {
			extending = true;
			if (upperPlatform != null) {upperPlatform.transform.localPosition = uMax;}
			if (lowerPlatform != null) {lowerPlatform.transform.localPosition = lMin;}
	
		}
		// if (platform.initiallyActive) {
		// 	platform.activate();
		// }
	}

	Vector3 getMaxVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos.y = maximumHeight - transform.position.y;
		return relativePos;
	}
	Vector3 getMinVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos.y = minimumHeight - transform.position.y;
		return relativePos;
	}
	Vector3 getMidVector() {
		Vector3 relativePos = new Vector3(0,0,0);
		relativePos = (getMaxVector() - getMinVector()) * 0.5f + getMinVector();
		return relativePos;
	}

	// Update is called once per frame
	void Update () {

		bool transit = true;
		if (active) {
			float dspeed = speed * Time.deltaTime;

			delayedTime += Time.deltaTime;
			if (delayedTime <= delay) {
				dspeed = 0;
			}
			
			float prevPosU = 0, prevPosL = 0;
			if (comesFromCeiling) {prevPosU = upperPlatform.transform.localPosition.y;}
			if (comesFromFloor) {prevPosL = lowerPlatform.transform.localPosition.y;}


			if (extending) {
				if (comesFromCeiling) {
					upperPlatform.transform.localPosition -= new Vector3(0,dspeed,0);
					if (upperPlatform.transform.localPosition.y <= uMin.y) {
						upperPlatform.transform.localPosition = uMin;
						transit = false;
					}
				}
				if (comesFromFloor) {
					lowerPlatform.transform.localPosition += new Vector3(0,dspeed,0);
					if (lowerPlatform.transform.localPosition.y >= lMax.y) {
						lowerPlatform.transform.localPosition = lMax;
						transit = false;
					}
				}
			} else {
				if (contractsSlower) {
					dspeed = dspeed*0.66f;
				}
				if (comesFromCeiling) {
					upperPlatform.transform.localPosition += new Vector3(0,dspeed,0);
					if (upperPlatform.transform.localPosition.y >= uMax.y) {
						upperPlatform.transform.localPosition = uMax;
						transit = false;
					}
				}
				if (comesFromFloor) {
					lowerPlatform.transform.localPosition -= new Vector3(0,dspeed,0);
					if (lowerPlatform.transform.localPosition.y <= lMin.y) {
						lowerPlatform.transform.localPosition = lMin;
						transit = false;
					}
				}
			}

			if (!transit ) {
				inTransit = false;
				hitEnd();
			}
			GameObject player = GameObject.Find("player");
			if (player != null) {
				playerController pc = player.GetComponent<playerController>();
				if (comesFromCeiling && pc.platContactU == upperPlatform.transform.Find("platBottom").gameObject.GetComponent<Collider>()) {
					player.transform.position = new Vector3(player.transform.position.x, 
															player.transform.position.y + (upperPlatform.transform.localPosition.y - prevPosU),
															player.transform.position.z);
				}

				if (comesFromFloor && pc.platContactL == lowerPlatform.transform.Find("platTop").gameObject.GetComponent<Collider>()) {
					player.transform.position = new Vector3(player.transform.position.x, 
															player.transform.position.y + (lowerPlatform.transform.localPosition.y - prevPosL),
															player.transform.position.z);
				}
			}

		}

		if (upperPlatform != null) {
			upperBottom.setLight();
			upperTop.setLight();
			foreach (MapSegmentSide side in upperSides) {
				side.setLight();
			}
		} 
		if (lowerPlatform != null) {
			lowerBottom.setLight();
			lowerTop.setLight();
			foreach (MapSegmentSide side in lowerSides) {
				side.setLight();
			}
		}
	}



	private void hitEnd() {
		if (deactivatesAtEachLevel || 
			(deactivatesAtInitialLevel && (
				(initiallyExtended && extending) ||
				(!initiallyExtended && !extending)
			))	) {
				active = false;
		}

		if (activatesAdjacantPlatformsAtEachLevel) {
			MapSegment.Message message = new MapSegment.Message();
			message.activatePlatform = true;
			List<int> exclude = new List<int>();
			parent.sendMessage(message, exclude);
		}

		if (!active) {
			deActivate(-4);
		}

		extending = !extending;
		delayedTime = 0;
	}


	public void playerTouch() {
		//Debug.Log("platform");
		//element.GetComponent<Mesh>().material =
				Debug.Log("woo?");
		if (door) {
			if (!locked) {
				activate(-2);
			} else {
				//?? play locked sound??
			}
		}
	}

	public void activate(int activator = -1) {
		if (activatesOnlyOnce && hasActivated) {return;}
		if (!isPlayerControllable && activator == -2) {return;}
		if (!isMonsterControllable && activator == -3) {return;}

 
		activatedBy = activator;
		active = true;
		// extending = !extending;
		delayedTime = 0;
		if (!delaysBeforeActivation) {delayedTime = delay;}
		hasActivated = true;
		
		foreach (ControlPanel cp in GlobalData.map.controlPanels) {
			if (cp.platformSwitch == parent.id || cp.tagSwitch == mapTag) {
				cp.toggle();
			}
		}

		if (activatesAdjacentPlatformsWhenActivating) {
			MapSegment.Message message = new MapSegment.Message();
			message.activatePlatform = true;
			List<int> exclude = new List<int>();
			if (doesNotActivateParent) {exclude.Add(activator);}
			parent.sendMessage(message, exclude);
		}
		if (deactivatesAdjacentPlatformsWhenActivating) {
			MapSegment.Message message = new MapSegment.Message();
			message.deActivatePlatform = true;
			List<int> exclude = new List<int>();
			if (doesNotActivateParent) {exclude.Add(activator);}
			parent.sendMessage(message, exclude);
		}

		if (activatesLight) {
			foreach (MapSegmentSide s in parent.sides) {
				if (s.upperLight != null) s.upperLight.activate();
				if (s.middleLight != null) s.middleLight.activate();
				if (s.lowerLight != null) s.lowerLight.activate();
			}
			parent.floor.light.activate();
			parent.ceiling.light.activate();

		}


	}




	

	public void deActivate(int activator = -1) {
		if (activator > -4) {return;}
		if (deactivatesAdjacentPlatformsWhenDeactivating) {
			MapSegment.Message message = new MapSegment.Message();
			message.deActivatePlatform = true;
			parent.sendMessage(message);
		}
		if (deactivatesLight) {
			foreach (MapSegmentSide s in parent.sides) {
				if (s.upperLight != null) s.upperLight.deActivate();
				if (s.middleLight != null) s.middleLight.deActivate();
				if (s.lowerLight != null) s.lowerLight.deActivate();
			}
			parent.floor.light.deActivate();
			parent.ceiling.light.deActivate();
		}
		if (activatesAdjacentPlatformsWhenDeactivating) {
			MapSegment.Message message = new MapSegment.Message();
			message.activatePlatform = true;
			parent.sendMessage(message);
		}
		active = false;

		foreach (ControlPanel cp in GlobalData.map.controlPanels) {
			if (cp.platformSwitch == parent.id || cp.tagSwitch == mapTag) {
				cp.toggle();
			}
		}
	}

}