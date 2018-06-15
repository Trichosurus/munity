using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapSegment : MonoBehaviour {
	public List<Vector3> vertices;
	public int id = -1;
	public Vector3 height;
	private int[] triangles;
	public PlatformObject platform = null;
	public Vector3 centerPoint;

	public List<MapSegmentSide> sides = new List<MapSegmentSide>();

	public MapSegmentFloorCeiling ceiling = new MapSegmentFloorCeiling();
	public MapSegmentFloorCeiling floor = new MapSegmentFloorCeiling();
	
	public List<impossibleVolume> impossibleVolumes;
	public Liquid liquid = null;
	public bool impossible = false;
	public List<int> collidesWith = new List<int>();
	public int viewEdge = -1;
	public bool hidden = false;
	public bool[] activePolygons;
	private GameObject visCheck;
	public int overDraw = GlobalData.occlusionOverDraw;
	
	private int activeCount = 0;
	private	GameObject viscalc;


	public bool itemImpassable = false;
	public bool monsterImpassable = false;
	public bool hill = false;
	public int mapBase = -1;
	public int lightOnTrigger = -1;
	public int platformOnTrigger = -1;
	public int lightOffTrigger = -1;
	public int platformOffTrigger = -1;
	public int teleporter = -1;
	public bool zoneBorder = false;
	public bool goal = false;
	public bool visibleMonsterTrigger = false;
	public bool invisibleMonsterTrigger = false;
	public bool dualMonsterTrigger = false;
	public bool itemTrigger = false;
	public bool mustBeExplored = false;
	public int automaticExit = -1;
	
	public int damage = 0;

	public bool glue;
	public bool glueTrigger;
	public bool superglue;
    




	// Use this for initialization
	void Start () {


	}

	void Update () {
		if (!hidden) {
			floor.setLight();
			ceiling.setLight();
			foreach (MapSegmentSide side in sides) {
				side.setLight();
			}

			if (liquid != null && liquid.volume != null) {

				float min = liquid.low;
				float max = liquid.high;
				Vector3 liquidHeight = new Vector3(
					liquid.volume.transform.position.x,
					centerPoint.y - (max - min) + ((max - min) * liquid.mediaLight.intensity()),
					liquid.volume.transform.position.z
				);

				liquid.volume.transform.position = liquidHeight;
			}
		}
	}


	public void triggerBehaviour() {
		if (platform != null && !platform.door) {
			platform.activate(-2);
		}
		if (platformOffTrigger >= 0 && GlobalData.map.segments[platformOffTrigger].platform != null) {
			GlobalData.map.segments[platformOffTrigger].platform.deActivate();
		}
		if (platformOnTrigger >= 0 && GlobalData.map.segments[platformOnTrigger].platform != null) {
			GlobalData.map.segments[platformOnTrigger].platform.activate();
		}
		if (lightOnTrigger >= 0 && GlobalData.map.lights[lightOnTrigger] != null) {
			if (!GlobalData.map.lights[lightOnTrigger].active) {
				GlobalData.map.lights[lightOnTrigger].toggle();
			}
		}
		if (lightOffTrigger >= 0 && GlobalData.map.lights[lightOffTrigger] != null) {
			if (GlobalData.map.lights[lightOffTrigger].active) {
				GlobalData.map.lights[lightOffTrigger].toggle();
			}
		}
		if (mustBeExplored) {
			mustBeExplored = false;
		}
		if (teleporter >= 0 && GlobalData.map.segments.Count > teleporter) {
			GameObject player =  GameObject.Find("player");
			if (player != null) {
				//Quaternion facing = Quaternion.Euler(0, (float)obj.Facing+90, 0);
				player.transform.position = GlobalData.map.segments[teleporter].centerPoint;
			}
		}

	}

	public void checkIfImpossible() {
		//if (impossible) {return;}
		RaycastHit hit;
		List<Vector3> verts = new List<Vector3>(vertices);
		verts.Add(new Vector3(0,0,0));
		foreach (Vector3 vert in verts) {
			Vector3 startPoint = gameObject.transform.TransformPoint(vert);

			startPoint = (startPoint-centerPoint)*0.95f + centerPoint  + (height*0.05f);
			Vector3 castPoint = gameObject.transform.TransformPoint(vert);
			// if (id == 47) {
			// 	Debug.Log(castPoint);
			// }
			float rayCount = 7f;

			if (Physics.Raycast(centerPoint + height*0.5f, (castPoint+height*0.5f)-(centerPoint + height*0.5f), out hit,Vector3.Distance(centerPoint + height*0.5f, castPoint+height*0.5f)*0.95f)) {
				if (hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.tag == "polygon") {
					if (hit.collider.transform.parent.GetComponent<MapSegment>() != null &&
						hit.collider.transform.parent.GetComponent<MapSegment>().id != id) {
						hit.collider.transform.parent.GetComponent<MapSegment>().impossible = true;
						impossible = true;
						if (!collidesWith.Contains(hit.collider.transform.parent.GetComponent<MapSegment>().id)) {
							collidesWith.Add(hit.collider.transform.parent.GetComponent<MapSegment>().id);
						}
						if (!hit.collider.transform.parent.GetComponent<MapSegment>().collidesWith.Contains(id)) {
							hit.collider.transform.parent.GetComponent<MapSegment>().collidesWith.Add(id);
						}

						Debug.Log(id);
						Debug.Log(hit.collider.transform.parent.GetComponent<MapSegment>().id);
						Debug.DrawRay(centerPoint + height*0.5f, (castPoint+height*0.5f)-(centerPoint + height*0.5f), Color.yellow);				
						continue;					

					}
				}
			}

			for (int i = 1; i <= rayCount; i++) {
				castPoint = (startPoint-centerPoint)*((1f/(rayCount-0.001f))*(float)i) + centerPoint  + (height*0.05f);

				if (Physics.Raycast(castPoint, Vector3.up, out hit, height.y)) {
					if (hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject != gameObject) {
						if (hit.collider.transform.parent.GetComponent<MapSegment>() != null) {
							hit.collider.transform.parent.GetComponent<MapSegment>().impossible = true;
						}
						impossible = true;
						Debug.DrawRay(castPoint, Vector3.up, Color.magenta);			

						if (!collidesWith.Contains(hit.collider.transform.parent.GetComponent<MapSegment>().id)) {
							collidesWith.Add(hit.collider.transform.parent.GetComponent<MapSegment>().id);
						}
						if (!hit.collider.transform.parent.GetComponent<MapSegment>().collidesWith.Contains(id)) {
							hit.collider.transform.parent.GetComponent<MapSegment>().collidesWith.Add(id);
						}
	
						continue;
					}
				}
				castPoint.y = centerPoint.y - (height.y*0.01f);
				if (Physics.Raycast(castPoint, Vector3.down, out hit,10)) {
					if (hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.tag == "polygon") {
						if (hit.collider.name != "ceiling") {
							hit.collider.transform.parent.GetComponent<MapSegment>().impossible = true;
							impossible = true;
							if (!collidesWith.Contains(hit.collider.transform.parent.GetComponent<MapSegment>().id)) {
								collidesWith.Add(hit.collider.transform.parent.GetComponent<MapSegment>().id);
							}
							if (!hit.collider.transform.parent.GetComponent<MapSegment>().collidesWith.Contains(id)) {
								hit.collider.transform.parent.GetComponent<MapSegment>().collidesWith.Add(id);
							}


							Debug.Log(id);
							Debug.DrawRay(castPoint, Vector3.down, Color.white);				
							continue;

						}
						
					}
				}
			}
		}
	}

	public void showHide(bool show = false) {
		if ( show == hidden ) {
			Component[] allChildren = gameObject.GetComponentsInChildren(typeof(Transform), true);
				if (id == 436) {
					;
				}
			foreach (Transform child in allChildren) {
				// if (child.gameObject.name == "floor" || child.gameObject.name == "ceiling" || child.gameObject.name == "wall" 
				// 		|| child.gameObject.name == "transparent" || child.gameObject.name == "polygonElement(Clone)"){
				if (child.gameObject.name != gameObject.name && child.gameObject.name != "upperPlatform" && child.gameObject.name != "lowerPlatform") {
					child.gameObject.SetActive(show);
				}
				// if (child.gameObject.name == "upperPlatform" || child.gameObject.name == "lowerPlatform") {
				// 	Component[] platChildren = child.gameObject.GetComponentsInChildren(typeof(Transform), true);
				// 	foreach (Transform plat in platChildren) {
				// 			plat.gameObject.SetActive(show);
				// 	}
				// }
			} 
			hidden = !show;
		}
	}

	public void setClippingPlanes(List<Vector3> planes,  bool additive = true) {
		//Vector3 plane1Position, plane1Rotation, plane2Position, plane2Rotation, plane3Position, plane3Rotation;
		Vector3 plane1Position = new Vector3(0,0,0);
		Vector3 plane1Rotation = new Vector3(0,0,0);
		Vector3 plane2Position = new Vector3(0,0,0);
		Vector3 plane2Rotation = new Vector3(0,0,0);
		Vector3 plane3Position = new Vector3(0,0,0);
		Vector3 plane3Rotation = new Vector3(0,0,0);

		if (planes.Count > 1) {
			plane1Position = planes[0];
    		plane1Rotation = planes[1];
		}
		if (planes.Count > 3) {
    		plane2Position = planes[2];
    		plane2Rotation = planes[3];
		}
		if (planes.Count > 5) {
    		plane3Position = planes[4];
    		plane3Rotation = planes[5];
		}

		Component[] allChildren = gameObject.GetComponentsInChildren(typeof(Transform), true);
		foreach (Transform child in allChildren) {
			MeshRenderer mr = child.gameObject.GetComponent<MeshRenderer>();
			if (mr != null) {
				Material mat = mr.sharedMaterial;
				mat.EnableKeyword("CLIP_ADDITIVE");
				if (additive) {
					mat.SetFloat("_planesAdditive", 1);
				} else {
					mat.SetFloat("_planesAdditive", 0);
				}
				if (planes.Count < 2) {
					mat.DisableKeyword("CLIP_ONE");
					mat.DisableKeyword("CLIP_TWO");
					mat.DisableKeyword("CLIP_THREE");
				}
				if (planes.Count == 2) { 
					mat.EnableKeyword("CLIP_ONE");
					mat.DisableKeyword("CLIP_TWO");
					mat.DisableKeyword("CLIP_THREE");
				
					mat.SetVector("_planePos", plane1Position);
					mat.SetVector("_planeNorm", Quaternion.Euler(plane1Rotation) * Vector3.up);

				}
				if (planes.Count == 4) { 
					mat.DisableKeyword("CLIP_ONE");
					mat.EnableKeyword("CLIP_TWO");
					mat.DisableKeyword("CLIP_THREE");

					mat.SetVector("_planePos", plane1Position);
					mat.SetVector("_planeNorm", Quaternion.Euler(plane1Rotation) * Vector3.up);

					mat.SetVector("_planePos2", plane2Position);
					mat.SetVector("_planeNorm2", Quaternion.Euler(plane2Rotation) * Vector3.up);

				}
				if (planes.Count == 6) { 
					mat.DisableKeyword("CLIP_ONE");
					mat.DisableKeyword("CLIP_TWO");
					mat.EnableKeyword("CLIP_THREE");

					mat.SetVector("_planePos", plane1Position);
					mat.SetVector("_planeNorm", Quaternion.Euler(plane1Rotation) * Vector3.up);

					mat.SetVector("_planePos2", plane2Position);
					mat.SetVector("_planeNorm2", Quaternion.Euler(plane2Rotation) * Vector3.up);

					mat.SetVector("_planePos3", plane3Position);
					mat.SetVector("_planeNorm3", Quaternion.Euler(plane3Rotation) * Vector3.up);
				}
			}
		}
	}


	public void calculatePoints() {
		float yIndex = centerPoint.y;

		centerPoint = calculateCenterPoint(vertices);

		for (int i = 0; i < vertices.Count; i++) {
			vertices[i] -= centerPoint;
		}

		centerPoint.y = yIndex;
		gameObject.transform.position = centerPoint;

	}

	public void recalculatePlatformVolume (){
		//platfroms will be converted into proper objects moving in a polygon
		//so we need to make sure that the polygon heights match what is pretending to go on 
		//in the map, rather thatn what actually is.
		//the floor should be the lowest connecting polygon and the ceiling should 
		//be the highest connecting pollygon if the platform comes from the 
		//floor/ceiling respectively
		foreach (MapSegmentSide s in sides) {
			if (s.connection == null && s.connectionID >= 0) {
				s.connection = GlobalData.map.segments[s.connectionID];
			}
			if (s.connection != null){
				MapSegment conn = s.connection;

				if (conn.centerPoint.y < centerPoint.y 
							&& platform.comesFromFloor) {
					float heightAdjust = (centerPoint.y - conn.centerPoint.y);
					height.y += heightAdjust;
					foreach (MapSegmentSide side in sides) {
						if (side.connectionID == -1) {
						side.upperOffset.y -= heightAdjust;
						side.middleOffset.y -= heightAdjust;
						side.lowerOffset.y -= heightAdjust;
						}
					}
					centerPoint.y = conn.centerPoint.y;
				}
				if (conn.centerPoint.y + conn.height.y > centerPoint.y + height.y
							&& platform.comesFromCeiling) {
					float heightAdjust = (conn.height.y - conn.centerPoint.y) - (height.y - centerPoint.y);
					height.y += heightAdjust;
					foreach (MapSegmentSide side in sides) {
						if (side.connectionID == -1) {
						side.upperOffset.y -= heightAdjust;
						side.middleOffset.y -= heightAdjust;
						side.lowerOffset.y -= heightAdjust;
						}
					}
				}
			}
		}
		gameObject.transform.position = centerPoint;
	}

	public void makePlatformObjects() {
		GameObject part = null;
		Vector3 pHeight = height;
		Vector3 splitPoint = new Vector3(height.x, 
										platform.minimumHeight + (platform.maximumHeight - platform.minimumHeight)/2,
										height.z); 
		splitPoint.y -= gameObject.transform.position.y;
		List<Vector3> PlatVertices = new List<Vector3>(vertices);
		PlatVertices.Reverse();
		bool split = platform.comesFromFloor && platform.comesFromCeiling;
		if (split) {
			pHeight.y = splitPoint.y;
		}
		if (platform.comesFromFloor) {
			platform.lowerBottom = new MapSegmentFloorCeiling();
			platform.lowerTop = new MapSegmentFloorCeiling();
			platform.lowerPlatform =  new GameObject("lowerPlatform");
			platform.lowerPlatform.transform.parent = gameObject.transform;
			part = makePolygon(true, PlatVertices, pHeight, platform.lowerPlatform, ceiling.upperMaterial, ceiling.upperOffset);
			if (part != null) {
				part.name = "platBottom";
				platform.lowerBottom.meshItem = part;
				platform.lowerBottom.light = ceiling.light;
				platform.lowerBottom.lightID = ceiling.lightID;
			}
			part = makePolygon(false, PlatVertices, pHeight, platform.lowerPlatform, floor.upperMaterial, floor.upperOffset);
			if (part != null) {
				part.name = "platTop";
				platform.lowerTop.meshItem = part;
				platform.lowerTop.light = floor.light;
				platform.lowerTop.lightID = floor.lightID;
			}
		}
		if (split) {
			pHeight.y = height.y - (splitPoint.y);
		}

		if (platform.comesFromCeiling) {
			platform.upperBottom = new MapSegmentFloorCeiling();
			platform.upperTop = new MapSegmentFloorCeiling();

			platform.upperPlatform =  new GameObject("upperPlatform");
			platform.upperPlatform.transform.parent = gameObject.transform;

			part = makePolygon(true, PlatVertices, pHeight, platform.upperPlatform, ceiling.upperMaterial, ceiling.upperOffset);
			if (part != null) {
				part.name = "platBottom";
				platform.upperBottom.meshItem = part;
				platform.upperBottom.light = ceiling.light;
				platform.upperBottom.lightID = ceiling.lightID;
			}
			part = makePolygon(false, PlatVertices, pHeight, platform.upperPlatform, floor.upperMaterial, floor.upperOffset);
			if (part != null) {
				part.name = "platTop";
				platform.upperTop.meshItem = part;
				platform.upperTop.light = floor.light;
				platform.upperTop.lightID = floor.lightID;
			}
		}

		PlatVertices.Reverse();

		for (int i = 0; i < PlatVertices.Count; i++) {
			Vector3 point1, point2;
			point2 = PlatVertices[i];
			if (i+1 < PlatVertices.Count) {
				point1 = PlatVertices[i+1];
			} else {
				point1 = PlatVertices[0];
			}
			MapSegmentSide wall = new MapSegmentSide();
			if (sides.Count > i) {wall = sides[i];}
			if (wall.connection == null && wall.connectionID >= 0) {
				wall.connection = GlobalData.map.segments[wall.connectionID];
			}
				
			
			if (platform.comesFromFloor) {
				if (split) {
					part = addWallPart(point1, point2, splitPoint, new Vector3(0,0,0), wall.lowerMaterial, wall.lowerOffset, platform.lowerPlatform);
				} else {
					part = addWallPart(point1, point2, pHeight, new Vector3(0,0,0), wall.lowerMaterial, wall.lowerOffset, platform.lowerPlatform);
				}
				if (part != null) {
					MapSegmentSide side = new MapSegmentSide();
					part.name = "platSide";
					side.upperLight = wall.lowerLight;
					side.upperMeshItem = part;
					platform.lowerSides.Add(side);
				}
			}
			if (platform.comesFromCeiling) {
				part = addWallPart(point1, point2, pHeight, new Vector3(0,0,0), wall.upperMaterial, wall.upperOffset, platform.upperPlatform);
				if (part != null) {
					MapSegmentSide side = new MapSegmentSide();
					side.upperLight = wall.upperLight;
					part.name = "platSide";
					side.upperMeshItem = part;
					platform.upperSides.Add(side);
				}

			}
		}

		if (platform.upperPlatform != null) {
			platform.upperPlatform.transform.position = gameObject.transform.position;
			platform.upperPlatform.AddComponent<platformController>();
			generateColliders(platform.upperPlatform);
		}
		if (platform.lowerPlatform != null) {
			platform.lowerPlatform.transform.position = gameObject.transform.position;
			platform.lowerPlatform.AddComponent<platformController>();
			generateColliders(platform.lowerPlatform);

		}

		if (split) {
			platform.upperPlatform.transform.position += splitPoint;
		}


	}

	public void generateMeshes() {

					

		floor.meshItem = makePolygon(true, vertices, height, gameObject);
		ceiling.meshItem = makePolygon(false, vertices, height, gameObject);

		// if (id == 555) {
		// 	Debug.Log(centerPoint);
		// }

		for (int i = 0; i < vertices.Count; i++) {
			makeWall(i);
		}
		generateLiquidVolumes();
		generateColliders(gameObject);

	}

	public void generateColliders (GameObject obj){
		foreach(Transform child in obj.transform) {
			// if (child.gameObject.name == "floor" ||child.gameObject.name == "ceiling" || child.gameObject.name == "wall" || child.gameObject.name == "upperWall" || child.gameObject.name == "lowerWall" || child.gameObject.name == "middleWall" ||
			// 		child.gameObject.name == "transparent" || child.gameObject.name == "polygonElement(Clone)"){
			MeshFilter mf = child.GetComponent<MeshFilter>();
			if (mf != null ) {
				MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
				mc.sharedMesh = mf.mesh;
				Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
				rb.useGravity = false;	
				rb.isKinematic = true;

						
			}
		}
	}

	public void generateLiquidVolumes (){
		if (liquid != null) {
			liquid.volume = new GameObject("liquid");
			liquid.volume.transform.parent = gameObject.transform;
			List<Vector3> liquidVertices = new List<Vector3>(vertices);
			liquidVertices.Reverse();
			Vector3 liquidHeight = new Vector3(0,
												liquid.high - centerPoint.y,
												0);
			makePolygon(true, liquidVertices, liquidHeight, liquid.volume, liquid.surface);
			makePolygon(false, liquidVertices, liquidHeight, liquid.volume, liquid.surface);

			for (int i = 0; i < vertices.Count; i++) {

				Vector3 point1, point2;
				point2 = liquidVertices[i];
				if (i+1 < liquidVertices.Count) {
					point1 = liquidVertices[i+1];
				} else {
					point1 = liquidVertices[0];
				}
				//MapSegmentSide wall = new MapSegmentSide();
				// addWallPart(point1, point2, liquidHeight, 
				// 			new Vector3(0,0,0), liquid.surface, 
				// 			new Vector2(0,0), liquid.volume);
			}
			liquid.volume.transform.position = gameObject.transform.position;
		}

	}


	Vector3 calculateCenterPoint ( List<Vector3> points) {
		Vector3 center = new Vector3(0,0,0);
		for (int i = 0; i < points.Count; i++) {
			center += points[i];
		}
		center /= points.Count;
		return center;
	}

	GameObject makePolygon(bool isBase, List<Vector3> vertices, Vector3 polHeight, GameObject parent, Material mat = null, Vector2 matOffset = default(Vector2)) {
		List<Vector3> _vertices = new List<Vector3>(vertices);
		GameObject meshItem = Instantiate(Resources.Load<GameObject>("polygonElement"), parent.transform.position, parent.transform.rotation);
		meshItem.transform.parent = parent.transform;
		MeshFilter meshfilter = meshItem.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
        meshfilter.mesh = mesh;
		//matOffset = new Vector2(0,0);
		//Material mat = Resources.Load("stripes") as Material;
		//Material mat = GameObject.Find("gameController").GetComponent<Map>().materials[1];
		//meshItem.GetComponent<MeshRenderer>().material = mat;
		// Material mat;
		if (mat == null) {
			if (isBase) {
				mat = floor.upperMaterial;
				matOffset = floor.upperOffset + new Vector2(centerPoint.z, centerPoint.x);
			} else {
				mat = ceiling.upperMaterial;
				matOffset = ceiling.upperOffset + new Vector2(centerPoint.z, centerPoint.x);
			}
		}
		if (mat == null) {mat = Resources.Load("Materials/texture") as Material;}
		meshItem.GetComponent<MeshRenderer>().material = mat;
		meshItem.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", matOffset);

		if (!isBase) {
			_vertices.Reverse();
			if (parent.GetComponent<MapSegment>()!= null && parent.GetComponent<MapSegment>().ceiling != null) {meshItem.name = "ceiling";}
		} else {
			if (parent.GetComponent<MapSegment>()!= null && parent.GetComponent<MapSegment>().floor != null) {meshItem.name = "floor";}
		}

		_vertices.Insert(0,new Vector3(0,0,0));
		Vector2[] uvs = new Vector2[_vertices.Count];
		List<int> triangles = new List<int>();
		for (int i = 0; i < _vertices.Count; i++) {
			if (!isBase) {_vertices[i] += polHeight;}
			triangles.Add(0);
			triangles.Add(i);
			if (i+1 < _vertices.Count){
				triangles.Add(i+1);
			} else {
				triangles.Add(1);
			}
			uvs[i] = new Vector2(_vertices[i].z, _vertices[i].x);

		}
		mesh.vertices = _vertices.ToArray();
		mesh.uv = uvs;
		mesh.triangles = triangles.ToArray();	

		mesh.RecalculateNormals();

		return meshItem;

	}

	public void makeWall(int side) {

		Vector3 point1, point2;
		point1 = vertices[side];
		if (side+1 < vertices.Count) {
			point2 = vertices[side+1];
		} else {
			point2 = vertices[0];
		}
		MapSegmentSide wall = new MapSegmentSide();
		if (sides.Count > side) {wall = sides[side];}

		if (wall.connection == null && wall.connectionID >= 0) {
			wall.connection = GlobalData.map.segments[wall.connectionID];
		}

		Vector3 wallHeightUpper = new Vector3(0,0,0);
		Vector3 wallHeightLower = new Vector3(0,0,0);
		Vector3 wallOffset = new Vector3(0, 0, 0);
		GameObject wallPart = null;

		if (wall.connection != null && 
				(
				wall.transparent == true || 
				wall.connection.platform != null ||
				platform != null ||
				wall.solid == true  
				)
			){

			bool connTop = (wall.connection.platform != null &&
							!wall.connection.platform.comesFromCeiling) ||
							 wall.connection.platform == null || wall.solid; 
			bool connBottom = (wall.connection.platform != null &&
							!wall.connection.platform.comesFromFloor)||
							wall.connection.platform == null || wall.solid; 

			connBottom = (wall.connection.transform.position.y > gameObject.transform.position.y 
				&& connBottom);
			connTop = (wall.connection.transform.position.y+wall.connection.height.y < gameObject.transform.position.y+height.y
				&& connTop);

			//occasionally a secondary texture may be defined in the side when there is no secondary wall
			//check if this is the case and use primary instead if necessary
			if (connBottom && !connTop) {
				wall.lowerMaterial = wall.upperMaterial;
				wall.lowerLight = wall.upperLight;
				wall.lowerOffset = wall.upperOffset;
			}

			if (connBottom) {
				if (wall.solid && wall.lowerMaterial == null ) {
					wall.lowerMaterial = Resources.Load<Material>("Materials/transparent");
				}
				wallHeightLower = new Vector3(height.x, height.y, height.z);
				wallHeightLower.y = wall.connection.transform.position.y - gameObject.transform.position.y;
				wallPart = addWallPart(point1, point2, wallHeightLower, wallOffset, wall.lowerMaterial, wall.lowerOffset, gameObject);
				if (wallPart != null) {
					wallPart.name = "lowerWall";
					sides[side].lowerMeshItem = wallPart;
				}
				if (!GlobalData.skipOcclusion) {
					if (wall.solid && wall.lowerMaterial.name == "transparent" || 
							(wall.transparent && connBottom && !connTop)) {
						wallPart.SetActive(false);
					}
				}
			}

			if (connTop) {
				if (wall.solid && wall.upperMaterial == null ) {
					wall.upperMaterial = Resources.Load<Material>("Materials/transparent");
				}
				wallHeightUpper = new Vector3(height.x, height.y, height.z);
				wallOffset = wall.connection.height + wall.connection.transform.position - gameObject.transform.position;
				wallOffset.x = 0;
				wallOffset.z = 0;
				wallHeightUpper.y = height.y - wallOffset.y;
				wallPart = addWallPart(point1, point2, wallHeightUpper, wallOffset, wall.upperMaterial, wall.upperOffset, gameObject);
				if (wallPart != null) {
					wallPart.name = "upperWall";
					sides[side].upperMeshItem = wallPart;
				
					if (!GlobalData.skipOcclusion) {
						if (wall.solid && wall.upperMaterial.name == "transparent" || 
							(wall.transparent && connTop && !connBottom)) {
							wallPart.SetActive(false);
						}
					}
				}
			}

			Vector3 wallHeightMiddle = height - wallHeightLower - wallHeightUpper;
			if ( wallHeightMiddle.y>0) {
				if (wall.solid && wall.middeMaterial == null ) {
					wall.middeMaterial = Resources.Load<Material>("Materials/transparent");
				}

				wallPart = addWallPart(point1, point2, 
							wallHeightMiddle,
							new Vector3(0,wallHeightLower.y,0), 
							wall.middeMaterial, wall.middleOffset, gameObject);
				
				if (wallPart != null) {
					wallPart.name = "middleWall";
					sides[side].middleMeshItem = wallPart;
				

					if (!GlobalData.skipOcclusion) {
						if (wall.solid && wall.middeMaterial.name == "transparent" || 
								(wall.transparent && !connTop && !connBottom)) {
							wallPart.SetActive(false);
						}
					}
				}
				
			}
		} else {
			if (wall.connectionID == -1){
				if (wall.upperMaterial == null) { wall.upperMaterial = Resources.Load<Material>("Materials/texture");}
				wallPart = addWallPart(point1, point2, height, wallOffset, wall.upperMaterial, wall.upperOffset, gameObject);
				wallPart.name = "wall";
				sides[side].upperMeshItem = wallPart;

			}
		}

		// if (wallPart != null) {
		// 	sides[side].upperMeshItem = wallPart;
		// 	wallPart.name = "wall";
		// }
	}


	GameObject addWallPart(Vector3 point1, Vector3 point2, Vector3 wallHeight, Vector3 offset, Material material, Vector2 matOffset, GameObject parent){
		if (material == null) {return null;}
		GameObject meshItem = Instantiate(Resources.Load<GameObject>("polygonElement"), parent.transform.position, parent.transform.rotation);
		meshItem.transform.parent = parent.transform;
		MeshFilter meshfilter = meshItem.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();

        meshfilter.mesh = mesh;

		List<Vector3> meshVerts = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2> ();
		List<int> triangles = new List<int>();


		meshVerts.Add(offset + point1);
		meshVerts.Add(offset + point1 + wallHeight);
		meshVerts.Add(offset + point2 + wallHeight);
		meshVerts.Add(offset + point2);

		uvs.Add(new Vector2( 0,  0));
		uvs.Add(new Vector2( 0,wallHeight.y));
		uvs.Add(new Vector2( Vector3.Distance(point1 + wallHeight, point2 + wallHeight), wallHeight.y));
		uvs.Add(new Vector2( Vector3.Distance(point1, point2), 0));

		int vl = meshVerts.Count-4;

		triangles.Add(0+vl);
		triangles.Add(1+vl);
		triangles.Add(2+vl);

		triangles.Add(0+vl);
		triangles.Add(2+vl);
		triangles.Add(3+vl);

		mesh.vertices = meshVerts.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = triangles.ToArray();	
		mesh.RecalculateNormals();

		if (material != null) {
			meshItem.GetComponent<MeshRenderer>().material = material;
			matOffset = new Vector2(matOffset.x, 0-(wallHeight.y+ matOffset.y));
			meshItem.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", matOffset);

		} else {
			meshItem.GetComponent<MeshRenderer>().material = Resources.Load<Material>("texture");
			// meshItem.name = "transparent";
			//meshItem.GetComponent<MeshRenderer>().enabled = false;
		}

		return meshItem;
	}


	public bool playerTouch(GameObject element) {
		bool touched = false;
		foreach (MapSegmentSide s in sides) {
			if (s.controlPanel != null && (s.upperMeshItem == element || s.middleMeshItem == element || s.lowerMeshItem == element)) {
		    	s.controlPanel.toggle(element, true);
				touched = true;
			}
		}
		if (platform != null) {
			Debug.Log("platform?");
			platform.playerTouch();
			touched = true;
		}
		return touched;
	}


	public void calculateVisibility() {
		if (id == 489) {
			;
		}

		if (activePolygons.Length == 0) {
			activePolygons = new bool[GlobalData.map.segments.Count];
		}

		if (GlobalData.skipOcclusion) {
			for (int i = 0; i < activePolygons.Length; i++) {
				activePolygons[i] = true;
			}
			return;
		}

		viscalc = GameObject.CreatePrimitive(PrimitiveType.Cube);
		viscalc.transform.parent = transform;
		viscalc.transform.position = transform.position + (height/2);
		viscalc.transform.localScale=new Vector3(0.05f,0.05f,0.05f);


	// Debug.Log("----------------------------------------");
		
		bool[] ap = new bool[GlobalData.map.segments.Count];
		for (int i = 0; i< ap.Length; i++) {
			ap[i] = activePolygons[i];
			GlobalData.map.segments[i].viewEdge = 0;
			if (GlobalData.map.segments[i].impossible) {
				GlobalData.map.segments[i].showHide(false);
			}
		}
		activePolygons = new bool[GlobalData.map.segments.Count];

		bool[] processedPolys = new bool[GlobalData.map.segments.Count];
		activeCount = 0;
		int processedCount = 0;
		float[] distances = new float[GlobalData.map.segments.Count];

 
		activePolygons[id] = true;activeCount++;
		foreach(MapSegmentSide side in GlobalData.map.segments[id].sides) {
			if (side.connectionID != -1) {
					activePolygons[side.connectionID] = true;activeCount++;
					if (GlobalData.map.segments[side.connectionID].activePolygons.Length == 0) {
						GlobalData.map.segments[side.connectionID].activePolygons = new bool[GlobalData.map.segments.Count];
					}
					GlobalData.map.segments[side.connectionID].activePolygons[id] = true;
					activeCount += addToPolygonList(side.connectionID);
					GlobalData.map.segments[id].viewEdge = 1;
			}
		}
		processedPolys[id] = true;processedCount++;
		
		while (processedCount < activeCount) {
			float distance = 7777777;
			int closest = -1;
			//int connections = 0;
			for (int i = 0; i < activePolygons.Length; i++) {
				if (activePolygons[i] && !processedPolys[i]){
					if (distances[i] == 0 ) {
						distances[i] = Vector3.Distance(gameObject.transform.position, GlobalData.map.segments[i].transform.position);
					}
					if (distances[i] < distance) {
						distance = distances[i];
						closest = i;
					}
				}
			}

		// if (id == 46 && closest == 31) {
		// 	processedCount = processedCount;
		// }


			if (closest >=0){
				activeCount += addToPolygonList(closest);
				processedPolys[closest] = true; 
			}
			processedCount++;

		}
		for (int i = 0; i< ap.Length; i++) {
			if (ap[i]) {activePolygons[i] = true;}
		}

		Destroy(viscalc);
	}

	int addToPolygonList (int PolygonID) {
		if (PolygonID < 0) {return 0;}
		
		bool isVisible;
		int addCount = 0;
		//activePolygons[PolygonID] = true; activePolygons++;
		int connCount = 0;
		// if (activePolygons.Count < 100) {
		MapSegment seg = GlobalData.map.segments[PolygonID];
		if (seg.viewEdge < 0) {seg.viewEdge = 0;}
		for( int s = 0; s < seg.sides.Count; s++) {
			MapSegmentSide side = GlobalData.map.segments[PolygonID].sides[s];
			if (side.connectionID >= 0 ) {
				bool backlink = activePolygons[side.connectionID];
				if (!backlink) {
					connCount++;
				}

				Vector3 point1, point2;
				point1 = seg.vertices[s];
				if (s+1 < seg.vertices.Count) {
					point2 = seg.vertices[s+1];
				} else {
					point2 = seg.vertices[0];
				}
				point1 = GlobalData.map.segments[PolygonID].transform.TransformPoint(point1);
				point2 = GlobalData.map.segments[PolygonID].transform.TransformPoint(point2);
				
				isVisible = getRectVisibility(point1, point2, seg.height);

				if (id == 489) {
					if (PolygonID == 465 || seg.id == 467) {
					;
					}				

				}
				if (isVisible) {
					if (!backlink) {
						activePolygons[side.connectionID] = true; 
						//activeCount++;
						addCount++;
						if (GlobalData.map.segments[side.connectionID].activePolygons.Length == 0) {
							GlobalData.map.segments[side.connectionID].activePolygons = new bool[GlobalData.map.segments.Count];
						}
						GlobalData.map.segments[side.connectionID].activePolygons[id] = true;

						seg.viewEdge = 0;
						GlobalData.map.segments[side.connectionID].viewEdge = 1;
					} else {
						GlobalData.map.segments[side.connectionID].viewEdge = 0;
						GlobalData.map.segments[side.connectionID].viewEdge = 0;
						//seg.viewEdge = 0;
					}
				} else if (seg.viewEdge < overDraw) {
					if (!backlink) {
						activePolygons[side.connectionID] = true; 
						//activeCount++;
						addCount++;
						if (GlobalData.map.segments[side.connectionID].activePolygons.Length == 0) {
							GlobalData.map.segments[side.connectionID].activePolygons = new bool[GlobalData.map.segments.Count];
						}
						GlobalData.map.segments[side.connectionID].activePolygons[id] = true;
						GlobalData.map.segments[side.connectionID].viewEdge = seg.viewEdge + 1;
					}
				}
			}
		}
		if (connCount == 0) {
			seg.viewEdge = 1;
		}
		return addCount;
	}


	bool getRectVisibility (Vector3 point1, Vector3 point2, Vector3 rectHeight) {
		bool isVisible = false;
		RaycastHit hit;
		Vector3[] points = new Vector3[8];
		Vector3 p1, p2, p3, h1, h2, h3;
		h1 = rectHeight*0.005f;
		h2 = rectHeight*0.995f;
		h3 = rectHeight*0.5f;
		p1 = (point1-point2)*0.005f;
		p2 = (point1-point2)*0.995f;
		p3 = (point1-point2)*0.5f;
		

		Vector3 midpoint = p3 + point2 + h3;
		//get points just inside rectangle to prevent colliding with corner of adjacent polygons
		points[0] = p1 + point2  + h1;
		points[1] = p2 + point2  + h1;
		points[2] = p1 + point2  + h2;
		points[3] = p2 + point2  + h2;
		points[4] = p3 + point2  + h2;
		points[5] = p3 + point2  + h1;
		points[6] = p1 + point2  + h3;
		points[7] = p2 + point2  + h3;


		Vector3 castPoint, crossPoint;
		
		//float rayCount = 4f;
		float heightCount = height.y/GlobalData.occlusionDensity + 1;
		float crossCount;

		isVisible = false;

		List<Vector3> checkpoints = new List<Vector3>();
		checkpoints.Add(centerPoint + (height/2));
		foreach(Vector3 v in vertices) {
			crossCount = Vector3.Distance(gameObject.transform.TransformPoint(v), centerPoint)/GlobalData.occlusionDensity + 1;
			for (int c = 1; c <=crossCount; c++) {
				crossPoint = ((gameObject.transform.TransformPoint(v)-centerPoint)*((1f/crossCount)*(float)c) + centerPoint);
				for (int h = 1; h <=heightCount; h++) {
					checkpoints.Add(crossPoint + ((height*0.95f)*(1f/heightCount))*h);



				}
			}
		}
		//Color colour = Random.ColorHSV();
		//if (id > 2) {return false;}
		// if (id == 489) {
		// 	float xAmt = Vector3.Distance(points[0], points[1])/GlobalData.occlusionDensity;
		// 	float yAmt = Vector3.Distance(points[2], points[0])/GlobalData.occlusionDensity;

		// 	GameObject casmPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		// 	casmPoint.transform.parent = transform;
		// 	casmPoint.transform.position = midpoint;
		// 	casmPoint.transform.localScale=new Vector3(0.05f,0.05f,0.05f);
		// 	for (int x = 0; x <= Vector3.Distance(point1, point2)/GlobalData.occlusionDensity; x++) {
		// 		for (int y = 0; y <= rectHeight.y/GlobalData.occlusionDensity; y++) {
		// 			castPoint = (points[0] + 
		// 			((points[1] - points[0])/xAmt)*x + 
		// 			((points[2] - points[0])/yAmt)*y);
		// 			GameObject casPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		// 			casPoint.transform.parent = transform;
		// 			casPoint.transform.position = castPoint;
		// 			casPoint.transform.localScale=new Vector3(0.05f,0.05f,0.05f);
		// 		}
		// 	}
		// }



		foreach(Vector3 cp in checkpoints) {
					// if (id == 489) {
					// 	GameObject dispPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
					// 	dispPoint.transform.parent = transform;
					// 	dispPoint.transform.position = cp;
					// 	dispPoint.transform.localScale=new Vector3(0.04f,0.04f,0.04f);
					// }
			viscalc.transform.position = cp;
			viscalc.name = "viscalc";
			isVisible = (Physics.Raycast(midpoint, viscalc.transform.position-midpoint, out hit, 50)) 
				&& (hit.collider.gameObject == viscalc || hit.collider.gameObject.name == "viscalc") ;
			if (isVisible) {
				return true;
			}
			//for (int p = 0; p < points.Length && !isVisible; p++){
			float xAmt = Vector3.Distance(points[0], points[1])/GlobalData.occlusionDensity;
			float yAmt = Vector3.Distance(points[2], points[0])/GlobalData.occlusionDensity;
			for (int x = 0; x <= xAmt + 1; x++) {
				for (int y = 0; y <= yAmt + 1; y++) {

					Vector3 xpt = ((points[1] - points[0])/xAmt)*x;
					Vector3 ypt = ((points[2] - points[0])/yAmt)*y;
					if (x > xAmt) {xpt = ((points[1] - points[0])/xAmt)*xAmt;}
					if (y > yAmt) {ypt = ((points[2] - points[0])/yAmt)*yAmt;}
					castPoint = points[0] + xpt + ypt;

					if (id == 489){
					if (castPoint.x > 3 && castPoint.x < 4 &&
						castPoint.y > -5 && castPoint.y < -4 &&
						castPoint.z > 8 && castPoint.z < 9) {
						if (cp.x > 9 && cp.x < 10 )
							if (cp.y > -6 && cp.y < -5 )
								if (cp.z > 2 && cp.z < 3
								) {
							isVisible = (Physics.Raycast(castPoint, viscalc.transform.position-castPoint, out hit, 50)) 
								&& hit.collider.gameObject == viscalc ;
								}
						}
					}

					isVisible = (Physics.Raycast(castPoint, viscalc.transform.position-castPoint, out hit, 50)) 
								&& hit.collider.gameObject == viscalc ;

					if (id == 0 && !isVisible) {
					Debug.DrawRay(castPoint, viscalc.transform.position-castPoint, Color.blue, 30);
					}
					if (isVisible) {
						if (id == 489) {
						Debug.DrawRay(castPoint, viscalc.transform.position-castPoint, Color.green ,30);
						}
						return true;
					}
				}
			}
		}

		return isVisible;

	}

}

