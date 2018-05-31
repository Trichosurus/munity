using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObject {
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
		extending = !extending;
		extended = !extended;
		float delayTimer = 0;
		if (!delaysBeforeActivation) {delayTimer = delay;}
		if( upperPlatform != null ) {
			upperPlatform.GetComponent<platformController>().delayedTime = delayTimer;
		}
		if (lowerPlatform != null) {
			lowerPlatform.GetComponent<platformController>().delayedTime = delayTimer;
		}
		hasActivated = true;
		
		foreach (MapSegment seg in GlobalData.map.segments) {
			foreach (MapSegmentSide side in seg.sides) {
				if (side.controlPanel != null  && side.controlPanel.platformSwitch == parent.id) {
					side.controlPanel.toggle(side.upperMeshItem);
				}
			}
		}

		foreach (MapSegmentSide side in parent.sides) {
			if (side.connection != null && side.connection.platform != null) {
				if (side.connection.id != activatedBy || !doesNotActivateParent) {
					if (activatesAdjacentPlatformsWhenActivating) {
						side.connection.platform.activate(parent.id);
					}
					if (deactivatesAdjacentPlatformsWhenActivating) {
						side.connection.platform.deActivate(parent.id);
					}
				}
			}

		}


	}


	

	public void deActivate(int activator = -1) {
		bool uptransit = false;
		bool lotransit = false;
		if (upperPlatform != null) {
			uptransit = upperPlatform.GetComponent<platformController>().inTransit;
			lotransit = uptransit;
		}
		if (lowerPlatform != null) {
			lotransit = lowerPlatform.GetComponent<platformController>().inTransit;
			if (upperPlatform == null) {
				uptransit = lotransit;
			}
		}
		bool deactivating = false;
		if (uptransit == lotransit) {
			//Debug.Log("stop?" + parent.id);
			if (parent.id == 6) {
				;
			}
			if (deactivatesAtEachLevel || 
				(deactivatesAtInitialLevel && (
					(initiallyExtended && extended) ||
					(!initiallyExtended && !extended)
				))	) {
				deactivating = true;
				active = false;
			}


			if (activatesAdjacantPlatformsAtEachLevel ||
				((activatesAdjacentPlatformsWhenDeactivating ||	deactivatesAdjacentPlatformsWhenDeactivating)
					 && deactivating))  {
				foreach (MapSegmentSide side in parent.sides) {
					if (side.connection != null && side.connection.platform != null) {
						if (side.connection.id != parent.id || !doesNotActivateParent) {
							if (activatesAdjacentPlatformsWhenDeactivating) {
								side.connection.platform.activate(parent.id);
							}
							if (deactivatesAdjacentPlatformsWhenDeactivating) {
								side.connection.platform.deActivate();
							}

						}
					}
				}
			}

			if (deactivating) {
				foreach (MapSegment seg in GlobalData.map.segments) {
					foreach (MapSegmentSide side in seg.sides) {
						if (side.controlPanel != null  && side.controlPanel.platformSwitch == parent.id) {
							side.controlPanel.toggle(side.upperMeshItem);
						}
					}
				}
			}

			if (!deactivating) {
				if (upperPlatform != null ) {
					upperPlatform.GetComponent<platformController>().delayedTime = 0;
				}
				if (lowerPlatform != null) {
					lowerPlatform.GetComponent<platformController>().delayedTime = 0;
				}
				//this.activate();
				active = true;
				extending = !extending;
				extended = !extended;

			}


		}
	}
}
