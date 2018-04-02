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
	public int overDraw = 3;
	
	private int activeCount = 0;
	private	GameObject viscalc;

	// Use this for initialization
	void Start () {


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
				if (hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.name == "polygon(Clone)") {
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
					if (hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.name == "polygon(Clone)") {
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
			foreach (Transform child in allChildren) {
				if (child.gameObject.name == "floor" ||child.gameObject.name == "ceiling" || child.gameObject.name == "wall" || child.gameObject.name == "transparent" || child.gameObject.name == "polygonElement(Clone)"){
					child.gameObject.SetActive(show);
				}
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
							&& platform.ComesFromFloor) {
					height.y += (centerPoint.y - conn.centerPoint.y);
					centerPoint.y = conn.centerPoint.y;
				}
				if (conn.centerPoint.y + conn.height.y > centerPoint.y + height.y
							&& platform.ComesFromCeiling) {
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
		bool split = platform.ComesFromFloor && platform.ComesFromCeiling;
		if (split) {
			pHeight.y = splitPoint.y;
		}
		if (platform.ComesFromFloor) {
			platform.lowerPlatform =  new GameObject("lowerPlatform");
			platform.lowerPlatform.transform.parent = gameObject.transform;
			makePolygon(true, PlatVertices, pHeight, platform.lowerPlatform, ceiling.upperMaterial, ceiling.upperOffset);
			makePolygon(false, PlatVertices, pHeight, platform.lowerPlatform, floor.upperMaterial, floor.upperOffset);
		}
		if (split) {
			pHeight.y = height.y - (splitPoint.y);
		}

		if (platform.ComesFromCeiling) {
			platform.upperPlatform =  new GameObject("upperPlatform");
			platform.upperPlatform.transform.parent = gameObject.transform;

			makePolygon(true, PlatVertices, pHeight, platform.upperPlatform);
			makePolygon(false, PlatVertices, pHeight, platform.upperPlatform);
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
				
			
			if (platform.ComesFromFloor) {
				if (split) {
				addWallPart(point1, point2, splitPoint, new Vector3(0,0,0), wall.lowerMaterial, wall.lowerOffset, platform.lowerPlatform);
				} else {
				addWallPart(point1, point2, pHeight, new Vector3(0,0,0), wall.lowerMaterial, wall.lowerOffset, platform.lowerPlatform);
				}
			}
			if (platform.ComesFromCeiling) {
				addWallPart(point1, point2, pHeight, new Vector3(0,0,0), wall.upperMaterial, wall.upperOffset, platform.upperPlatform);
				
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

		makePolygon(true, vertices, height, gameObject);
		makePolygon(false, vertices, height, gameObject);

		for (int i = 0; i < vertices.Count; i++) {
			makeWall(i);
		}
		generateLiquidVolumes();
		generateColliders(gameObject);

	}

	public void generateColliders (GameObject obj){
		foreach(Transform child in obj.transform) {
			if (child.gameObject.name == "floor" ||child.gameObject.name == "ceiling" || child.gameObject.name == "wall" || child.gameObject.name == "transparent" || child.gameObject.name == "polygonElement(Clone)"){
				MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
				mc.sharedMesh = child.GetComponent<MeshFilter>().mesh;
				if (child.gameObject.name != "transparent") {
					Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
					rb.useGravity = false;	
					rb.isKinematic = true;

				// } else {
					// mc.convex = true;
					// mc.isTrigger = true;
					// child.gameObject.name = "transparent";
				}
						
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
				addWallPart(point1, point2, liquidHeight, 
							new Vector3(0,0,0), liquid.surface, 
							new Vector2(0,0), liquid.volume);
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

	void makePolygon(bool isBase, List<Vector3> vertices, Vector3 polHeight, GameObject parent, Material mat = null, Vector2 matOffset = default(Vector2)) {
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
				platform != null
				)
			){

			// if (wall.connection.GetComponent<MapSegment>().id == 17){ 
			// 	Debug.Log(gameObject.transform.position.y);
			// 	Debug.Log(wall.connection.transform.position.y);
			// 	Debug.Log(wall.connection.transform.position.y+wall.connection.GetComponent<MapSegment>().height.y);
			// 	Debug.Log(gameObject.transform.position.y+height.y);

			// 	}
			bool connTop = (wall.connection.platform != null &&
							!wall.connection.platform.ComesFromCeiling) ||
							 wall.connection.platform == null; 
			bool connBottom = (wall.connection.platform != null &&
							!wall.connection.platform.ComesFromFloor)||
							wall.connection.platform == null; 
; 
		
			if (wall.connection.transform.position.y > gameObject.transform.position.y 
				&& connBottom) {
				wallHeightLower = new Vector3(height.x, height.y, height.z);
				wallHeightLower.y = wall.connection.transform.position.y - gameObject.transform.position.y;
				wallPart = addWallPart(point1, point2, wallHeightLower, wallOffset, wall.lowerMaterial, wall.lowerOffset, gameObject);
			}


			if (wall.connection.transform.position.y+wall.connection.height.y < gameObject.transform.position.y+height.y
				&& connTop) {
				wallHeightUpper = new Vector3(height.x, height.y, height.z);
				wallOffset = wall.connection.height + wall.connection.transform.position - gameObject.transform.position;
				wallOffset.x = 0;
				wallOffset.z = 0;
				wallHeightUpper.y = height.y - wallOffset.y;
				wallPart = addWallPart(point1, point2, wallHeightUpper, wallOffset, wall.upperMaterial, wall.upperOffset, gameObject);
			}

			Vector3 wallHeightMiddle = height - wallHeightLower - wallHeightUpper;
			if ( wallHeightMiddle.y>0) {

				wallPart = addWallPart(point1, point2, 
							wallHeightMiddle,
							new Vector3(0,wallHeightLower.y,0), 
							wall.middeMaterial, wall.middleOffset, gameObject);
			}
		} else {
			wallPart = addWallPart(point1, point2, height, wallOffset, wall.upperMaterial, wall.upperOffset, gameObject);
		}
		if (wallPart != null) {
			sides[side].meshItem = wallPart;
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
			meshItem.GetComponent<MeshRenderer>().material = Resources.Load<Material>("transparent");
			meshItem.name = "transparent";
			meshItem.GetComponent<MeshRenderer>().enabled = false;
		}

		return meshItem;
	}


	// Update is called once per frame
	void Update () {
		
	}

	public bool playerTouch(GameObject element) {
		bool touched = false;
		foreach (MapSegmentSide s in sides) {
			if (s.controlPanel != null && s.meshItem == element) {
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


		viscalc = GameObject.CreatePrimitive(PrimitiveType.Cube);
		viscalc.transform.parent = transform;
		viscalc.transform.position = transform.position + (height/2);
		viscalc.transform.localScale=new Vector3(0.005f,0.005f,0.005f);


	// Debug.Log("----------------------------------------");
		if (activePolygons.Length == 0) {
			activePolygons = new bool[GlobalData.map.segments.Count];
		}
		
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
			if (side.meshItem == null && side.connectionID != -1) {
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

		if (id == 46 && closest == 31) {
			processedCount = processedCount;
		}


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
		float crossCount = 1f;

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
			isVisible = (Physics.Raycast(midpoint, viscalc.transform.position-midpoint, out hit, 50)) 
				&& hit.collider.gameObject == viscalc ;
			// if(	isVisible &&id == 46 ) {
			// 	Debug.DrawRay(midpoint, viscalc.transform.position-midpoint, Color.red);
			// }
			if (isVisible) {
				return true;
			}
			for (int i = 1; i < rayCount; i++) {
				for (int p = 0; p < points.Length && !isVisible; p++){
					castPoint = (points[p]-midpoint)*((1f/rayCount)*(float)i) + midpoint;
					isVisible = (Physics.Raycast(castPoint, viscalc.transform.position-castPoint, out hit, 50)) 
								&& hit.collider.gameObject == viscalc ;
					// if (id == 46) {
					// 	Debug.DrawRay(castPoint, viscalc.transform.position-castPoint, colour);
					// }
					if (isVisible) {
						// if (id == 46) {
						// Debug.DrawRay(castPoint, viscalc.transform.position-castPoint, Color.red);
						// }
						return true;
						}
				}
			}
		}

		

		return isVisible;

	}



}


public class MapSegmentSide {
	public MapSegment connection = null;
	public GameObject meshItem = null;
	public int connectionID = -1;
	public float Opacity = 1;
	public bool solid = true;
	public bool transparent = false;
	public Material upperMaterial, lowerMaterial, middeMaterial;
	public Vector2 upperOffset, middleOffset, lowerOffset = new Vector2(0,0);
	public ControlPanel controlPanel = null;

}

public class MapSegmentFloorCeiling {
	public GameObject meshItem = null;

	public MapSegment conection = null;
	public int connectionID = -1;
	public float Opacity = 1;
	public bool solid = true;
	public bool transparent = false;
	public Material upperMaterial, lowerMaterial, middeMaterial;
	public Vector2 upperOffset, middleOffset, lowerOffset = new Vector2(0,0);
	public ControlPanel controlPanel = null;
}

public class Platform {
	public float Speed = 1;
	public float delay = 0;
	public float maximumHeight;
	public float minimumHeight;
	public int tag;
	public bool InitiallyActive = false;
	public bool InitiallyExtended = false;
	public bool DeactivatesAtEachLevel  = false;
	public bool DeactivatesAtInitialLevel  = false;
	public bool ActivatesAdjacentPlatformsWhenDeactivating  = false;
	public bool ExtendsFloorToCeiling  = false;
	public bool ComesFromFloor  = false;
	public bool ComesFromCeiling  = false;
	public bool CausesDamage  = false;
	public bool DoesNotActivateParent  = false;
	public bool ActivatesOnlyOnce  = false;
	public bool ActivatesLight  = false;
	public bool DeactivatesLight  = false;
	public bool IsPlayerControllable  = false;
	public bool IsMonsterControllable  = false;
	public bool ReversesDirectionWhenObstructed  = false;
	public bool CannotBeExternallyDeactivated  = false;
	public bool UsesNativePolygonHeights = false;
	public bool delaysBeforeActivation  = false;
	public bool activatesAdjacentPlatformsWhenActivating = false;
	public bool deactivatesAdjacentPlatformsWhenActivating  = false;
	public bool deactivatesAdjacentPlatformsWhenDeactivating  = false;
	public bool contractsSlower  = false;
	public bool activatesAdjacantPlatformsAtEachLevel  = false;
	public bool locked = false;
	public bool secret  = false;
	public bool door = false;
	public GameObject lowerPlatform = null;
	public GameObject upperPlatform = null;
	public float upperMaxHeight = 0;
	public float upperMinHeight = 0;
	public float lowerMaxHeight = 0;
	public float lowerMinHeight = 0;

	public bool active = false;
	public bool extended = true;
	public bool extending = false;
	//public float delayedTime = 0;
	private bool hasActivated = false;


	public void playerTouch() {
		//Debug.Log("platform");
		//element.GetComponent<Mesh>().material =
				Debug.Log("'woo?'");
		if (door) {
			if (!locked) {
				
				activate();
			} else {
				//?? play locked sound??
			}
		}
	}

	public void activate() {

		active = true;
		extending = !extending;
		extended = !extended;
		if( upperPlatform != null ) {
			upperPlatform.GetComponent<platformController>().delayedTime = 0;
		}
		if (lowerPlatform != null) {
			lowerPlatform.GetComponent<platformController>().delayedTime = 0;
		}
		hasActivated = true;
		
	}

	public void deActivate() {
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
		if (uptransit == lotransit) {
			active = false;
			Debug.Log("stop");
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


	public void toggle(GameObject wall, bool playerTouched = false) {
		active = !active;
		Vector2 offset = wall.GetComponent<MeshRenderer>().material.mainTextureOffset;
		if (active) {
			wall.GetComponent<MeshRenderer>().material = activeMat;
			if (platformSwitch > -1) {
				MapSegment pol = GlobalData.map.segments[platformSwitch];
				if (pol.platform != null) {
					pol.platform.activate();
				}
			}
		} else {
			wall.GetComponent<MeshRenderer>().material = inactiveMat;
		}
		wall.GetComponent<MeshRenderer>().material.mainTextureOffset = offset;
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