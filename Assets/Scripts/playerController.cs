using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
	public float accelleration = 1;
	public float maxspeed = 3;

	float accellAmountZ = 0;
	float accellAmountX = 0;
	float accellAmountY = 0;
	float z,x,y = 0;
	bool touched;
	public int currentPolygon = -1;

	public int maxView = 3;

	//private List<int> activePolygons = new List<int>();
	private bool[] activePolygons = new bool[0];
	private	int activeCount = 0;
	private	int processedCount = 0;

	// Use this for initialization
	void Start () {
		transform.Find("playerCamera/touchCollider").gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
		//getCurrentPolygon();
		// Debug.Log(currentPolygon);
		// for(int seg = 0; seg < GlobalData.map.segments.Count; seg++) {
		// 	if (seg != 3){
		// 		GlobalData.map.segments[seg],showHide(false);
		// 	}
		// }

		currentPolygon = -1;

		//gameObject.GetComponent<CharacterController>().enabled = false;
	}
	
		// Update is called once per frame

	void Update () {
		transform.Find("playerCamera").GetComponent<mouselook>().lockCursor = !Input.GetKey(KeyCode.LeftControl);
		CharacterController cc = GetComponent<CharacterController>();
		
		if (Input.GetKey("w") || Input.GetKey("s")) {
			accellAmountZ += accelleration * Time.deltaTime;
		} else {
			accellAmountZ -= accelleration * Time.deltaTime;
		}

		if (Input.GetKey("a") || Input.GetKey("d")) {
			accellAmountX += accelleration * Time.deltaTime;
		} else {
			accellAmountX -= accelleration * Time.deltaTime;
		}

		if (Input.GetKey("space") || Input.GetKey("space")) {
			accellAmountY += accelleration * Time.deltaTime;
		} else {
			accellAmountY -= accelleration * Time.deltaTime;
		}

		if (accellAmountZ > 1) {accellAmountZ = 1;}
		if (accellAmountX > 1) {accellAmountX = 1;}
		if (accellAmountY > 10) {accellAmountY = 1;}
		if (accellAmountZ <= 0) {
			accellAmountZ = 0;
			z = 0;
		}
		if (accellAmountX <= 0) {
			accellAmountX = 0;
			x = 0;
		}
		if (accellAmountY <= 0) {
			accellAmountY = 0;
			y = 0;
		}
		// float z = 0;
		// float x = 0;
		if (Input.GetKey("w")){z = 1;}
		if (Input.GetKey("s")){z = -1;}
		if (Input.GetKey("a")){x = -1;}
		if (Input.GetKey("d")){x = 1;}
		if (Input.GetKey("space")){y = 1;}

		Vector3 fwspeed = transform.forward * accelleration * (z*accellAmountZ);
		Vector3 rtspeed = transform.right * accelleration * (x*accellAmountX);

		Vector3 move = Vector3.ClampMagnitude(fwspeed + rtspeed, maxspeed);

		if (Input.GetKey("space")) {
			// Debug.Log("upp");
			cc.Move(new Vector3(0, 0.03f * accelleration * (y*accellAmountY),0));
		}
		cc.SimpleMove(move);

		if (Input.GetKey("e")){
			transform.Find("playerCamera/touchCollider").gameObject.SetActive(true);


		} else {
			transform.Find("playerCamera/touchCollider").gameObject.SetActive(false);
			touched = false;
		}

		getCurrentPolygon();
		if (currentPolygon >= 0) {
			calculateVisibility();
		}
		if (Input.GetKey("f")){castRay();}

	}

	void getCurrentPolygon() {

		RaycastHit hit;
		//GameObject camera = transform.Find("playerCamera").gameObject;
		if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 50)) {
			if (hit.collider.transform.parent != null && hit.collider.transform.parent.name == "polygon(Clone)") {
				if (hit.collider.transform.parent.GetComponent<MapSegment>().id != currentPolygon) {
					//Debug.Log(hit.collider.transform.parent.GetComponent<MapSegment>().id);
				}
				currentPolygon = hit.collider.transform.parent.GetComponent<MapSegment>().id;
			}
		}

	}

	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		if (!touched){
			MapSegment ms = null;
			if (other.name == "floor" ||other.name == "ceiling" || other.name == "wall" || other.name == "transparent" || other.name == "polygonElement(Clone)"){

				if (other.transform.parent.name == "polygon(Clone)") {
					ms = other.transform.parent.GetComponent<MapSegment>();
				} else if (other.transform.parent.name == "upperPlatform" || other.transform.parent.name == "lowerPlatform") {
					ms =  other.transform.parent.parent.GetComponent<MapSegment>();
				}
				if (ms != null) {
					touched = ms.playerTouch(other.gameObject);
				}
				// Debug.Log(other.)
			}
		}
	}


	void calculateVisibility() {
	// Debug.Log("----------------------------------------");
		float ms = Time.realtimeSinceStartup;
		activePolygons = new bool[GlobalData.map.segments.Count];
		bool[] processedPolys = new bool[GlobalData.map.segments.Count];
		activeCount = 0;
		processedCount = 0;
		float[] distances = new float[GlobalData.map.segments.Count];

		// List<int> processedPolys = new List<int>();

		// Debug.Log(activePolygons.Count);
		// if (activePolygons.Count == 0) {
		//if (!activePolygons.Contains(currentPolygon)){
			activePolygons[currentPolygon] = true;activeCount++;
			foreach(MapSegmentSide side in GlobalData.map.segments[currentPolygon].sides) {
				if (side.meshItem == null) {
					// GlobalData.map.segments[side.connectionID].SetActive(true);
					//if (side.connectionID >= 0 && !activePolygons.Contains(side.connectionID)){
						activePolygons[side.connectionID] = true;activeCount++;
						addToPolygonList(side.connectionID);
						GlobalData.map.segments[currentPolygon].viewEdge = 1;
					//}
				}
			}
			processedPolys[currentPolygon] = true;processedCount++;
		//}

// Debug.Log((Time.realtimeSinceStartup - ms)*1000);
// Debug.Log((Time.realtimeSinceStartup - ms)*1000);
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
// Debug.Log((Time.realtimeSinceStartup - ms)*1000);

			if (closest >=0){
				connections = addToPolygonList(closest);
				processedPolys[closest] = true; 
			}
			processedCount++;
// Debug.Log((Time.realtimeSinceStartup - ms)*1000);

			drawPolygonList(true);
// Debug.Log((Time.realtimeSinceStartup - ms)*1000);

		}
		drawPolygonList(false);
