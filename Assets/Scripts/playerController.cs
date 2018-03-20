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

	private List<int> activePolygons = new List<int>();

	// Use this for initialization
	void Start () {
		transform.Find("playerCamera/touchCollider").gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
		//getCurrentPolygon();
		Debug.Log(currentPolygon);
		// for(int seg = 0; seg < GlobalData.map.segments.Count; seg++) {
		// 	if (seg != 3){
		// 		GlobalData.map.segments[seg].GetComponent<MapSegment>().showHide(false);
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
			if (other.name == "polygonElement(Clone)"){

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
		// if (other.name == "transparent") {
			
		// 	if (other.transform.parent.GetComponent<MapSegment>().id != currentPolygon) {
		// 		currentPolygon = other.transform.parent.GetComponent<MapSegment>().id;
		// 	}
		// 	Debug.Log(currentPolygon);
		// }
	}


	void calculateVisibility() {
		activePolygons.Clear();
		List<int> processedPolys = new List<int>();

		// if (activePolygons.Count == 0) {
		activePolygons.Add(currentPolygon);
		foreach(MapSegmentSide side in GlobalData.map.segments[currentPolygon].GetComponent<MapSegment>().sides) {
			if (side.meshItem == null) {
				// GlobalData.map.segments[side.connectionID].SetActive(true);
				if (side.connectionID >= 0){
					activePolygons.Add(side.connectionID);
					addToPolygonList(side.connectionID);
				}
			}
		}
		processedPolys.Add(currentPolygon);

		//Debug.Log(activePolygons);
		//drawPolygonList();
		//int totalAdded = 0;

		while (processedPolys.Count < activePolygons.Count) {
		float distance = 100000;
		int closest = -1;
		foreach(int i in activePolygons) {
			if (!processedPolys.Contains(i)){
				float d = Vector3.Distance(gameObject.transform.position, GlobalData.map.segments[i].transform.position);
				if (d < distance) {distance = d;closest = i;}
			}
		}
		addToPolygonList(closest);
		processedPolys.Add(closest);
		drawPolygonList();
		}

		//Debug.Log(activePolygons.Count);
		for(int seg = 0; seg < GlobalData.map.segments.Count; seg++) {
			if (!activePolygons.Contains(seg)) {
				GlobalData.map.segments[seg].GetComponent<MapSegment>().showHide(false);
			}
		}




	}

	void castRay() {
		RaycastHit hit;
		Vector3 cameraPos;
		// cameraPos = transform.Find("playerCamera").position;
		cameraPos = transform.position;

		if (Physics.Raycast(cameraPos, transform.Find("playerCamera").forward, out hit, 20)) {
			Debug.Log(hit.collider.name);

		}
		Vector3 point1, point2, midpoint;
MapSegment seg = GlobalData.map.segments[0].GetComponent<MapSegment>();
MapSegmentSide side = GlobalData.map.segments[0].GetComponent<MapSegment>().sides[3];
			//if (side.connectionID >= 0 && !activePolygons.Contains(side.connectionID)) {
				point1 = seg.vertices[3];
				point2 = seg.vertices[4];




				point1 = GlobalData.map.segments[0].transform.TransformPoint(point1);
				point2 = GlobalData.map.segments[0].transform.TransformPoint(point2);
		bool isVisible;		
		midpoint = (point1-point2)/2 + point2 + (seg.height/2);
				isVisible = (Physics.Raycast(midpoint, cameraPos-midpoint, out hit, 20)) 
								&& hit.collider.gameObject == gameObject ;
		Debug.Log(hit.collider.gameObject.name);

Debug.DrawRay(point1, cameraPos-point1, Color.green);
Debug.DrawRay(point2, cameraPos-point2, Color.green);
Debug.DrawRay(midpoint, cameraPos-midpoint, Color.green);

		//	}

	}


	void drawPolygonList () {
		foreach (int i in activePolygons) {
			GlobalData.map.segments[i].GetComponent<MapSegment>().showHide(true);
		}
	}



	void addToPolygonList (int PolygonID) {
		if (PolygonID <0) {return;}
		
		bool isVisible;
		RaycastHit hit;
		GameObject camera;
		camera = transform.Find("playerCamera").gameObject;
		// int addCount = 0;
		activePolygons.Add(PolygonID);
		// if (activePolygons.Count < 100) {
		MapSegment seg = GlobalData.map.segments[PolygonID].GetComponent<MapSegment>();
		for( int s = 0; s < seg.sides.Count; s++) {
			MapSegmentSide side = GlobalData.map.segments[PolygonID].GetComponent<MapSegment>().sides[s];
			if (side.connectionID >= 0 && !activePolygons.Contains(side.connectionID)) {
				isVisible = false;
				Vector3 point1, point2, midpoint, pointa, pointb, pointc, pointd;
				point1 = seg.vertices[s];
				if (s+1 < seg.vertices.Count) {
					point2 = seg.vertices[s+1];
				} else {
					point2 = seg.vertices[0];
				}
				point1 = GlobalData.map.segments[PolygonID].transform.TransformPoint(point1);
				point2 = GlobalData.map.segments[PolygonID].transform.TransformPoint(point2);
				midpoint = (point1-point2)/2 + point2 + (seg.height/2);
				pointa = (point1-point2)*0.05f + point2  + (seg.height*0.05f);
				pointb = (point1-point2)*0.95f + point2  + (seg.height*0.05f);

				pointc = (point1-point2)*0.05f + point2  + (seg.height*0.95f);
				pointd = (point1-point2)*0.95f + point2  + (seg.height*0.95f);


if (PolygonID == 53){
Debug.DrawRay(midpoint, camera.transform.position-midpoint, Color.green);
Debug.DrawRay(pointa, camera.transform.position-point1, Color.red);
Debug.DrawRay(pointb,camera.transform.position-pointb, Color.blue);
Debug.DrawRay(pointc, camera.transform.position-pointc, Color.cyan);
Debug.DrawRay(pointd, camera.transform.position-pointd, Color.magenta);
}
				isVisible = (Physics.Raycast(midpoint, camera.transform.position-midpoint, out hit, 20)) 
								&& hit.collider.gameObject == camera ;
// Debug.Log((point1-point2)/2 + point2 + (seg.height/2));
// Debug.Log(hit.collider.transform.parent.gameObject.GetComponent<MapSegment>().id);
//  Debug.Log(hit.collider.name);
if (PolygonID == 53){
	Debug.Log(hit.collider.gameObject.name);
	Debug.Log(isVisible);

}

				if (!isVisible) {
					isVisible = (Physics.Raycast(pointa, camera.transform.position-pointa, out hit, 20)) 
								&& hit.collider.gameObject == camera;
				}
if (PolygonID == 53){
	Debug.Log(hit.collider.gameObject.name);
	Debug.Log(isVisible);

}
				if (!isVisible) {
					isVisible = (Physics.Raycast(pointb, camera.transform.position-pointb, out hit, 20)) 
								&& hit.collider.gameObject == camera;
					}
if (PolygonID == 53){
	Debug.Log(hit.collider.gameObject.name);
	Debug.Log(isVisible);

}

				if (!isVisible) {
					isVisible = (Physics.Raycast(pointc, camera.transform.position-pointc, out hit, 20)) 
								&& hit.collider.gameObject == camera;
				}
if (PolygonID == 53){
	Debug.Log(hit.collider.gameObject.name);
	Debug.Log(isVisible);

}
				if (!isVisible) {
					isVisible = (Physics.Raycast(pointd, camera.transform.position-pointd, out hit, 20)) 
								&& hit.collider.gameObject == camera;
				}
if (PolygonID == 53){
	Debug.Log(hit.collider.gameObject.name);
	Debug.Log(isVisible);

}
				if (isVisible) {
					activePolygons.Add(side.connectionID);
					// addCount++;
				}
			}
		}
		// }
		// return addCount;
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
