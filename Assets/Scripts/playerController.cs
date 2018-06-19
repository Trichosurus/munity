﻿using System.Collections;
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

	private List<int> activePolygons;
	// private bool[] activePolygons = new bool[0];
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

		if (!GlobalData.playerLight) {
			gameObject.GetComponent<Light>().enabled = false;
		}

		//gameObject.GetComponent<CharacterController>().enabled = false;
	}
	
		// Update is called once per frame

	void Update () {
		if (GlobalData.captureMouse) {
			transform.Find("playerCamera").GetComponent<mouselook>().lockCursor = !Input.GetKey(KeyCode.LeftControl);
		} else {
			transform.Find("playerCamera").GetComponent<mouselook>().lockCursor = Input.GetKey(KeyCode.LeftControl);
	
		}
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

		int prevPolygon = currentPolygon;
		getCurrentPolygon();
		if (currentPolygon >= 0) {
			if (prevPolygon != currentPolygon) {
				activePolygons = new List<int>(); 
				drawActivePolygons();
				GlobalData.map.segments[currentPolygon].triggerBehaviour();
			}

			calculateVisibility();
		}
		if (Input.GetKey("f")){castRay();}



	}

	void getCurrentPolygon() {

		RaycastHit hit;
		//GameObject camera = transform.Find("playerCamera").gameObject;
		if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 50)) {
			if (hit.collider.transform.parent != null && hit.collider.transform.parent.tag == "polygon") {
				// if (hit.collider.transform.parent.GetComponent<MapSegment>().id != currentPolygon) {
				// 	Debug.Log(hit.collider.transform.parent.GetComponent<MapSegment>().id);
				// }
				currentPolygon = hit.collider.transform.parent.GetComponent<MapSegment>().id;
			} else if (hit.collider.transform.parent != null && (hit.collider.transform.parent.name == "upperPlatform" || hit.collider.transform.parent.name == "lowerPlatform")) {
				// if (hit.collider.transform.parent.GetComponent<MapSegment>().id != currentPolygon) {
				// 	Debug.Log(hit.collider.transform.parent.GetComponent<MapSegment>().id);
				// }
				currentPolygon = hit.collider.transform.parent.parent.GetComponent<MapSegment>().id;
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
			//if (other.name == "floor" ||other.name == "ceiling" || other.name == "wall" || other.name == "transparent" || other.name == "polygonElement(Clone)"){

				if (other.transform.parent.tag == "polygon") {
					ms = other.transform.parent.GetComponent<MapSegment>();
				} else if (other.transform.parent.name == "upperPlatform" || other.transform.parent.name == "lowerPlatform") {
					ms =  other.transform.parent.parent.GetComponent<MapSegment>();
				}
				if (ms != null) {
					touched = ms.playerTouch(other.gameObject);
				}
				// Debug.Log(other.)
			//}
		}
	}

	void drawActivePolygons() {
		bool[] active = GlobalData.map.segments[currentPolygon].activePolygons;
		for (int i = 0; i < active.Length; i++) {
			if (!GlobalData.map.segments[i].impossible) {
				GlobalData.map.segments[i].showHide(active[i]);
			} else {
				GlobalData.map.segments[i].showHide(false);
				if (active[i]) {activePolygons.Add(i);};
			}
		}
	}
	void calculateVisibility() {
		//get list of active volumes that are visible
		List<impossibleVolume> ivs = new List<impossibleVolume>();
		List<int> ap = new List<int>(activePolygons);
		while (ap.Count > 0) {
			foreach(impossibleVolume iv in GlobalData.map.segments[ap[0]].impossibleVolumes) {
				ivs.Add(iv);
				foreach(int pol in iv.collisionPolygonsSelf) {
					ap.Remove(pol);
					GlobalData.map.segments[pol].showHide(false);
				}
			}
		}
		int selfIV = -1;
		List<float[]> distances = new List<float[]>();
		//get the distances of the closest connection to each impossible volume
		for (int i = 0; i < ivs.Count; i++) {
			foreach (int pol in ivs[i].collisionPolygonsSelf) {
				if (pol == currentPolygon) {selfIV = i;}
				// foreach(MapSegmentSide side in GlobalData.map.segments[pol].sides) {
				for (int s = 0; s < GlobalData.map.segments[pol].sides.Count; s++) {
					MapSegmentSide side = GlobalData.map.segments[pol].sides[s];
					if (side.connectionID > -1 
					&& GlobalData.map.segments[currentPolygon].activePolygons[side.connectionID] 
					&& !ivs[i].collisionPolygonsSelf.Contains(side.connectionID)) {
							//calculatedistance
						GameObject entryway = side.entryCollider;
						if (entryway == null) {
							entryway = new GameObject("sideCollider");
							entryway.transform.position = GlobalData.map.segments[pol].transform.position;
							BoxCollider box = entryway.AddComponent<BoxCollider>();

							Vector3 v1 = GlobalData.map.segments[pol].vertices[s];
							int s2 = s;
							if (s+1 < GlobalData.map.segments[pol].vertices.Count) {
								s2 = s+1;
							} else {
								s2 = 0;
							}
							Vector3 v2 = GlobalData.map.segments[pol].vertices[s2];
							v1 = GlobalData.map.segments[pol].transform.TransformPoint(v1);
							v2 = GlobalData.map.segments[pol].transform.TransformPoint(v2);

							box.transform.position = Vector3.Lerp(v1,v2,0.5f) + (GlobalData.map.segments[pol].height/2f);
							box.size = new Vector3(Vector3.Distance(v1,v2), GlobalData.map.segments[pol].height.y, 0.02f);
							float entrywayAngle = Mathf.Atan2(v2.x-v1.x, v2.z-v1.z) * Mathf.Rad2Deg;
							box.transform.rotation = Quaternion.Euler(0,entrywayAngle+90,0);
							box.enabled = false;
							entryway.transform.parent = GlobalData.map.segments[pol].transform;
							GlobalData.map.segments[pol].sides[s].entryCollider = entryway;
						}
		
						Collider coll = entryway.GetComponent<Collider>();
						coll.enabled = true;
						Vector3 closestPoint = coll.ClosestPointOnBounds(gameObject.transform.position);
						coll.enabled = false;
						float d = Vector3.Distance(closestPoint,gameObject.transform.position);
						// if (d < distance){
							// distance = d;
							distances.Add(new [] {d, i, pol, s});
						// }
					}
				}
			}
		}
		//distances.Sort();
		List<float[]> temp = new List<float[]>();
		while (distances.Count > 0) {
			float d = 7777777f;
			float[] lastf = distances[0];
			foreach(float[] f in distances) {
				if (f[0] < d) {d = f[0]; lastf = f;}
			}
			temp.Add(lastf);
			distances.Remove(lastf);
		}
		distances = temp;
		// if we are in impossible space, then draw the volume we are in before doing anything else
		if (selfIV >=0) {
			showVolume(ivs[selfIV]);
		}

		//draw impossible volumes in order of their closest connection to normal pols
		for (int d = 0; d < distances.Count; d++) {
			Vector3 point1, point2;
			point1 = GlobalData.map.segments[(int)distances[d][2]].vertices[(int)distances[d][3]];
			if ((int)distances[d][3]+1 < GlobalData.map.segments[(int)distances[d][2]].vertices.Count) {
				point2 = GlobalData.map.segments[(int)distances[d][2]].vertices[(int)distances[d][3]+1];
			} else {
				point2 = GlobalData.map.segments[(int)distances[d][2]].vertices[0];
			}
			point1 = GlobalData.map.segments[(int)distances[d][2]].transform.TransformPoint(point1);
	 		point2 = GlobalData.map.segments[(int)distances[d][2]].transform.TransformPoint(point2);

			if (getRectVisibility(point1, point2, GlobalData.map.segments[(int)distances[d][2]].height)) {
				showVolume(ivs[(int)distances[d][1]]);
				foreach(int i in ivs[(int)distances[d][1]].collisionPolygonsOther) {
					if (!GlobalData.map.segments[i].hidden) {
						int other = -1;
						for (int o = 0; o < distances.Count; o++) {
							if (o != d && ivs[(int)distances[o][1]].collisionPolygonsSelf.Contains(i)) {
								other = o; 
								break;
							}
						}
						calculateClipping(ivs, distances, d, other);
						break;
					}
				}
			}
		}

	}

	void showVolume(impossibleVolume iv, bool show = true) {
		foreach (int pol in iv.collisionPolygonsSelf) {
			if (GlobalData.map.segments[currentPolygon].collidesWith != null &&
			GlobalData.map.segments[currentPolygon].collidesWith.Contains(pol) &&
			iv.collisionPolygonsSelf.Contains(currentPolygon)) {
				GlobalData.map.segments[pol].showHide(false);
			} else {
				GlobalData.map.segments[pol].showHide(show);
				GlobalData.map.segments[pol].setClippingPlanes(new List<Vector3>());
			}
		}
	}

	void calculateClipping (List<impossibleVolume> ivs, List<float[]> distances, int volSelf, int volOther) {
		impossibleVolume iv = ivs[(int)distances[volSelf][1]];
		GameObject camera;
		camera = transform.Find("playerCamera").gameObject;
		Vector3 pp = camera.transform.position;

		//get clockwise entry point

		List<float> clipAngles = new List<float>();
		float distance = 7777777f;
		int closest = 0;
		for (int i = 0; i < iv.collisionPoints.Count; i++) {
			//distances.Add(Vector2.Distance(new Vector2(vec.x, vec.z), new Vector2(pp.x, pp.z)));
			float d = Vector2.Distance(new Vector2(iv.collisionPoints[i].x, iv.collisionPoints[i].z), new Vector2(pp.x, pp.z));
			if (d < distance) {
				distance = d;
				closest = i;
			}
		}
		int clockwise = closest +1;
		int counterClockwise = closest -1;
		if (clockwise >= iv.collisionPoints.Count) {clockwise = 0;}
		if (counterClockwise < 0) {counterClockwise = iv.collisionPoints.Count-1;}

		clipAngles.Add(Mathf.Atan2(pp.x-iv.collisionPoints[closest].x, pp.z-iv.collisionPoints[closest].z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(pp.x-iv.collisionPoints[clockwise].x, pp.z-iv.collisionPoints[clockwise].z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(pp.x-iv.collisionPoints[counterClockwise].x, pp.z-iv.collisionPoints[counterClockwise].z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(iv.collisionPoints[closest].x-iv.collisionPoints[clockwise].x, iv.collisionPoints[closest].z-iv.collisionPoints[clockwise].z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(iv.collisionPoints[closest].x-iv.collisionPoints[counterClockwise].x, iv.collisionPoints[closest].z-iv.collisionPoints[counterClockwise].z) * Mathf.Rad2Deg);


		List<float> sideAngles = new List<float>();
		List<Vector3> points = new List<Vector3>();
		Vector3 point1 = new Vector3(0,0,0);
		Vector3	point2 = new Vector3(0,0,0);


		//Debug.Log(clipAngles[0] +"::"+ clipAngles[1] +"::"+ clipAngles[2]);
		points.Add(GlobalData.map.segments[(int)distances[volSelf][2]].vertices[(int)distances[volSelf][3]]);
		if ((int)distances[volSelf][3] >= GlobalData.map.segments[(int)distances[volSelf][2]].vertices.Count-1) {
			points.Add(GlobalData.map.segments[(int)distances[volSelf][2]].vertices[0]);
		} else {
			points.Add(GlobalData.map.segments[(int)distances[volSelf][2]].vertices[(int)distances[volSelf][3]+1]);
		}

		points.Add(GlobalData.map.segments[(int)distances[volOther][2]].vertices[(int)distances[volOther][3]]);
		if ((int)distances[volOther][3] >= GlobalData.map.segments[(int)distances[volOther][2]].vertices.Count-1) {
			points.Add(GlobalData.map.segments[(int)distances[volOther][2]].vertices[0]);
		} else {
			points.Add(GlobalData.map.segments[(int)distances[volOther][2]].vertices[(int)distances[volOther][3]+1]);
		}

	
		points[0] = GlobalData.map.segments[(int)distances[volSelf][2]].transform.TransformPoint(points[0]);
		points[1] = GlobalData.map.segments[(int)distances[volSelf][2]].transform.TransformPoint(points[1]);
		points[2] = GlobalData.map.segments[(int)distances[volOther][2]].transform.TransformPoint(points[2]);
		points[3] = GlobalData.map.segments[(int)distances[volOther][2]].transform.TransformPoint(points[3]);
		distance = 7777777;
		if (iv.collisionPolygonsSelf[0] == 43) {
			;
		}
		foreach (Vector3 point in points) {
			foreach (Vector3 p in points) {
				float d = Vector3.Distance(point,p);
				if (d > 0 && d < distance) {
					point1 = point;
					point2 = p;
					distance = d;
				}
			}
		}

		sideAngles.Add(Mathf.Atan2(pp.x-point1.x, pp.z-point1.z) * Mathf.Rad2Deg);
		sideAngles.Add(Mathf.Atan2(pp.x-point2.x, pp.z-point2.z) * Mathf.Rad2Deg);
		sideAngles.Add(Mathf.Atan2(point1.x-point2.x, point1.z-point2.z) * Mathf.Rad2Deg);

		for (int i = 0; i < sideAngles.Count; i++) {
			if (sideAngles[i] < 0) {sideAngles[i] = 360f + sideAngles[i];}
			Debug.Log(sideAngles[i]);
		}
			if (clipAngles[4] < 0) {clipAngles[4] = 360f + clipAngles[4];}
		//Debug.Log(ivs[(int)distances[volSelf][1]].collisionPolygonsSelf[0] + "::" + ivs[(int)distances[volOther][1]].collisionPolygonsSelf[0] + "::" + sideAngles[0] + "::" + sideAngles[1] + "::" + sideAngles[2] + "::" + sideAngles[3]);
		// Debug.Log(ivs[(int)distances[volSelf][1]].collisionPolygonsSelf[0] + "::" + ivs[(int)distances[volOther][1]].collisionPolygonsSelf[0] + "::" + point1 + "::" + point2 + "::" + point3 + "::" + point4);
		Debug.Log(point1 + "::" + point2);
		// Debug.Log (sideAngles[3] - sideAngles[2]);

		// if (sideAngles[1] - sideAngles[0] < -180 || sideAngles[1] - sideAngles[0] >=0) {
		// 	s1 = 0;
		// }
		// if (sideAngles[3] - sideAngles[2] < -180 || sideAngles[3] - sideAngles[2] >=0) {
		// 	s2 = 3;
		// }

		// Debug.Log(s1 + "::" + s2);
		// Debug.Log(180 - Mathf.Abs(Mathf.Abs(sideAngles[1] - sideAngles[0]) - 180));
		// Debug.Log(180 - Mathf.Abs(Mathf.Abs(sideAngles[3] - sideAngles[2]) - 180));
		Debug.Log(iv.collisionPolygonsSelf[0]);
		Debug.Log(sideAngles[0] + "::" + sideAngles[1]);
		Debug.Log(sideAngles[2]);
		Debug.Log(clipAngles[0] + "::" + clipAngles[1] + "::" + clipAngles[2]);
		Debug.Log(clipAngles[3] + "::" + clipAngles[4]);
		Debug.Log(clipAngles[4]); 
		
		Debug.Log(sideAngles[1] - sideAngles[0]);
		//if (sideAngles[1] - sideAngles[0] > -180 && sideAngles[1] - sideAngles[0] < 0 || sideAngles[1] - sideAngles[0] > 180 ) {
		if ((sideAngles[2] > 180 && clipAngles[4] < 180)
		|| (sideAngles[2] <= 180 && clipAngles[4] >= 180)) {
			clipPlanes(pp,pp,iv.collisionPoints[closest],clipAngles[0], clipAngles[1], clipAngles[3], true, true, iv.collisionPolygonsSelf);
			clipPlanes(pp,pp,iv.collisionPoints[closest],clipAngles[0], clipAngles[2], clipAngles[4], true, false, iv.collisionPolygonsOther);
			Debug.Log("self");
		} else {
			clipPlanes(pp,pp,iv.collisionPoints[closest],clipAngles[0], clipAngles[1], clipAngles[3], true, true,iv.collisionPolygonsOther);
			clipPlanes(pp,pp,iv.collisionPoints[closest],clipAngles[0], clipAngles[2], clipAngles[4], true, false, iv.collisionPolygonsSelf);
			Debug.Log("other");
		}

		Debug.DrawRay(point1,pp-point1,  Color.cyan);
		Debug.DrawRay(point2,pp-point2,  Color.cyan);
		// Debug.DrawRay(point3,pp-point3,  Color.cyan);
		// Debug.DrawRay(point4,pp-point4,  Color.cyan);


		Debug.DrawRay(iv.collisionPoints[closest],pp-iv.collisionPoints[closest],  Color.blue);
		Debug.DrawRay(iv.collisionPoints[clockwise],pp-iv.collisionPoints[clockwise],  Color.yellow);
		Debug.DrawRay(iv.collisionPoints[counterClockwise],pp-iv.collisionPoints[counterClockwise],  Color.green);

	}

	void clipPlanes (Vector3 p1, Vector3 p2, Vector3 p3, float a1, float a2, float a3, bool additive, bool inverse, List<int> segments) {
		float angle = 0f;
		if (inverse) {angle = 180f;}
		List<Vector3> planes = new List<Vector3>();
		planes.Add(p1);
		planes.Add(new Vector3(90, 180-angle, 90-a1));
		planes.Add(p1);
		planes.Add(new Vector3(90, 0+angle, 90-a2));
		planes.Add(new Vector3(p3.x, p1.y, p3.z));
		planes.Add(new Vector3(90, 0+angle, 90-a3));
		foreach(int pol in segments) {
			GlobalData.map.segments[pol].setClippingPlanes(planes, additive);
		}
	}
	
	bool ContainsPoint(Vector2[] polyPoints, Vector2 p) { 
   		int j = polyPoints.Length-1; 
   		bool inside = false; 
		for (int i = 0; i < polyPoints.Length; j = i++) { 
			if ( ((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) && 
				(p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x)) 
				inside = !inside; 
		} 
		return inside; 
	}


	bool getRectVisibility (Vector3 point1, Vector3 point2, Vector3 height) {
		bool isVisible = false;
		RaycastHit hit;
		GameObject camera;
		camera = transform.Find("playerCamera").gameObject;

		List<Vector3> points = new List<Vector3>();
		Vector3 p1, p2, p3, h1, h2, h3;
		h1 = height*0.005f;
		h2 = height*0.995f;
		h3 = height*0.5f;
		p1 = (point1-point2)*0.005f;
		p2 = (point1-point2)*0.995f;
		p3 = (point1-point2)*0.5f;
		

		Vector3 midpoint = p3 + point2 + h3;

		points.Add(p1 + point2 + h1);
		points.Add(p2 + point2 + h1);
		points.Add(p1 + point2 + h2);
		points.Add(p2 + point2 + h2);
		points.Add(p3 + point2 + h2);
		points.Add(p3 + point2 + h1);
		points.Add(p1 + point2 + h3);
		points.Add(p2 + point2 + h3);
		if (camera.transform.position.y > point1.y && camera.transform.position.y < point1.y+height.y ) {
			points.Add(p3 + point2 + new Vector3(0, camera.transform.position.y - point1.y,0 ));
			points.Add(p3 + point2 + new Vector3(0, camera.transform.position.y - point1.y + 0.7f, 0));
		}


		Vector3 castPoint;
		float rayCount = 3f;
		isVisible = false;

		isVisible = (Physics.Raycast(midpoint, camera.transform.position-midpoint, out hit, 50)) 
			&& hit.collider.gameObject == camera ;

		if (isVisible) {
			Debug.DrawRay(midpoint, camera.transform.position-midpoint, Color.red);
		} else {
			//Debug.DrawRay(midpoint, camera.transform.position-midpoint, Color.green);
		}

		Color colour  = Random.ColorHSV();
		for (int i = 1; i <= rayCount && !isVisible; i++) {
			for (int p = 0; p < points.Count && !isVisible; p++){
				castPoint = (points[p]-midpoint)*((1f/rayCount)*(float)i) + midpoint;
				isVisible = (Physics.Raycast(castPoint, camera.transform.position-castPoint, out hit, 50)) 
							&& hit.collider.gameObject == camera ;
				if (isVisible) {
					Debug.DrawRay(castPoint, camera.transform.position-castPoint, Color.red);
					return true;
				} else {
					//Debug.DrawRay(castPoint, camera.transform.position-castPoint, Color.green);
				}
			}
		}

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