public class impossibleVolume {
	public List<Vector3[]> sidesSelf = new List<Vector3[]>();
	public List<Vector3[]> sidesOther = new List<Vector3[]>();
	public List<Vector3> collisionPoints = new List<Vector3>();
	public List<int> collisionPolygonsOther = new List<int>();
	public List<int> collisionPolygonsSelf = new List<int>();

	public void assembleVolumeSides (MapSegment seg, bool self = true) {
		foreach (impossibleVolume iv in seg.impossibleVolumes) {
			List<int> segList;
			List<Vector3[]> lineList = new List<Vector3[]>();
			if (self) {
				segList = iv.collisionPolygonsSelf;
			} else {
				segList = iv.collisionPolygonsOther;
			}
			Vector3[] currentPoints = new Vector3[2];
			int[] currentLine = new int[2];
			currentLine[0] = -1;
			currentLine[1] = -1;

			for (int i = 0; i < segList.Count; i++){
			for (int s = 0; s < GlobalData.map.segments[segList[i]].sides.Count; s++) {
				if (sideValid(segList[i], s, segList)) {
					currentLine[0] = segList[i];
					currentLine[1] = s;
					break;
				}
			}
			if (currentLine[0] >=0) {break;}
			}
			currentPoints = getLinePoints(GlobalData.map.segments[currentLine[0]], currentLine[1]);
			lineList.Add(currentPoints);
			currentPoints = new Vector3[2];
			currentLine[1]++;
			int count = 0;
			while (currentPoints[0] != lineList[0][0] || currentPoints[1] != lineList[0][1] && count < 666) {
				if (currentLine[0] == 63) {
					;
				}

				if (!sideValid(currentLine[0], currentLine[1], segList)) {
					int connID = -1;
					if (GlobalData.map.segments[currentLine[0]].sides.Count <= currentLine[1]) {
						connID = GlobalData.map.segments[currentLine[0]].sides[0].connectionID;
					} else {
						connID = GlobalData.map.segments[currentLine[0]].sides[currentLine[1]].connectionID;
					}
					currentLine[1] = getConnectingLine(currentLine[0], currentLine[1], connID);
					currentLine[0] = connID;
				}
				
				currentPoints = getLinePoints(GlobalData.map.segments[currentLine[0]], currentLine[1]);
				if (currentPoints[0] == lineList[0][0] && currentPoints[1] == lineList[0][1]) {
					break;
				}
				lineList.Add(currentPoints);

				currentLine[1]++;
				if (GlobalData.map.segments[currentLine[0]].sides.Count <= currentLine[1]) {
					currentLine[1] = 0;
				}
				currentPoints = getLinePoints(GlobalData.map.segments[currentLine[0]], currentLine[1]);
				count++;
			}
			if (self) {
				iv.sidesSelf = lineList;
			} else {
				iv.sidesOther = lineList;
			}
		}
	}

