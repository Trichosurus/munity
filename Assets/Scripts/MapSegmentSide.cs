using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSegmentSide  {
	public MapSegment connection = null;
	public GameObject upperMeshItem = null;
	public GameObject middleMeshItem = null;
	public GameObject lowerMeshItem = null;
	public GameObject entryCollider = null;
	public mapLight upperLight, lowerLight, middleLight;
	public int connectionID = -1;
	public float Opacity = 1;
	public bool solid = true;
	public bool transparent = false;
	public Material upperMaterial, lowerMaterial, middeMaterial;
	public Vector2 upperOffset, middleOffset, lowerOffset = new Vector2(0,0);
	public ControlPanel controlPanel = null;

	public void setLight() {
		if (upperLight != null && upperMeshItem != null && upperMeshItem.activeInHierarchy ) {
			upperLight.lightMaterial(upperMeshItem);
		}
		if (lowerLight != null && lowerMeshItem != null && lowerMeshItem.activeInHierarchy) {
			lowerLight.lightMaterial(lowerMeshItem);
		}
		if (middleLight != null && middleMeshItem != null && middleMeshItem.activeInHierarchy ) {
			middleLight.lightMaterial(middleMeshItem);
		}
	}
}