using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControlPanel {

	public short type = 0;
	public short permutation = 0;
	public int controlPanelStatus = 0;
	public bool controlPanel = false;
	public bool repairSwitch = false;
	public bool destructiveSwitch = false;
	public bool active = false;
	public int lightSwitch = -1;
	public int platformSwitch = -1;
	public int tagSwitch = -1;
	public bool canBeDestroyed = false;
	public bool canOnlyBeHitByProjectiles = false;
	public bool dirty = false;
	public bool savePoint = false;
	public bool terminal = false;
	public Material activeMat = new Material(Shader.Find("Custom/StandardClippableV2"));
	public Material inactiveMat = new Material(Shader.Find("Custom/StandardClippableV2"));


	private bool toggled = false;
	public void toggle(GameObject wall, bool playerTouched = false) {
		// if (toggled) {return;}
		// toggled = true;
		// active = !active;
		if (playerTouched) {
			if (platformSwitch > -1) {
				MapSegment pol = GlobalData.map.segments[platformSwitch];
				if (pol.platform != null) {
					if (!active) {
						pol.platform.activate();
					} else {
						pol.platform.deActivate();
					}
				}
			}
			if (lightSwitch > -1) {
				GlobalData.map.lights[lightSwitch].toggle();
			}
			if (tagSwitch > -1) {
				foreach(mapLight light in GlobalData.map.lights) {
					if (light.mapTag == tagSwitch) {
						light.toggle();
					}
				}
				foreach(MapSegment seg in GlobalData.map.segments) {
					if (seg.platform != null && seg.platform.mapTag == tagSwitch) {
						seg.platform.activate();
					}
				}

			}



		} else {
			active = !active;
			Vector2 offset = wall.GetComponent<MeshRenderer>().material.mainTextureOffset;
			if (active) {
				wall.GetComponent<MeshRenderer>().material = activeMat;
			} else {
				wall.GetComponent<MeshRenderer>().material = inactiveMat;
			}
			wall.GetComponent<MeshRenderer>().material.mainTextureOffset = offset;

		}
		// Vector2 offset = wall.GetComponent<MeshRenderer>().material.mainTextureOffset;
		// if (active) {
			// wall.GetComponent<MeshRenderer>().material = activeMat;
			// if (platformSwitch > -1) {
				// MapSegment pol = GlobalData.map.segments[platformSwitch];
				// if (playerTouched && pol.platform != null) {
					// pol.platform.activate();
				// }
				// foreach (MapSegment seg in GlobalData.map.segments) {
				// 	foreach (MapSegmentSide side in seg.sides) {
				// 		if (side.controlPanel != null && side.controlPanel.platformSwitch == platformSwitch) {
				// 			if (side.controlPanel != this) {
				// 				side.controlPanel.toggle(side.meshItem);
				// 				break;
				// 			}
				// 		}
				// 	}
				// }
			// }
		// } else {
		// 	wall.GetComponent<MeshRenderer>().material = inactiveMat;
		// }
		// wall.GetComponent<MeshRenderer>().material.mainTextureOffset = offset;
		// toggled = false;
	}
    // public enum ControlPanelClass : short {
	// Oxygen,
	// Shield,
	// DoubleShield,
	// TripleShield,
	// LightSwitch,
	// PlatformSwitch,
	// TagSwitch,
	// PatternBuffer,
	// Terminal
    // }

}