	int getConnectingLine(int segID, int sideNo, int connID) {
		MapSegment seg = GlobalData.map.segments[segID];
		MapSegment conn = GlobalData.map.segments[connID];

		Vector3 point = new Vector3();
		point = seg.transform.TransformPoint(seg.vertices[sideNo]);
		// if (sideNo < seg.sides.Count-1) {
		// 	point = seg.transform.TransformPoint(seg.vertices[sideNo+1]);
		// } else {
		// 	point = seg.transform.TransformPoint(seg.vertices[0]);
		// }
		int matchPoint = -1;
		for (int s = 0; s < conn.sides.Count; s++) {
			Vector3 connPoint = conn.transform.TransformPoint(conn.vertices[s]);
			if (conn.transform.TransformPoint(conn.vertices[s]) == point) {
				matchPoint = s;
			}
		}
		
		if (conn.sides[matchPoint].connectionID != segID) {
			return matchPoint;
		}
		matchPoint++;
		if (matchPoint >= conn.sides.Count) {matchPoint = 0;}
		return matchPoint;

	}

	bool sideValid(int segID, int sideNo, List<int> segList) {
		if (GlobalData.map.segments[segID].sides.Count <= sideNo) {
			sideNo = sideNo - GlobalData.map.segments[segID].sides.Count;
		}
		return (GlobalData.map.segments[segID].sides[sideNo].connectionID < 0 ||
				!segList.Contains(GlobalData.map.segments[segID].sides[sideNo].connectionID));
	}

