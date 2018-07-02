using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapLight : MonoBehaviour {
	public int id = -1;
	public int mapTag = -1;
	public bool stateless = false;
	public bool initiallyActive = false;
	public int phase = 0;
	public int type = 0;
	public bool active = false;
	private float elapsedTime = -1;
	private bool lightChanged = true;
	private float currentIntensity;
	
	public LightFunction becomingActive = new LightFunction();
	public LightFunction primaryActive = new LightFunction();
	public LightFunction secondaryActive = new LightFunction();
	public LightFunction becomingInactive = new LightFunction();
	public LightFunction primaryInactive = new LightFunction();
	public LightFunction secondaryInactive = new LightFunction();

	public void Start () {
		becomingActive.initialise();
		primaryActive.initialise();
		secondaryActive.initialise();
		becomingInactive.initialise();
		primaryInactive.initialise();
		secondaryInactive.initialise();
		active = false;
		if (initiallyActive) {
			active = true;
			if (phase == 0) {phase = 1;}//???this seems to be how it works
		}
	}

	public void Update() {
		float intensity = currentIntensity;
		if (!stateless) {
			if (active && phase > 2) {
				elapsedTime = 0;
				phase = 0;
			}
			if (!active && phase < 3) {
				elapsedTime = 0;
				phase = 3;
			}
		}

		elapsedTime += Time.deltaTime;
		switch (phase) {
		case 0: 
			if (elapsedTime < becomingActive.totalPeriod) {
				intensity = becomingActive.lightIntensity(elapsedTime);
			} else {
				primaryActive.initialise(currentIntensity);
				phase = 1;
				elapsedTime = 0;
			}
			break;
		case 1: 
			if (elapsedTime < primaryActive.totalPeriod) {
				intensity = primaryActive.lightIntensity(elapsedTime,true);
			} else {
				secondaryActive.initialise(currentIntensity);
				phase = 2;
				elapsedTime = 0;
			}
			break;
		case 2: 
			if (elapsedTime < secondaryActive.totalPeriod) {
				intensity = secondaryActive.lightIntensity(elapsedTime);
			} else {
				if (stateless) {
					becomingInactive.initialise(currentIntensity);
					phase = 3;
				} else {
					primaryActive.initialise(currentIntensity);
					phase = 1;
				}
				elapsedTime = 0;
			}
			break;
		case 3: 
			if (elapsedTime < becomingInactive.totalPeriod) {
				intensity = becomingInactive.lightIntensity(elapsedTime, true);
			} else {
				primaryInactive.initialise(currentIntensity);
				phase = 4;
				elapsedTime = 0;
			}
			break;
		case 4: 
			if (elapsedTime < primaryInactive.totalPeriod) {
				intensity = primaryInactive.lightIntensity(elapsedTime);
			} else {
				secondaryInactive.initialise(currentIntensity);
				phase = 4;
				elapsedTime = 0;
			}
			break;
		case 5: 
			if (elapsedTime < secondaryInactive.totalPeriod) {
				intensity = secondaryInactive.lightIntensity(elapsedTime, true);
			} else {
				if (stateless) {
					becomingActive.initialise(currentIntensity);
					phase = 0;
				} else {
					primaryInactive.initialise(currentIntensity);
					phase = 4;
				}
				elapsedTime = 0;
			}
			break;
		}
		if (currentIntensity != intensity) {
			lightChanged = true;
			currentIntensity = intensity;
		}
	}
	public void activate() {
		active = false;
		toggle();
	}
	public void deActivate() {
		active = true;
		toggle();
	}

	public void toggle() {
		active = !active;
		if (active) {
			becomingActive.initialise(currentIntensity);
			phase = 0;
			elapsedTime = 0;
		} else {
			becomingInactive.initialise(currentIntensity);
			phase = 3;
			elapsedTime = 0;
		}

		foreach (ControlPanel cp in GlobalData.map.controlPanels) {
			if (cp.lightSwitch == id || cp.tagSwitch == mapTag) {
				cp.toggle();
			}
		}
	}

	public void lightMaterial (GameObject obj) {
		if (lightChanged) {
			MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
			if (meshRenderer != null) {
				Material material = meshRenderer.sharedMaterial;
				Color brightness = Color.white;
				brightness *= currentIntensity;
				material.SetColor ("_EmissionColor", brightness);

				//material.renderQueue = 2502;

				obj.GetComponent<MeshRenderer>().sharedMaterial = material;
			}
		}
	}

	public float intensity () {
		return currentIntensity;
	}

	

}

