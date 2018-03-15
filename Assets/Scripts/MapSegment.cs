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

	public MapSegmentFloorCeiling Ceiling = new MapSegmentFloorCeiling();
	public MapSegmentFloorCeiling Floor = new MapSegmentFloorCeiling();
	

	public List<GameObject> levelSegments;
	// Use this for initialization
	void Start () {


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
				MapSegment conn = s.connection.GetComponent<MapSegment>();

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
		if (id == 32) {
			Debug.Log(height);
			Debug.Log(platform.MaximumHeight);
			Debug.Log(platform.MinimumHeight);
		}
		Vector3 pHeight = height;
		Vector3 splitPoint = new Vector3(height.x, 
										platform.MinimumHeight + (platform.MaximumHeight - platform.MinimumHeight)/2,
										height.z); 
		splitPoint.y -= gameObject.transform.position.y;
		// Debug.Log(platform.MaximumHeight);
		// Debug.Log(platform.MinimumHeight);
		// Debug.Log(platform.MinimumHeight + (platform.MaximumHeight - platform.MinimumHeight)/2);
		List<Vector3> PlatVertices = new List<Vector3>(vertices);
		PlatVertices.Reverse();
		bool split = platform.ComesFromFloor && platform.ComesFromCeiling;
		if (split) {
			pHeight.y = splitPoint.y;
		}
		if (platform.ComesFromFloor) {
			platform.lowerPlatform =  new GameObject("lowerPlatform");
			platform.lowerPlatform.transform.parent = gameObject.transform;
			makePolygon(true, PlatVertices, pHeight, platform.lowerPlatform);
			makePolygon(false, PlatVertices, pHeight, platform.lowerPlatform);
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
		generateColliders(gameObject);

		
	}

	public void generateColliders (GameObject obj){
		foreach(Transform child in obj.transform) {
			if (child.gameObject.name == "polygonElement(Clone)") {
				MeshCollider mc = child.gameObject.AddComponent<MeshCollider>();
				Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
				mc.sharedMesh = child.GetComponent<MeshFilter>().mesh;
				rb.useGravity = false;	
				rb.isKinematic = true;			
			}
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

	void makePolygon(bool isBase, List<Vector3> vertices, Vector3 polHeight, GameObject parent) {
		List<Vector3> _vertices = new List<Vector3>(vertices);
		GameObject meshItem = Instantiate(Resources.Load<GameObject>("polygonElement"), parent.transform.position, parent.transform.rotation);
		meshItem.transform.parent = parent.transform;
		MeshFilter meshfilter = meshItem.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
        meshfilter.mesh = mesh;
		Vector2 matOffset = new Vector2(0,0);
		//Material mat = Resources.Load("stripes") as Material;
		//Material mat = GameObject.Find("gameController").GetComponent<Map>().materials[1];
		//meshItem.GetComponent<MeshRenderer>().material = mat;
		Material mat;
		if (isBase) {
			mat = Floor.upperMaterial;
			matOffset = Floor.upperOffset + new Vector2(centerPoint.z, centerPoint.x);
		} else {
			mat = Ceiling.upperMaterial;
			matOffset = Ceiling.upperOffset + new Vector2(centerPoint.z, centerPoint.x);
		}
		if (mat == null) {mat = Resources.Load("texture") as Material;}
		meshItem.GetComponent<MeshRenderer>().material = mat;
		meshItem.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", matOffset);

		if (!isBase) {
			_vertices.Reverse();
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

		if (wall.connection != null && 
			(
				wall.transparent == true || 
				wall.connection.GetComponent<MapSegment>().platform != null ||
				platform != null
			)
			){

			// if (wall.connection.GetComponent<MapSegment>().id == 17){ 
			// 	Debug.Log(gameObject.transform.position.y);
			// 	Debug.Log(wall.connection.transform.position.y);
			// 	Debug.Log(wall.connection.transform.position.y+wall.connection.GetComponent<MapSegment>().height.y);
			// 	Debug.Log(gameObject.transform.position.y+height.y);

			// 	}
			bool connTop = (wall.connection.GetComponent<MapSegment>().platform != null &&
							!wall.connection.GetComponent<MapSegment>().platform.ComesFromCeiling) ||
							 wall.connection.GetComponent<MapSegment>().platform == null; 
			bool connBottom = (wall.connection.GetComponent<MapSegment>().platform != null &&
							!wall.connection.GetComponent<MapSegment>().platform.ComesFromFloor)||
							wall.connection.GetComponent<MapSegment>().platform == null; 
; 
		
			if (wall.connection.transform.position.y > gameObject.transform.position.y 
				&& connBottom) {
				wallHeightLower = new Vector3(height.x, height.y, height.z);
				wallHeightLower.y = wall.connection.transform.position.y - gameObject.transform.position.y;
				addWallPart(point1, point2, wallHeightLower, wallOffset, wall.lowerMaterial, wall.lowerOffset, gameObject);
			}


			if (wall.connection.transform.position.y+wall.connection.GetComponent<MapSegment>().height.y < gameObject.transform.position.y+height.y
				&& connTop) {
				wallHeightUpper = new Vector3(height.x, height.y, height.z);
				wallOffset = wall.connection.GetComponent<MapSegment>().height + wall.connection.transform.position - gameObject.transform.position;
				wallOffset.x = 0;
				wallOffset.z = 0;
				wallHeightUpper.y = height.y - wallOffset.y;
				addWallPart(point1, point2, wallHeightUpper, wallOffset, wall.upperMaterial, wall.upperOffset, gameObject);
			}

			Vector3 wallHeightMiddle = height - wallHeightLower - wallHeightUpper;
			if (wall.middeMaterial != null && wallHeightMiddle.y>0) {

				addWallPart(point1, point2, 
							wallHeightMiddle,
							new Vector3(0,wallHeightLower.y,0), 
							wall.middeMaterial, wall.middleOffset, gameObject);
			}
		} else {
			addWallPart(point1, point2, height, wallOffset, wall.upperMaterial, wall.upperOffset, gameObject);
		}
	}


	void addWallPart(Vector3 point1, Vector3 point2, Vector3 wallHeight, Vector3 offset, Material material, Vector2 matOffset, GameObject parent){

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
			meshItem.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
		}

		// if (id==3) {
		// }
	}


	// Update is called once per frame
	void Update () {
		
	}

}


public class MapSegmentSide {
	public GameObject connection = null;
	public int connectionID = -1;
	public float Opacity = 1;
	public bool solid = true;
	public bool transparent = false;
	public Material upperMaterial, lowerMaterial, middeMaterial;
	public Vector2 upperOffset, middleOffset, lowerOffset = new Vector2(0,0);

}

public class MapSegmentFloorCeiling {
	public GameObject conection = null;
	public int connectionID = -1;
	public float Opacity = 1;
	public bool solid = true;
	public bool transparent = false;
	public Material upperMaterial, lowerMaterial, middeMaterial;
	public Vector2 upperOffset, middleOffset, lowerOffset = new Vector2(0,0);

}

public class Platform {
	public float Speed;
	public float Delay;
	public float MaximumHeight;
	public float MinimumHeight;
	public int Tag;
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
	public bool DelaysBeforeActivation  = false;
	public bool ActivatesAdjacentPlatformsWhenActivating = false;
	public bool DeactivatesAdjacentPlatformsWhenActivating  = false;
	public bool DeactivatesAdjacentPlatformsWhenDeactivating  = false;
	public bool ContractsSlower  = false;
	public bool ActivatesAdjacantPlatformsAtEachLevel  = false;
	public bool IsLocked = false;
	public bool IsSecret  = false;
	public bool IsDoor = false;
	public GameObject lowerPlatform = null;
	public GameObject upperPlatform = null;
}
