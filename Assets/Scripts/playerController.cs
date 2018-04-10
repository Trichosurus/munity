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

		int prevPolygon = currentPolygon;
		getCurrentPolygon();
		if (currentPolygon >= 0) {
			if (prevPolygon != currentPolygon) {
				drawActivePolygons();
				if (GlobalData.map.segments[currentPolygon].platform != null &&
						!GlobalData.map.segments[currentPolygon].platform.door) {
					GlobalData.map.segments[currentPolygon].platform.activate(-2);
				}
			}

			calculateVisibility();
		}
		if (Input.GetKey("f")){castRay();}



	}

	void getCurrentPolygon() {

		RaycastHit hit;
		//GameObject camera = transform.Find("playerCamera").gameObject;
		if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 50)) {
			if (hit.collider.transform.parent != null && hit.collider.transform.parent.name == "polygon(Clone)") {
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

	void drawActivePolygons() {
		bool[] active = GlobalData.map.segments[currentPolygon].activePolygons;
		for (int i = 0; i < active.Length; i++) {
			if (!GlobalData.map.segments[i].impossible) {
				GlobalData.map.segments[i].showHide(active[i]);
			} else {
				GlobalData.map.segments[i].showHide(false);
			}
		}


	}

	void calculateVisibility() {
	// Debug.Log("----------------------------------------");
		activePolygons = new bool[GlobalData.map.segments.Count];
		bool[] processedPolys = new bool[GlobalData.map.segments.Count];
		List<int> collides = new List<int>();
		List<int> deferred = new List<int>();
		activeCount = 0;
		processedCount = 0;
		float[] distances = new float[GlobalData.map.segments.Count];
		for (int i = 0; i < GlobalData.map.segments.Count; i++) {
			if (GlobalData.map.segments[i].impossible &&
					GlobalData.map.segments[currentPolygon].activePolygons[i]) {
				activePolygons[i] = true;
				activeCount++;
				GlobalData.map.segments[i].showHide(false);
				//float distance;
				distances[i] = 7777777;
				//for (int s = 0; s < GlobalData.map.segments[i].sides.Count; s++) {
					//if (GlobalData.map.segments[i].sides[s].connectionID != -1) {
						//Vector3 v1 = GlobalData.map.segments[i].vertices[s];
						// Vector2 v2;
						// if (s < GlobalData.map.segments[i].sides.Count-1) {
						// 	v2 = GlobalData.map.segments[i].vertices[s+1];
						// } else {
						// 	v2 = GlobalData.map.segments[i].vertices[0];
						// }
					//	distance = Vector3.Distance(gameObject.transform.position, GlobalData.map.segments[i].transform.TransformPoint(v1));
					//	if (distance < distances[i]) {distances[i] = distance;}
					//}
					// foreach (Vector3 v in GlobalData.map.segments[i].vertices) {
					// 	distance = Vector3.Distance(gameObject.transform.position, GlobalData.map.segments[i].transform.TransformPoint(v));
					// 	if (distance < distances[i]) { distances[i] = distance;}
					// }
				//}
				//distances[i] = Vector3.Distance(gameObject.transform.position, GlobalData.map.segments[i].transform.position);

			}
		}

		MapSegment pol  = GlobalData.map.segments[currentPolygon];

		if (pol.impossible) {
			activePolygons[currentPolygon] = true;
			pol.showHide(true);
			collides.AddRange(pol.collidesWith);
			pol.setClippingPlanes(new List<Vector3>(), true);


		
			for( int s = 0; s < pol.sides.Count; s++) {
				if (pol.sides[s].connectionID >= 0 && 
				GlobalData.map.segments[pol.sides[s].connectionID].impossible) {
					if (!collides.Contains(pol.sides[s].connectionID) ) {
						//GlobalData.map.segments[pol.sides[s].connectionID].showHide(true);
					// } else {
					// 	deferred.Add(pol.sides[s].connectionID);
					// }
						distances[pol.sides[s].connectionID] = 0;
					}
				}
			}
		}
		

		while (processedCount < activeCount) {
			float distance = 7777777;
			int closest = -1;
			// int connections = 0;
			for (int i = 0; i < activePolygons.Length; i++) {
				if (activePolygons[i] && !processedPolys[i]){

					if (distances[i] == 7777777) {
						for (int s = 0; s < GlobalData.map.segments[i].sides.Count; s++) {
							if (GlobalData.map.segments[i].sides[s].connectionID != -1 &&
							(GlobalData.map.segments[GlobalData.map.segments[i].sides[s].connectionID].hidden == false ||
								deferred.Contains(GlobalData.map.segments[i].sides[s].connectionID)) )
							{
								Vector3 v1 = GlobalData.map.segments[i].vertices[s];
								int s2 = s;
								if (s+1 < GlobalData.map.segments[i].vertices.Count) {
									s2 = s+1;
								} else {
									s2 = 0;
								}
								Vector3 v2 = GlobalData.map.segments[i].vertices[s2];

								float d = Vector3.Distance(gameObject.transform.position, GlobalData.map.segments[i].transform.TransformPoint(v1));
								if (d < distances[i]){
									distances[i] = d;
								}
								float d2 = Vector3.Distance(gameObject.transform.position, GlobalData.map.segments[i].transform.TransformPoint(v2));
								if (d2 < distances[i]){
									distances[i] = d2;
								}
							}
						}
					}

					if (distances[i] < distance) {
						distance = distances[i];
						closest = i;
					}
				}
			}
			if (closest == -1) {break;}
			activePolygons[closest] = false;
			for( int s = 0; s < GlobalData.map.segments[closest].sides.Count; s++) {
				if (GlobalData.map.segments[closest].sides[s].connectionID >= 0 &&
					!GlobalData.map.segments[GlobalData.map.segments[closest].sides[s].connectionID].hidden) {
					
					Vector3 point1, point2;
					point1 = GlobalData.map.segments[closest].vertices[s];
					if (s+1 < GlobalData.map.segments[closest].vertices.Count) {
						point2 = GlobalData.map.segments[closest].vertices[s+1];
					} else {
						point2 = GlobalData.map.segments[closest].vertices[0];
					}
					point1 = GlobalData.map.segments[closest].transform.TransformPoint(point1);
					point2 = GlobalData.map.segments[closest].transform.TransformPoint(point2);
					if (getRectVisibility(point1, point2, GlobalData.map.segments[closest].height)) {
						if (!collides.Contains(closest)) {
							GlobalData.map.segments[closest].showHide(true);
							collides.AddRange(GlobalData.map.segments[closest].collidesWith);
						} else {
							deferred.Add(closest);
						}
						foreach(MapSegmentSide side in GlobalData.map.segments[closest].sides) {
							if (side.connectionID != -1) {
								distances[side.connectionID] = 0;
							}
						}
						break;
					}
				}
			}

			GlobalData.map.segments[closest].setClippingPlanes(new List<Vector3>(), true);

			processedPolys[closest] = true; 
			processedCount++;

			// drawPolygonList(true);

		}
		// drawPolygonList(false);
		//Debug.Log(deferred.Count);

		foreach (int i in deferred) {
			float distance = 7777777;
			int closest = -1;
			foreach (int s in GlobalData.map.segments[i].collidesWith) {
				if (distances[s] < distance && GlobalData.map.segments[s].hidden == false) {
					distance = distances[s];
					closest = s;
				}
			}
			if (closest > -1) {
				clipSegments(GlobalData.map.segments[i], GlobalData.map.segments[closest]);
			}
		}
	}

	void clipSegments(MapSegment segment1, MapSegment segment2) {
		GameObject camera;
		camera = transform.Find("playerCamera").gameObject;

		List<Vector2> intersectPoints = new List<Vector2>();
		Vector3 point1a, point2a, point1b, point2b;

		List<int> linesA = new List<int>();
		List<int> linesB = new List<int>();


		//find intersections between polygons
		for (int a = 0; a < segment1.sides.Count; a++) {
			point1a = segment1.vertices[a];
			if (a < segment1.vertices.Count-1) {
				point2a = segment1.vertices[a+1];
			} else {
				point2a = segment1.vertices[0];
			}
			
			point1a = segment1.transform.TransformPoint(point1a);
			point2a = segment1.transform.TransformPoint(point2a);

			float a1 = point2a.z - point1a.z;
			float b1 = point1a.x - point2a.x;
			float c1 = a1*point1a.x + b1*point1a.z;

			for (int b = 0; b < segment2.sides.Count; b++) {
				point1b = segment2.vertices[b];
				if (b < segment2.vertices.Count-1) {
					point2b = segment2.vertices[b+1];
				} else {
					point2b = segment2.vertices[0];
				}
				
				point1b = segment2.transform.TransformPoint(point1b);
				point2b = segment2.transform.TransformPoint(point2b);

				float a2 = point2b.z - point1b.z;
				float b2 = point1b.x - point2b.x;
				float c2 = a2*point1b.x + b2*point1b.z;


				float delta = a1*b2 - a2*b1;
				if(delta != 0) {
					Vector2 intersect = new Vector2((b2*c1 - b1*c2)/delta, (a1*c2 - a2*c1)/delta);
					
					float d1 = Vector2.Distance(new Vector2(point1a.x, point1a.z), intersect);
					float d2 = Vector2.Distance(new Vector2(point2a.x, point2a.z), intersect);
					float d3 = Vector2.Distance(new Vector2(point1a.x, point1a.z), new Vector2(point2a.x, point2a.z));

					float d4 = Vector2.Distance(new Vector2(point1b.x, point1b.z), intersect);
					float d5 = Vector2.Distance(new Vector2(point2b.x, point2b.z), intersect);
					float d6 = Vector2.Distance(new Vector2(point1b.x, point1b.z), new Vector2(point2b.x, point2b.z));

					// if lenghts from end to point to end = length of line then valid point
					//+- 0.01 to account for floating point errors 
					if (d1 + d2 - 0.001 < d3 + 0.001 && d1 + d2 + 0.001 > d3 - 0.001
						 && d4 + d5 - 0.001 < d6 + 0.001 && d4 + d5 + 0.001 > d6 - 0.001) {
						intersectPoints.Add(intersect);
						linesA.Add(a);
						linesB.Add(b);
					}					
				}
			}
		}
		
		Vector2 centerPoint = new Vector2(0,0);
		foreach(Vector2 point in intersectPoints) {
			centerPoint += point;
		}
		centerPoint /= intersectPoints.Count;
		List<float> clockwise = new List<float>();
		for (int i = 0; i < intersectPoints.Count; i++) {
			clockwise.Add(Vector2.SignedAngle(intersectPoints[i]-centerPoint, intersectPoints[0]-centerPoint));
		}
		
		if (clockwise.Count == 0) {
			segment1.showHide(false);
			segment2.showHide(true);
			return;
		}

		//get relevant intersection points
		int[] closest = {-1,-1,-1};
		float distance = 7777777;
		Vector2 pp = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
		for (int i = 0; i < intersectPoints.Count; i++) {
			float d = Vector2.Distance(pp, intersectPoints[i]);
			if (d < distance) {
				distance = d;
				closest[0] = i;
			}
		}


		float below = -777;
		float above = 777;
		float max = -666; 
		float min = 666;
		int clockMax = 0;
		int clockMin = 0;
		for (int i = 0; i < intersectPoints.Count; i++) {
			if (i != closest[0]) {
				if (clockwise[i] > clockwise[closest[0]] && clockwise[i] < above) {
					above = clockwise[i];
					closest[1] = i;
				} else if (clockwise[i] < clockwise[closest[0]] && clockwise[i] > below) {
					below = clockwise[i];
					closest[2] = i;
				}
			}
			if (clockwise[i] > max) {
				max = clockwise[i];
				clockMax = i;
			}
			if (clockwise[i] < min) {
				min = clockwise[i];
				clockMin = i;
			}
		}
		if (closest[1] == -1) {
			if (clockwise[closest[0]] == max) {
				closest[1] = clockMin;
			} else {
				closest[1] = clockMax;
			}
		}
		if (closest[2] == -1) {
			if (clockwise[closest[0]] == min) {
				closest[2] = clockMax;
			} else {
				closest[2] = clockMin;
			}
		}


		Vector2 polyCheck = new Vector2(intersectPoints[closest[1]].x, intersectPoints[closest[1]].y);
		polyCheck -= intersectPoints[closest[0]];
		polyCheck = new Vector2(0f-polyCheck.y, polyCheck.x);
		polyCheck = intersectPoints[closest[0]]/2f + intersectPoints[closest[1]]/2f + new Vector2(polyCheck.x * 0.001f, polyCheck.y * 0.001f);

		List<Vector2> checkPoly = new List<Vector2>();

		for(int i = 0; i < segment1.vertices.Count; i++) {
			checkPoly.Add(new Vector2(segment1.transform.TransformPoint(segment1.vertices[i]).x,segment1.transform.TransformPoint(segment1.vertices[i]).z));
		}
		bool s1 = ContainsPoint(checkPoly.ToArray(), polyCheck);
		bool left = false;
		 //dx=x2-x1 and dy=y2-y1,
		//sort clockwise
		float[] clipAngles = {0,0,0,0};
		clipAngles[0] = Mathf.Atan2(pp.x-intersectPoints[closest[0]].x, pp.y-intersectPoints[closest[0]].y) * Mathf.Rad2Deg;
		//Vector3 p0 = camera.GetComponent<Camera>().WorldToViewportPoint(new Vector3(intersectPoints[closest[0]].x, camera.transform.position.y, intersectPoints[closest[0]].y));
		//Vector3 p1 = new Vector3();
		if (clipAngles[0] < 0 && pp.y < intersectPoints[closest[0]].y) {clipAngles[0] += 360;}
		if (s1) {
			clipAngles[1] = Mathf.Atan2(pp.x-intersectPoints[closest[2]].x, pp.y-intersectPoints[closest[2]].y) * Mathf.Rad2Deg;
		if (clipAngles[1] < 0 && pp.y < intersectPoints[closest[2]].y) {clipAngles[1] += 360;}
			//p1 = camera.GetComponent<Camera>().WorldToViewportPoint(new Vector3(intersectPoints[closest[2]].x, camera.transform.position.y, intersectPoints[closest[2]].y));
		} else {
			clipAngles[1] = Mathf.Atan2(pp.x-intersectPoints[closest[1]].x, pp.y-intersectPoints[closest[1]].y) * Mathf.Rad2Deg;
		if (clipAngles[1] < 0 && pp.y < intersectPoints[closest[1]].y) {clipAngles[1] += 360;}
			//p1 = camera.GetComponent<Camera>().WorldToViewportPoint(new Vector3(intersectPoints[closest[1]].x, camera.transform.position.y, intersectPoints[closest[1]].y));
		}
		// Vector3 p2 = camera.GetComponent<Camera>().WorldToScreenPoint(new Vector3(intersectPoints[closest[2]].x, camera.transform.position.y, intersectPoints[closest[2]].y));

		

		// int temp = closest[0];
		int[] clipPoints = {0,0,0};
		if (clipAngles[0] < clipAngles[1]) {
			left = true;
			clipPoints[0] = closest[0];
			if (s1) {
				clipPoints[1] = closest[2];
				clipPoints[2] = closest[1];
			} else {
				clipPoints[1] = closest[1];
				clipPoints[2] = closest[2];
			}
		} else {
			if (s1) {
				clipPoints[0] = closest[2];
				clipPoints[1] = closest[0];
				clipPoints[2] = closest[1];
			} else {
				clipPoints[0] = closest[1];
				clipPoints[1] = closest[0];
				clipPoints[2] = closest[2];
			}
		}
		// 	closest[0] = closest[1];
		// 	closest[1] = temp;
		// }

		// get angles for clipping planes
		clipAngles[0] = Mathf.Atan2(pp.x-intersectPoints[clipPoints[0]].x, pp.y-intersectPoints[clipPoints[0]].y) * Mathf.Rad2Deg;
		clipAngles[1] = Mathf.Atan2(pp.x-intersectPoints[clipPoints[1]].x, pp.y-intersectPoints[clipPoints[1]].y) * Mathf.Rad2Deg;
		clipAngles[2] = Mathf.Atan2(intersectPoints[clipPoints[1]].x-intersectPoints[clipPoints[0]].x, intersectPoints[clipPoints[1]].y-intersectPoints[clipPoints[0]].y) * Mathf.Rad2Deg;
		if (left) {
			clipAngles[3] = Mathf.Atan2(intersectPoints[clipPoints[2]].x-intersectPoints[clipPoints[0]].x, intersectPoints[clipPoints[2]].y-intersectPoints[clipPoints[0]].y) * Mathf.Rad2Deg;
		} else {
			clipAngles[3] = Mathf.Atan2(intersectPoints[clipPoints[2]].x-intersectPoints[clipPoints[1]].x, intersectPoints[clipPoints[2]].y-intersectPoints[clipPoints[1]].y) * Mathf.Rad2Deg;
		}
		// Debug.Log(angle1);
		// Debug.Log(angle2);
		List<Vector3> planes = new List<Vector3>();
		planes.Add(camera.transform.position);
		planes.Add(new Vector3(90, 180, 90-clipAngles[0]));
		planes.Add(camera.transform.position);
		planes.Add(new Vector3(90, 0, 90-clipAngles[1]));
		planes.Add(new Vector3(intersectPoints[clipPoints[0]].x, camera.transform.position.y, intersectPoints[clipPoints[0]].y));
		if (left) {
			planes.Add(new Vector3(90, 0, 90-clipAngles[3]));
		} else {
			planes.Add(new Vector3(90, 0, 90-clipAngles[3]));
		}

		segment1.setClippingPlanes(planes, true);

		planes.Clear();
		planes.Add(camera.transform.position);
		planes.Add(new Vector3(90, 0, 90-clipAngles[0]));
		planes.Add(camera.transform.position);
		planes.Add(new Vector3(90, 180, 90-clipAngles[1]));
		planes.Add(new Vector3(intersectPoints[clipPoints[0]].x, camera.transform.position.y, intersectPoints[clipPoints[0]].y));
		planes.Add(new Vector3(90, 180, 90-clipAngles[2]));

		segment2.setClippingPlanes(planes, false);

		segment1.showHide(true);
		segment2.showHide(true);



		Vector3 r0 = new Vector3(intersectPoints[clipPoints[0]].x, camera.transform.position.y, intersectPoints[clipPoints[0]].y);
		Vector3 r1 = new Vector3(intersectPoints[clipPoints[1]].x, camera.transform.position.y, intersectPoints[clipPoints[1]].y);
		Vector3 r2 = new Vector3(intersectPoints[clipPoints[2]].x, camera.transform.position.y, intersectPoints[clipPoints[2]].y);
		Debug.DrawRay(r0, camera.transform.position-r0, Color.magenta);
		Debug.DrawRay(r1, camera.transform.position-r1, Color.green);
		Debug.DrawRay(r2, camera.transform.position-r2, Color.cyan);
		//Debug.DrawRay(new Vector3(intersectPoints[clipPoints[3]].x, camera.transform.position.y, intersectPoints[clipPoints[3]].y), camera.transform.position, Color.magenta);

		Vector3 r3 = new Vector3(intersectPoints[clipPoints[0]].x, camera.transform.position.y, intersectPoints[clipPoints[0]].y);
		if (!left) {
			r3 = new Vector3(intersectPoints[clipPoints[1]].x, camera.transform.position.y, intersectPoints[clipPoints[1]].y);	
		}
		Debug.DrawRay(r0, r1-r0, Color.blue);
		Debug.DrawRay(r0, r2-r3, Color.yellow);

		// Debug.DrawRay(r0, r1-r0, Color.blue);
		// Debug.DrawRay(r0, r2-r0, Color.yellow);



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


	void drawPolygonList (bool showOnly) {
		for (int i = 0; i < activePolygons.Length; i++) {
			if (GlobalData.map.segments[i].impossible && GlobalData.map.segments[currentPolygon].activePolygons[i] &&
					(activePolygons[i] || !showOnly)) {
				GlobalData.map.segments[i].showHide(activePolygons[i]);
			}
		}
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
		if (camera.transform.position.y > p1.y && camera.transform.position.y < p1.y+height.y ) {
			points.Add(p3 + point2 + new Vector3(0, camera.transform.position.y - p1.y,0 ));
			points.Add(p3 + point2 + new Vector3(0, camera.transform.position.y - p1.y - 0.7f, 0));
		}


		Vector3 castPoint;
		float rayCount = 3f;
		isVisible = false;

		isVisible = (Physics.Raycast(midpoint, camera.transform.position-midpoint, out hit, 50)) 
			&& hit.collider.gameObject == camera ;

		if (isVisible) {
			Debug.DrawRay(midpoint, camera.transform.position-midpoint, Color.red);
		} else {
			Debug.DrawRay(midpoint, camera.transform.position-midpoint, Color.green);
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
					Debug.DrawRay(castPoint, camera.transform.position-castPoint, Color.green);
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