	Vector3[] getLinePoints(MapSegment seg, int sideNo) {
		Vector3[] points = new Vector3[2]; 
		points[0] = seg.transform.TransformPoint(seg.vertices[sideNo]);
		if (sideNo < seg.vertices.Count-1) {
			points[1] = seg.transform.TransformPoint(seg.vertices[sideNo+1]);
		} else {
			points[1] = seg.transform.TransformPoint(seg.vertices[0]);
		}
		return points;
	}

	public List<int> getConnectedVolumes(MapSegment seg, bool self, List<int> connected = null) {
		if (connected == null) {connected = new List<int>();}
		if (self) { 
			if (!connected.Contains(seg.id)) {connected.Add(seg.id);}
			foreach(MapSegmentSide side in seg.sides) {
				if (side.connection != null && side.connection.impossible && !connected.Contains(side.connectionID)) {
					foreach(int i in side.connection.collidesWith) {
						if (seg.collidesWith.Contains(i)) {
							connected = getConnectedVolumes(side.connection, true, connected);
							break;
						}
					}
				}
			}
		//connected.Sort();
		} else {
		// int volCount = 0;
			seg.impossibleVolumes[0].collisionPolygonsOther = GlobalData.map.segments[seg.collidesWith[0]].impossibleVolumes[0].collisionPolygonsSelf;
			foreach (int i in seg.collidesWith) {
				bool match = false;
				foreach(impossibleVolume iv in seg.impossibleVolumes) {
					HashSet<int> hsSelf = new HashSet<int>(iv.collisionPolygonsOther);
					HashSet<int> hsOther = new HashSet<int>(GlobalData.map.segments[i].impossibleVolumes[0].collisionPolygonsSelf);
					if (hsSelf.SetEquals(hsOther)) {
						match = true;				
					}
				}
				if (!match) {
					impossibleVolume iv = new impossibleVolume();
					iv.collisionPolygonsSelf = seg.impossibleVolumes[0].collisionPolygonsSelf;
					iv.collisionPolygonsOther = GlobalData.map.segments[i].impossibleVolumes[0].collisionPolygonsSelf;
					seg.impossibleVolumes.Add(iv);
				}
			}
			connected = seg.impossibleVolumes[0].collisionPolygonsSelf;
		}
		
		return connected;
	}

