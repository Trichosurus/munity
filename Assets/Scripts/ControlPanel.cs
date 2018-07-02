using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControlPanel {
	public int sidePart = 0;
	public int oxygen = 0;
	public int shield = 0;

	public bool patternBuffer = false;
	public int terminal = -1; 
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
	public Material activeMat = new Material(Shader.Find("Custom/StandardClippableV2"));
	public Material inactiveMat = new Material(Shader.Find("Custom/StandardClippableV2"));
	public GameObject wall = null;
	public void setDisplay() {
		if (wall == null) {
			;
		}
		Vector2 offset = wall.GetComponent<MeshRenderer>().material.mainTextureOffset;
		if (active) {
			wall.GetComponent<MeshRenderer>().material = activeMat;
		} else {
			wall.GetComponent<MeshRenderer>().material = inactiveMat;
		}
		wall.GetComponent<MeshRenderer>().material.mainTextureOffset = offset;

	}
	private bool toggled = false;
	public void toggle(bool playerTouched = false) {
		// if (toggled) {return;}
		// toggled = true;
		// active = !active;
		if (platformSwitch > -1) {
			MapSegment seg = GlobalData.map.segments[platformSwitch];
			if (seg.platform != null) {
				if (playerTouched) {
					if (!active) {
					seg.platform.activate();
					} else {
					seg.platform.deActivate();
					}
				}
				active = seg.platform.active;
			}
		}
		if (lightSwitch > -1) {
			if (playerTouched) {GlobalData.map.lights[lightSwitch].toggle();}
			active = GlobalData.map.lights[lightSwitch].phase < 3;
		}
		if (tagSwitch > -1) {
			foreach(mapLight light in GlobalData.map.lights) {
				if (light.mapTag == tagSwitch) {
					if (playerTouched) {light.toggle();}
					active = light.phase < 3;
				}
			}
			foreach(MapSegment seg in GlobalData.map.segments) {
				if (seg.platform != null && seg.platform.mapTag == tagSwitch) {
					if (playerTouched) {
						if (!active) {
						seg.platform.activate();
						} else {
						seg.platform.deActivate();
						}
					}
				active = seg.platform.active;
				}
			}

		}
		setDisplay();
	}

}
