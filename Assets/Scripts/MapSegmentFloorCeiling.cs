using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSegmentFloorCeiling  {
	public GameObject meshItem = null;
	public int lightID = -1;
	public mapLight light = null;
	public MapSegment conection = null;
	public int connectionID = -1;
	public float Opacity = 1;
	public bool solid = true;
	public bool transparent = false;
	public Material upperMaterial, lowerMaterial, middeMaterial;
	public Vector2 upperOffset, middleOffset, lowerOffset = new Vector2(0,0);
	public ControlPanel controlPanel = null;

	public void setLight() {
		if (light != null && meshItem != null  && meshItem.activeInHierarchy) {
			light.lightMaterial(meshItem);
		}
	}

}
