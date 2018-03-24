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
	
	public List<MapSegment> levelSegments;
	public Liquid liquid = null;
	public bool impossible = false;
	public int viewEdge = -1;
	private bool hidden = false;
	// Use this for initialization
	void Start () {


	}

	public void checkIfImpossible() {
		RaycastHit hit;
		List<Vector3> verts = new List<Vector3>(vertices);
		verts.Add(new Vector3(0,0,0));
		foreach (Vector3 vert in verts) {
			Vector3 startPoint = gameObject.transform.TransformPoint(vert);
			// if (id == 22) {
			// 	Debug.Log(centerPoint);
			// }

			startPoint = (startPoint-centerPoint)*0.95f + centerPoint  + (height*0.05f);
			// if (id == 22) {
			// 	Debug.Log(castPoint);
			// }
			Vector3 castPoint = gameObject.transform.TransformPoint(vert);
			float rayCount = 4f;
			for (int i = 0; i < rayCount; i++) {
				castPoint = (startPoint-centerPoint)*((1f/rayCount)*(float)i) + centerPoint  + (height*0.05f);

				if (Physics.Raycast(castPoint, Vector3.up, out hit, height.y)) {
					if (hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject != gameObject) {
						if (hit.collider.transform.parent.GetComponent<MapSegment>() != null) {
							hit.collider.transform.parent.GetComponent<MapSegment>().impossible = true;
						}
						impossible = true;
						Debug.DrawRay(castPoint, Vector3.up, Color.magenta);				
					}
				}
				castPoint.y -=  (height.y*0.1f);
				if (Physics.Raycast(castPoint, Vector3.down, out hit,10)) {
					if (hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.name == "polygon(Clone)") {
						if (hit.collider.name != "ceiling") {
							hit.collider.transform.parent.GetComponent<MapSegment>().impossible = true;
							impossible = true;
							Debug.Log(id);
							Debug.DrawRay(castPoint, Vector3.down, Color.white);				

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
				s.connection = levelSegments[s.connectionID];
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
				wall.connection = levelSegments[wall.connectionID];
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
			wall.connection = levelSegments[wall.connectionID];
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
	public Material tint = new Material(Shader.Find("Standard"));
	public Material surface = new Material(Shader.Find("Standard"));
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
	public Material activeMat = new Material(Shader.Find("Standard"));
	public Material inactiveMat = new Material(Shader.Find("Standard"));


	public void toggle(GameObject wall, bool playerTouched = false) {
		active = !active;
		Vector2 offset = wall.GetComponent<MeshRenderer>().material.mainTextureOffset;
		if (active) {
			wall.GetComponent<MeshRenderer>().material = activeMat;
			if (platformSwitch > -1) {
				MapSegment pol = wall.transform.parent.GetComponent<MapSegment>().levelSegments[platformSwitch];
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