// Debug.Log((Time.realtimeSinceStartup - ms)*1000);

// Debug.Log(activeCount);
	}

	void drawPolygonList (bool showOnly) {
		for (int i = 0; i < activePolygons.Length; i++) {
			if (activePolygons[i] || !showOnly) {
				GlobalData.map.segments[i].showHide(activePolygons[i]);
			}
		}
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

				if (isVisible) {
					if (!backlink) {
						activePolygons[side.connectionID] = true; activeCount++;
						seg.viewEdge = 0;
						GlobalData.map.segments[side.connectionID].viewEdge = 1;
						addCount++;
					} else {
						GlobalData.map.segments[side.connectionID].viewEdge = 0;
						//seg.viewEdge = 0;
					}
				} else if (seg.viewEdge < maxView) {
					if (!backlink && !GlobalData.map.segments[side.connectionID].impossible) {
						activePolygons[side.connectionID] = true; activeCount++;
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


	bool getRectVisibility (Vector3 point1, Vector3 point2, Vector3 height) {
		bool isVisible = false;
		RaycastHit hit;
		GameObject camera;
		camera = transform.Find("playerCamera").gameObject;

		Vector3[] points = new Vector3[8];
		Vector3 p1, p2, p3, h1, h2, h3;
		h1 = height*0.05f;
		h2 = height*0.95f;
		h3 = height*0.5f;
		p1 = (point1-point2)*0.05f;
		p2 = (point1-point2)*0.95f;
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


		Vector3 castPoint;
		// if (PolygonID == 53){
		// Debug.DrawRay(midpoint, camera.transform.position-midpoint, Color.green);
		// Debug.DrawRay(pointa, camera.transform.position-point1, Color.red);
		// Debug.DrawRay(pointb,camera.transform.position-pointb, Color.blue);
		// Debug.DrawRay(pointc, camera.transform.position-pointc, Color.cyan);
		// Debug.DrawRay(pointd, camera.transform.position-pointd, Color.magenta);
		// }
		//Color colour  = Random.ColorHSV();
		float rayCount = 1f;
		isVisible = false;

		isVisible = (Physics.Raycast(midpoint, camera.transform.position-midpoint, out hit, 50)) 
			&& hit.collider.gameObject == camera ;

		for (int i = 1; i <= rayCount && !isVisible; i++) {
			for (int p = 0; p < points.Length && !isVisible; p++){
				castPoint = (points[p]-midpoint)*((1f/rayCount)*(float)i) + midpoint;
				isVisible = (Physics.Raycast(castPoint, camera.transform.position-castPoint, out hit, 50)) 
							&& hit.collider.gameObject == camera ;
				//Debug.DrawRay(castPoint, camera.transform.position-castPoint, colour);
				if (isVisible) {return true;}
			}
		}

		// isVisible = (Physics.Raycast(midpoint, camera.transform.position-midpoint, out hit, 50)) 
		// 				&& hit.collider.gameObject == camera ;
		// if (!isVisible) {
		// 	isVisible = (Physics.Raycast(pointa, camera.transform.position-pointa, out hit, 50)) 
		// 				&& hit.collider.gameObject == camera;
		// }
		// if (!isVisible) {
		// 	isVisible = (Physics.Raycast(pointb, camera.transform.position-pointb, out hit, 50)) 
		// 				&& hit.collider.gameObject == camera;
		// 	}
		// if (!isVisible) {
		// 	isVisible = (Physics.Raycast(pointc, camera.transform.position-pointc, out hit, 50)) 
		// 				&& hit.collider.gameObject == camera;
		// }
		// if (!isVisible) {
		// 	isVisible = (Physics.Raycast(pointd, camera.transform.position-pointd, out hit, 50)) 
		// 				&& hit.collider.gameObject == camera;
		// }

		return isVisible;

	}
	void castRay() {
		RaycastHit hit;
		Vector3 cameraPos;
		// cameraPos = transform.Find("playerCamera").position;
		cameraPos = transform.Find("playerCamera").transform.position;

		if (Physics.Raycast(cameraPos, transform.Find("playerCamera").forward, out hit, 20)) {
			Debug.Log(hit.collider.name);
			Debug.DrawRay(cameraPos, transform.Find("playerCamera").forward, Color.yellow, 5f);

		}
// 		Vector3 point1, point2, midpoint;
// 		MapSegment seg = GlobalData.map.segments[0].GetComponent<MapSegment>();
// 		MapSegmentSide side = GlobalData.map.segments[0].GetComponent<MapSegment>().sides[3];
// 			//if (side.connectionID >= 0 && !activePolygons.Contains(side.connectionID)) {
// 				point1 = seg.vertices[3];
// 				point2 = seg.vertices[4];




// 				point1 = GlobalData.map.segments[0].transform.TransformPoint(point1);
// 				point2 = GlobalData.map.segments[0].transform.TransformPoint(point2);
// 		bool isVisible;		
// 		midpoint = (point1-point2)/2 + point2 + (seg.height/2);
// 				isVisible = (Physics.Raycast(midpoint, cameraPos-midpoint, out hit, 20)) 
// 								&& hit.collider.gameObject == gameObject ;
// 		Debug.Log(hit.collider.gameObject.name);

// Debug.DrawRay(point1, cameraPos-point1, Color.green);
// Debug.DrawRay(point2, cameraPos-point2, Color.green);
// Debug.DrawRay(midpoint, cameraPos-midpoint, Color.green);

		//	}

	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		//touched = false;

		// Rigidbody rb = GetComponent<Rigidbody>();


		// float z = 0;
		// float x = 0;
		// if (Input.GetKey("w")){z += 1;}
		// if (Input.GetKey("s")){z -= 1;}
		// if (Input.GetKey("a")){x -= 1;}
		// if (Input.GetKey("d")){x += 1;}

		// Vector3 fwspeed = transform.forward * accelleration * z;
		// Vector3 rtspeed = transform.right * accelleration * x;
		// if (rb.velocity.magnitude < maxspeed) {
		// 	rb.velocity += fwspeed + rtspeed;
		// }
	}

}