	public void calculateCollisionPoints() {
		//List<Vector2> intersectPoints = new List<Vector2>();
		foreach (Vector3[] self in sidesSelf) {
			float a1 = self[1].z - self[0].z;
			float b1 = self[0].x - self[1].x;
			float c1 = a1*self[0].x + b1*self[0].z;

			foreach (Vector3[] other in sidesOther) {

				float a2 = other[0].z - other[1].z;
				float b2 = other[1].x - other[0].x;
				float c2 = a2*other[1].x + b2*other[1].z;

				float delta = a1*b2 - a2*b1;
				if(delta != 0) {
					Vector2 intersect = new Vector2((b2*c1 - b1*c2)/delta, (a1*c2 - a2*c1)/delta);
					
					float d1 = Vector2.Distance(new Vector2(self[0].x, self[0].z), intersect);
					float d2 = Vector2.Distance(new Vector2(self[1].x, self[1].z), intersect);
					float d3 = Vector2.Distance(new Vector2(self[0].x, self[0].z), new Vector2(self[1].x, self[1].z));

					float d4 = Vector2.Distance(new Vector2(other[1].x, other[1].z), intersect);
					float d5 = Vector2.Distance(new Vector2(other[0].x, other[0].z), intersect);
					float d6 = Vector2.Distance(new Vector2(other[1].x, other[1].z), new Vector2(other[0].x, other[0].z));

					// if lenghts from end to point to end = length of line then valid point
					//+- 0.01 to account for floating point errors 
					if (d1 + d2 - 0.001 < d3 + 0.001 && d1 + d2 + 0.001 > d3 - 0.001
						 && d4 + d5 - 0.001 < d6 + 0.001 && d4 + d5 + 0.001 > d6 - 0.001) {
						collisionPoints.Add(intersect);
					}					
				}			
			}
		}
	}

}