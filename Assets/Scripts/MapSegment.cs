using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapSegment : MonoBehaviour {
	public List<Vector3> vertices;
	public int id = -1;
	public Vector3 height;
	private int[] triangles;
	public Platform platform = null;
	public Vector3 centerPoint;

	public List<MapSegmentSide> sides = new List<MapSegmentSide>();

	public MapSegmentFloorCeiling ceiling = new MapSegmentFloorCeiling();
	public MapSegmentFloorCeiling floor = new MapSegmentFloorCeiling();
	
	//public List<MapSegment> levelSegments;
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
		floor.setLight();
		ceiling.setLight();
		foreach (MapSegmentSide side in sides) {
			side.setLight();
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
			if (id == 36) {
				Debug.Log(centerPoint);
			}

			startPoint = (startPoint-centerPoint)*0.95f + centerPoint  + (height*0.05f);
			// if (id == 22) {
			// 	Debug.Log(castPoint);
			// }
			Vector3 castPoint = gameObject.transform.TransformPoint(vert);
			float rayCount = 4f;

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
						Debug.DrawRay(centerPoint + height*0.5f, (castPoint+height*0.5f)-(centerPoint + height*0.5f), Color.yellow);				
						return;					

					}
				}
			}

			for (int i = 0; i < rayCount; i++) {
				castPoint = (startPoint-centerPoint)*((1f/rayCount)*(float)i) + centerPoint  + (height*0.05f);

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
	
						return;
					}
				}
				castPoint.y -=  (height.y*0.1f);
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
							return;

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
		foreach (MapSegmentSide s in sides) {
			if (s.connection == null && s.connectionID >= 0) {
				s.connection = GlobalData.map.segments[s.connectionID];
			}
			if (s.connection != null){
				MapSegment conn = s.connection;

					// if (id == 6) {
					// 	Debug.Log(centerPoint);
					// 	Debug.Log(conn.centerPoint);
					// 	Debug.Log(height);

					// }

				if (conn.centerPoint.y < centerPoint.y 
							&& platform.comesFromFloor) {
					height.y += (centerPoint.y - conn.centerPoint.y);
					centerPoint.y = conn.centerPoint.y;
				}
				if (conn.centerPoint.y + conn.height.y > centerPoint.y + height.y
							&& platform.comesFromCeiling) {
					height.y += (conn.height.y - conn.centerPoint.y) - (height.y - centerPoint.y);
				}
			}
		}
		gameObject.transform.position = centerPoint;
	}

	public void makePlatformObjects() {
		// if (id == 32) {
		// 	Debug.Log(height);
		// 	Debug.Log(platform.maximumHeight);
		// 	Debug.Log(platform.minimumHeight);
		// }
		GameObject part = null;
		Vector3 pHeight = height;
		Vector3 splitPoint = new Vector3(height.x, 
										platform.minimumHeight + (platform.maximumHeight - platform.minimumHeight)/2,
										height.z); 
		splitPoint.y -= gameObject.transform.position.y;
		// Debug.Log(platform.maximumHeight);
		// Debug.Log(platform.minimumHeight);
		// Debug.Log(platform.minimumHeight + (platform.maximumHeight - platform.minimumHeight)/2);
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

			part = makePolygon(true, PlatVertices, pHeight, platform.upperPlatform);
			if (part != null) {
				part.name = "platBottom";
				platform.upperBottom.meshItem = part;
				platform.upperBottom.light = floor.light;
				platform.upperBottom.lightID = floor.lightID;
			}
			part = makePolygon(false, PlatVertices, pHeight, platform.upperPlatform);
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
			makePolygon(true, liquidVertices, liquidHeight, liquid.volume, Resources.Load("texture") as Material);
			makePolygon(false, liquidVertices, liquidHeight, liquid.volume, Resources.Load("texture") as Material);

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
		if (mat == null) {mat = Resources.Load("texture") as Material;}
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

		// if (id == 61) {
		// 	id = id;
		// }

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

			if (wall.connection.GetComponent<MapSegment>().id == 362){ 
				Debug.Log(gameObject.transform.position.y);
				}
			bool connTop = (wall.connection.platform != null &&
							!wall.connection.platform.comesFromCeiling) ||
							 wall.connection.platform == null || wall.solid; 
			bool connBottom = (wall.connection.platform != null &&
							!wall.connection.platform.comesFromFloor)||
							wall.connection.platform == null || wall.solid; 
; 

			if (wall.connection.transform.position.y > gameObject.transform.position.y 
				&& connBottom) {
				if (wall.solid && wall.lowerMaterial == null ) {
					wall.lowerMaterial = Resources.Load<Material>("transparent");
				}
				wallHeightLower = new Vector3(height.x, height.y, height.z);
				wallHeightLower.y = wall.connection.transform.position.y - gameObject.transform.position.y;
				wallPart = addWallPart(point1, point2, wallHeightLower, wallOffset, wall.lowerMaterial, wall.lowerOffset, gameObject);
				if (wallPart != null) {
					wallPart.name = "lowerWall";
					sides[side].lowerMeshItem = wallPart;
				}
				// if (wall.solid && wall.lowerMaterial.name == "transparent" || 
				// 		(wall.transparent && connBottom && !connTop)) {
				// 	wallPart.SetActive(false);
				// }
			}

			if (wall.connection.transform.position.y+wall.connection.height.y < gameObject.transform.position.y+height.y
				&& connTop) {
				if (wall.solid && wall.upperMaterial == null ) {
					wall.upperMaterial = Resources.Load<Material>("transparent");
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
				}
				// if (wall.solid && wall.upperMaterial.name == "transparent" || 
				// 		(wall.transparent && connTop && !connBottom)) {
				// 	wallPart.SetActive(false);
				// }
			}

			Vector3 wallHeightMiddle = height - wallHeightLower - wallHeightUpper;
			if ( wallHeightMiddle.y>0) {
				if (wall.solid && wall.middeMaterial == null ) {
					wall.middeMaterial = Resources.Load<Material>("transparent");
				}

				wallPart = addWallPart(point1, point2, 
							wallHeightMiddle,
							new Vector3(0,wallHeightLower.y,0), 
							wall.middeMaterial, wall.middleOffset, gameObject);
				
				if (wallPart != null) {
					wallPart.name = "middleWall";
					sides[side].middleMeshItem = wallPart;
				}


				if (wall.solid && wall.middeMaterial.name == "transparent" || 
						(wall.transparent && !connTop && !connBottom)) {
					wallPart.SetActive(false);
				}
				
			}
		} else {
			if (wall.connectionID == -1){
				if (wall.upperMaterial == null) { wall.upperMaterial = Resources.Load<Material>("texture");}
				wallPart = addWallPart(point1, point2, height, wallOffset, wall.upperMaterial, wall.upperOffset, gameObject);
				wallPart.name = "wall";
				sides[side].upperMeshItem = wallPart;

			}
		}

		if (wallPart != null) {
			sides[side].upperMeshItem = wallPart;
			wallPart.name = "wall";
		}
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
		viscalc.transform.localScale=new Vector3(0.005f,0.005f,0.005f);


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
					addToPolygonList(side.connectionID);
					GlobalData.map.segments[id].viewEdge = 1;
			}
		}
		processedPolys[id] = true;processedCount++;
		
		while (processedCount < activeCount) {
			float distance = 7777777;
			int closest = -1;
			int connections = 0;
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
				connections = addToPolygonList(closest);
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

				// if ( GlobalData.map.segments[side.connectionID].impossible) {

				// }


				isVisible = getRectVisibility(point1, point2, seg.height);
				if (id == 16) {
					if (seg.id == 39 || seg.id == 40) {
					;
					}
				}
				if (isVisible) {
					if (!backlink) {
						activePolygons[side.connectionID] = true; activeCount++;
						if (GlobalData.map.segments[side.connectionID].activePolygons.Length == 0) {
							GlobalData.map.segments[side.connectionID].activePolygons = new bool[GlobalData.map.segments.Count];
						}
						GlobalData.map.segments[side.connectionID].activePolygons[id] = true;

						seg.viewEdge = 0;
						GlobalData.map.segments[side.connectionID].viewEdge = 1;
						addCount++;
					} else {
						GlobalData.map.segments[side.connectionID].viewEdge = 0;
						GlobalData.map.segments[side.connectionID].viewEdge = 0;
						//seg.viewEdge = 0;
					}
				} else if (seg.viewEdge < overDraw) {
					if (!backlink) {
						activePolygons[side.connectionID] = true; activeCount++;
						if (GlobalData.map.segments[side.connectionID].activePolygons.Length == 0) {
							GlobalData.map.segments[side.connectionID].activePolygons = new bool[GlobalData.map.segments.Count];
						}
						GlobalData.map.segments[side.connectionID].activePolygons[id] = true;
						GlobalData.map.segments[side.connectionID].viewEdge = seg.viewEdge + 1;
						addCount++;
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

		points[0] = p1 + point2  + h1;
		points[1] = p2 + point2  + h1;
		points[2] = p1 + point2  + h2;
		points[3] = p2 + point2  + h2;
		points[4] = p3 + point2  + h2;
		points[5] = p3 + point2  + h1;
		points[6] = p1 + point2  + h3;
		points[7] = p2 + point2  + h3;


		Vector3 castPoint, crossPoint;
		float rayCount = 3f;
		float heightCount = 2f;
		float crossCount = 2f;

		isVisible = false;

		List<Vector3> checkpoints = new List<Vector3>();
		checkpoints.Add(viscalc.transform.position);
		foreach(Vector3 v in vertices) {

			for (int c = 1; c <=crossCount; c++) {
				crossPoint = ((gameObject.transform.TransformPoint(v)-centerPoint)*((1f/crossCount)*(float)c) + centerPoint);
				for (int h = 1; h <=heightCount; h++) {
					checkpoints.Add(crossPoint + (height*0.95f)*(1f/heightCount));
				}
			}
		}
		//Color colour = Random.ColorHSV();

		foreach(Vector3 cp in checkpoints) {
			viscalc.transform.position = cp;
			viscalc.name = "viscalc";
			isVisible = (Physics.Raycast(midpoint, viscalc.transform.position-midpoint, out hit, 50)) 
				&& (hit.collider.gameObject == viscalc || hit.collider.gameObject.name == "viscalc") ;
			if(	id == 38 ) {
				Debug.DrawRay(midpoint, viscalc.transform.position-midpoint, Color.magenta, 10);
			
				if (Physics.Raycast(midpoint, viscalc.transform.position-midpoint, out hit, 50) && hit.collider.gameObject.name != "wall") {
					;
				}
			}
			if (isVisible) {
				if (id == 38) {
					Debug.DrawRay(midpoint, viscalc.transform.position-midpoint, Color.green ,10);
				}
				return true;
			}
			for (int i = 1; i < rayCount; i++) {
				for (int p = 0; p < points.Length && !isVisible; p++){
					castPoint = (points[p]-midpoint)*((1f/rayCount)*(float)i) + midpoint;
					isVisible = (Physics.Raycast(castPoint, viscalc.transform.position-castPoint, out hit, 50)) 
								&& hit.collider.gameObject == viscalc ;
					if (id == 38) {
						Debug.DrawRay(castPoint, viscalc.transform.position-castPoint, Color.blue, 10);
					}
					if (isVisible) {
						if (id == 38) {
						Debug.DrawRay(castPoint, viscalc.transform.position-castPoint, Color.green ,10);
						}
						return true;
						}
				}
			}
		}

		

		return isVisible;

	}



}


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

public class Platform {
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
				Debug.Log("'woo?'");
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



public class Liquid {
	public float currentSpeed = 0;
	public Quaternion currentDirectioin = new Quaternion();
	public float high = 0;
	public float low = 0;
	public Material tint = new Material(Shader.Find("Custom/StandardClippableV2"));
	public Material surface = new Material(Shader.Find("Custom/StandardClippableV2"));
	public float damage = 0;

	public GameObject volume = null;

}

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
		active = initiallyActive;
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
	}

	public void lightMaterial (GameObject obj) {
		if (lightChanged) {
			MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
			if (meshRenderer != null) {
				Material material = meshRenderer.sharedMaterial;
				Color brightness = Color.white;
				brightness *= currentIntensity;
				material.SetColor ("_EmissionColor", brightness);
				obj.GetComponent<MeshRenderer>().sharedMaterial = material;
			}
		}
	}

	

}

public class LightFunction {
	public int mode = 0;
	public float period = 0;
	public float periodDelta = 0;
	public float intensity = 0;
	public float intensityDelta = 0;

	public float totalPeriod = 0;
	public float totalIntensity = 0;

	private float flickertime = 0;
	private float initialIntensity = 0;

	public void initialise(float currentIntensity = 0) {
		initialIntensity = currentIntensity;
		totalPeriod = period + Random.Range(0f-periodDelta,periodDelta);
		totalIntensity = intensity + Random.Range(0f-intensityDelta*intensity,intensityDelta*intensity);
		flickertime = 0;
	}

	public float lightIntensity (float time, bool hightToLow = false) {
		float value = 0;
		float high, low, delta, amt;
		amt = 0;
		switch (mode) {
		case 0: //constant
			value = totalIntensity;
			break;
		case 1: //linear
			if (hightToLow) {
				high = initialIntensity;
				low = totalIntensity;
			} else {
				low = initialIntensity;
				high = totalIntensity;
			}
			delta = high-low;
			if (time > 0) {
				value = delta * (time / totalPeriod);
			} else {
				value = initialIntensity;
			}
			if (hightToLow) {
				value = initialIntensity - value;
			} else {
				value = initialIntensity + value;
			}
			break;
		case 2: //smooth
			if (hightToLow) {
				high = initialIntensity;
				low = totalIntensity;
			} else {
				low = initialIntensity;
				high = totalIntensity;
			}
			delta = high-low;
			if (time > 0) {
				amt = time /totalPeriod;
				amt *= 2f;
				amt -= 1f;
				value = Mathf.Sin(amt * 90f * Mathf.Deg2Rad);
				value += 1f;
				value /= 2f;
				value *= delta;
			} else {
				value = initialIntensity;
			}

			if (hightToLow) {
				value = initialIntensity - value;
			} else {
				value = initialIntensity + value;
			}

			break;
		case 3: //flicker
			value = totalIntensity;
			if (time > flickertime) {
				flickertime = time + Random.Range(0f,0.2f);
				if (Random.Range(0, 1) < 0.5f) {
					value = initialIntensity;
				}
			}
			break;
		}

		//if (value > totalIntensity) {value = totalIntensity;}
		if (value < 0) {value = 0;}
		return value;
	} 

	public void setFromMarathonObject (Weland.Light.Function state) {
		switch (state.LightingFunction) {
		case Weland.LightingFunction.Constant:
			mode = 0;
			break;
		case Weland.LightingFunction.Linear:
			mode = 1;
			break;
		case Weland.LightingFunction.Smooth:
			mode = 2;
			break;
		case Weland.LightingFunction.Flicker:
			mode = 3;
			break;	
		}
		intensityDelta = (float)state.DeltaIntensity;
		period = (float)state.Period/30f;
		intensity = (float)state.Intensity;
		periodDelta = (float)state.DeltaPeriod/30f;
	}

}