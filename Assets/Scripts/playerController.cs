using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
	public float accelleration = 1;
	public float maxspeed = 3;

	bool touched;
	public int currentPolygon = -1;

	public int maxView = 3;

	private List<int> activePolygons;
	// private bool[] activePolygons = new bool[0];
	private	int activeCount = 0;
	private	int processedCount = 0;

	private float ambientLoopTimer = 0;
	private AmbientSound ambient = null;

	private float randomSoundTimer = 0;
	private RandomSound randomSound = null;
	private float randomSoundNext = 1;
	private List<GameObject> randomSounds = new List<GameObject>();
	private Vector3 velocity = new Vector3(0,0,0);

	public playerPhysics running = new playerPhysics();
	public playerPhysics walking = new playerPhysics();
	public playerPhysics phys = new playerPhysics();
	

	private bool airborne = false;
	private bool climbing = false;
	private bool swimming = false;
	private bool vis = false;
	private GameObject lastTouch = null;
	private GameObject playerCollider;
	private GameObject playerLight;
	private GameObject playerCamera;
	private List<Collider> floorContacts = new List<Collider>();
	private List<Collider> wallContacts = new List<Collider>();
	public Collider platContactU = null;
	public Collider platContactL = null;



	// Use this for initialization
	void Start () {
		// transform.Find("playerCamera/touchCollider").gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
		//getCurrentPolygon();
		// Debug.Log(currentPolygon);
		// for(int seg = 0; seg < GlobalData.map.segments.Count; seg++) {
		// 	if (seg != 3){
		// 		GlobalData.map.segments[seg],showHide(false);
		// 	}
		// }

		currentPolygon = -1;
		playerCollider = transform.Find("playerCollider").gameObject;
		playerLight = transform.Find("playerLight").gameObject;
		playerCamera = transform.Find("playerCamera").gameObject;





	// walking standard physics, remove when physics files get read
		walking.maxForwardSpeed = 0.0714f * 30f;
		walking.maxBackwardSpeed = 0.0588f * 30f;
		walking.maxPerpSpeed = 0.0500f * 30f;
		walking.acceleration = 0.0050f * 30f;
		walking.climbingAcceleration = 0.0033f * 30f;

		updatePlayerSettings();

	}


	public void updatePlayerSettings() {
		playerLight.GetComponent<Light>().enabled = GlobalData.playerLight;
		playerLight.GetComponent<Light>().intensity = GlobalData.playerLightIntensity;
		playerLight.GetComponent<Light>().range = GlobalData.playerLightRange;


		switch (GlobalData.playerLightType) {
			case 0:
				playerLight.GetComponent<Light>().shadows = LightShadows.None;
				playerLight.GetComponent<Light>().type = LightType.Point;
				break;
			case 1:
				playerLight.GetComponent<Light>().shadows = LightShadows.Hard;
				playerLight.GetComponent<Light>().type = LightType.Spot;
				playerLight.GetComponent<Light>().spotAngle = 15;
				break;
			case 2:
				playerLight.GetComponent<Light>().shadows = LightShadows.Hard;
				playerLight.GetComponent<Light>().type = LightType.Spot;
				playerLight.GetComponent<Light>().spotAngle = 30;
				break;
			case 3:
				playerLight.GetComponent<Light>().shadows = LightShadows.Hard;
				playerLight.GetComponent<Light>().type = LightType.Spot;
				playerLight.GetComponent<Light>().spotAngle = 45;
				break;
			case 4:
				playerLight.GetComponent<Light>().shadows = LightShadows.Hard;
				playerLight.GetComponent<Light>().type = LightType.Spot;
				playerLight.GetComponent<Light>().spotAngle = 60;
				break;
			case 5:
				playerLight.GetComponent<Light>().shadows = LightShadows.Soft;
				playerLight.GetComponent<Light>().type = LightType.Spot;
				playerLight.GetComponent<Light>().spotAngle = 15;
				break;
			case 6:
				playerLight.GetComponent<Light>().shadows = LightShadows.Soft;
				playerLight.GetComponent<Light>().type = LightType.Spot;
				playerLight.GetComponent<Light>().spotAngle = 30;
				break;
			case 7:
				playerLight.GetComponent<Light>().shadows = LightShadows.Soft;
				playerLight.GetComponent<Light>().type = LightType.Spot;
				playerLight.GetComponent<Light>().spotAngle = 45;
				break;
			case 8:
				playerLight.GetComponent<Light>().shadows = LightShadows.Soft;
				playerLight.GetComponent<Light>().type = LightType.Spot;
				playerLight.GetComponent<Light>().spotAngle = 60;
				break;
		}


		switch (GlobalData.playerLightPosition) {
			case 0:
				playerLight.transform.position = playerCamera.transform.position;
				break;
			case 1:
				playerLight.transform.position = transform.position;
				break;
			case 2:
				playerLight.transform.position = new Vector3(
										playerCamera.transform.position.x,
										playerCamera.transform.position.y + 0.1f,
										playerCamera.transform.position.z);
				break;
			case 3:
				playerLight.transform.position = new Vector3(
										playerCamera.transform.position.x + 0.1f,
										playerCamera.transform.position.y - 0.1f,
										playerCamera.transform.position.z);
				break;
			case 4:
				playerLight.transform.position = new Vector3(
										playerCamera.transform.position.x - 0.1f,
										playerCamera.transform.position.y - 0.1f,
										playerCamera.transform.position.z);
				break;
			case 5:
				playerLight.transform.position = new Vector3(
										playerCamera.transform.position.x - 0.15f,
										playerCamera.transform.position.y - 0.2f,
										playerCamera.transform.position.z);
				break;
			case 6:
				playerLight.transform.position = new Vector3(
										playerCamera.transform.position.x + 0.15f,
										playerCamera.transform.position.y - 0.2f,
										playerCamera.transform.position.z);
				break;
		}

		playerCamera.GetComponent<Camera>().fieldOfView = GlobalData.playerFOV;
	}
	
	// Update is called once per frame
	void Update () {



		if (GlobalData.captureMouse) {
			playerCamera.GetComponent<mouselook>().lockCursor = !GlobalData.inputController.getButton("Show Cursor");
		} else {
			playerCamera.GetComponent<mouselook>().lockCursor = GlobalData.inputController.getButton("Show Cursor");
		}

		if (GlobalData.inputController.getButton("Menu")) {
			GlobalData.map.menu.SetActive(true);
			playerCamera.GetComponent<mouselook>().lockCursor = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			gameObject.SetActive(false);
		}

		if ((GlobalData.alwaysRun && !GlobalData.inputController.getButton("Run/Walk")) || (!GlobalData.alwaysRun && GlobalData.inputController.getButton("RunWalk"))) {
			phys = running;
		} else {
			phys = walking;
		}
		// airborne = true;



		if (GlobalData.inputController.getButton("Action")) {
			RaycastHit hit;
			Vector3 cameraPos;
			cameraPos = playerCamera.transform.position;
			MapSegment ms = null;
			if (Physics.Raycast(cameraPos, playerCamera.transform.forward, out hit, 7)) {
				if (hit.collider.transform.parent.tag == "polygon") {
					ms = hit.collider.transform.parent.GetComponent<MapSegment>();
				} else if (hit.collider.transform.parent.name == "upperPlatform" || hit.collider.transform.parent.name == "lowerPlatform") {
					ms =  hit.collider.transform.parent.parent.parent.GetComponent<MapSegment>();
				}
				if (ms != null) {
					if (hit.collider.gameObject != lastTouch) {
						touched = ms.playerTouch(hit.collider.gameObject);
					}
				}
				if (touched) {
					lastTouch = hit.collider.gameObject;
				}
			}
		} else {
			lastTouch = null;
		}

		int prevPolygon = currentPolygon;
		getCurrentPolygon();
		if (currentPolygon >= 0) {
			if (prevPolygon != currentPolygon) {
				activePolygons = new List<int>(); 
				drawActivePolygons();
				setActivePolygonColliders();
				GlobalData.map.segments[currentPolygon].triggerBehaviour();
			}
			playAmbientSound();
			playRandomSound();

			vis = true;
			calculateVisibility();
			vis = false;
		}
		//if (Input.GetKey("f")){castRay();}

	}

	void setActivePolygonColliders(int connections = 1, MapSegment polygon = null) {
		if (connections < 0) {return;}
		if (polygon == null) {
			polygon = GlobalData.map.segments[currentPolygon];
			wallContacts = new List<Collider>();
		}
		foreach (MapSegmentSide side in polygon.sides) {
			if (side.connection != null) {
				setActivePolygonColliders(connections-1, side.connection);
			}
			if (side.lowerMeshItem != null) {
				if (!wallContacts.Contains(side.lowerMeshItem.GetComponent<Collider>())) {
					if (side.connection.height.y < phys.height * 0.98f) {
						wallContacts.Add(side.lowerMeshItem.GetComponent<Collider>());
					}
				}
			}
			if (side.middleMeshItem != null) {
				if (!wallContacts.Contains(side.middleMeshItem.GetComponent<Collider>())) {
					wallContacts.Add(side.middleMeshItem.GetComponent<Collider>());
				}
			}
			if (side.upperMeshItem != null) {
				if (!wallContacts.Contains(side.upperMeshItem.GetComponent<Collider>())) {
					wallContacts.Add(side.upperMeshItem.GetComponent<Collider>());
				}
			}
		}
	}


	void addInternalForces() {
		swimming = GlobalData.inputController.getButton("Swim");
		if (!airborne || swimming) {
			if (GlobalData.inputController.getButton("Forwards") && velocity.z < phys.maxForwardSpeed * Time.fixedDeltaTime) {
				velocity.z += phys.acceleration/GlobalData.accellerationScaleFactor * Time.fixedDeltaTime;
				velocity.z += phys.deceleration/GlobalData.decellerationScaleFactor * Time.fixedDeltaTime;
				if (velocity.z > phys.maxForwardSpeed * Time.fixedDeltaTime) {velocity.z = phys.maxForwardSpeed * Time.fixedDeltaTime;}
			}
			if (GlobalData.inputController.getButton("Backwards") && velocity.z > 0-phys.maxBackwardSpeed * Time.fixedDeltaTime) {
				velocity.z -= phys.acceleration/GlobalData.accellerationScaleFactor * Time.fixedDeltaTime;
				velocity.z -= phys.deceleration/GlobalData.decellerationScaleFactor * Time.fixedDeltaTime;
				if (velocity.z < 0-phys.maxBackwardSpeed * Time.fixedDeltaTime) {velocity.z = 0-phys.maxBackwardSpeed * Time.fixedDeltaTime;}
			}
			if (GlobalData.inputController.getButton("Left") && velocity.x > 0-phys.maxPerpSpeed * Time.fixedDeltaTime) {
				velocity.x -= phys.acceleration/GlobalData.accellerationScaleFactor * Time.fixedDeltaTime;
				velocity.x -= phys.deceleration/GlobalData.decellerationScaleFactor * Time.fixedDeltaTime;
				if (velocity.x < 0-phys.maxPerpSpeed * Time.fixedDeltaTime) {velocity.x = 0-phys.maxPerpSpeed * Time.fixedDeltaTime;}
			}
			if (GlobalData.inputController.getButton("Right") && velocity.x < phys.maxPerpSpeed * Time.fixedDeltaTime) {
				velocity.x += phys.acceleration/GlobalData.accellerationScaleFactor * Time.fixedDeltaTime;
				velocity.x += phys.deceleration/GlobalData.decellerationScaleFactor * Time.fixedDeltaTime;
				if (velocity.x > phys.maxPerpSpeed * Time.fixedDeltaTime) {velocity.x = phys.maxPerpSpeed * Time.fixedDeltaTime;}
			}
		}

	}


	void addExternalForces() {
		
		if (!airborne || swimming) {
			if (velocity.z > 0) {
				velocity.z -= phys.deceleration/GlobalData.decellerationScaleFactor * Time.fixedDeltaTime;
				if (velocity.z < 0) {velocity.z = 0;}
			}
			if (velocity.z < 0) {
				velocity.z += phys.deceleration/GlobalData.decellerationScaleFactor * Time.fixedDeltaTime;
				if (velocity.z > 0) {velocity.z = 0;}
			}

			if (velocity.x < 0) {
				velocity.x += phys.deceleration/GlobalData.decellerationScaleFactor * Time.fixedDeltaTime;
				if (velocity.x > 0) {velocity.x = 0;}
			}
			if (velocity.x > 0) {
				velocity.x -= phys.deceleration/GlobalData.decellerationScaleFactor * Time.fixedDeltaTime;
				if (velocity.x < 0) {velocity.x = 0;}
			}

			if (climbing || swimming) {
				Debug.Log("climbing");
				if (velocity.y < phys.terminalVelocity * Time.fixedDeltaTime) {
					velocity.y += phys.climbingAcceleration/GlobalData.antiGraviyScaleFactor * Time.fixedDeltaTime ;
					if (velocity.y > phys.terminalVelocity * Time.fixedDeltaTime) {velocity.y = phys.terminalVelocity * Time.fixedDeltaTime;}
				}
			}


		} else {
			// if (velocity.z > 0) {
			// 	velocity.z -= phys.airborneDeceleration * Time.fixedDeltaTime;
			// 	if (velocity.z < 0) {velocity.z = 0;}
			// }
			// if (velocity.z < 0) {
			// 	velocity.z += phys.airborneDeceleration * Time.fixedDeltaTime;
			// 	if (velocity.z > 0) {velocity.z = 0;}
			// }

			// if (velocity.x < 0) {
			// 	velocity.x += phys.airborneDeceleration * Time.fixedDeltaTime;
			// 	if (velocity.x > 0) {velocity.x = 0;}
			// }
			// if (velocity.x > 0) {
			// 	velocity.x -= phys.airborneDeceleration * Time.fixedDeltaTime;
			// 	if (velocity.x < 0) {velocity.x = 0;}
			// }


			if (velocity.y > 0-phys.terminalVelocity * Time.fixedDeltaTime) {
				velocity.y -= phys.gravity/GlobalData.graviyScaleFactor * Time.fixedDeltaTime;
				if (velocity.y < 0-phys.terminalVelocity * Time.fixedDeltaTime) {velocity.y = 0-phys.terminalVelocity * Time.deltaTime;}
			}


		}

		// foreach (Vector3 


	}

	void applyForces() {
		//	Debug.Log(velocity * 10);
		gameObject.transform.Translate(velocity);
		bool adjusted = true;
		while (adjusted) {
		adjusted = false;
		foreach (Collider wall in wallContacts) {	
			
			bool gapTooSmall = false;
			if (wall.transform.parent.tag == "polygon") {
				MapSegment seg = wall.transform.parent.GetComponent<MapSegment>();
				foreach (MapSegmentSide side in seg.sides) {
					if (side.lowerMeshItem == wall.gameObject 
					|| side.middleMeshItem == wall.gameObject
					|| side.upperMeshItem == wall.gameObject) {
						if (side.connection != null && side.connection.height.y < phys.height - 0.02) {
							gapTooSmall = true;
							break;
						}
					}
				}
			}

			if (gapTooSmall || (wall.bounds.center.y + wall.bounds.extents.y > phys.maxStepSize + transform.position.y 
			&& wall.bounds.center.y - wall.bounds.extents.y  < transform.position.y + phys.height-0.02))	{	
				
				Vector3 local = wall.transform.InverseTransformPoint(new Vector3(transform.position.x, wall.bounds.center.y, transform.position.z));

				Mesh mesh = wall.gameObject.GetComponent<MeshCollider>().sharedMesh;


				Vector3 collPt = wall.transform.TransformPoint(ClosestPointOnMesh(mesh, local));
				Vector3 meshNormal = mesh.normals[0]; //walls are flat, to the normals should be the same for all 
				//meshNormal = transform.InverseTransformDirection(meshNormal);
				if ((meshNormal.z > 0 != (transform.position.z - collPt.z) > 0) || (meshNormal.x > 0 != (transform.position.x - collPt.x) > 0)) {
					continue;
				}
				collPt.y = transform.position.y;
				Vector3 delta = transform.position - collPt;
				// Debug.Log(transform.position);
				// if (phys.radius - delta.magnitude > 0 || wall.name == "upperWall") {
				// 	Debug.Log("----------" +wall.name );
				// 	Debug.Log(collPt);
				// 	Debug.Log(collPt2);
				// 	Debug.Log(delta * 10);
				// 	Debug.Log(delta2 * 10);
				// }
				delta = Vector3.ClampMagnitude(delta, Mathf.Clamp(phys.radius - delta.magnitude, 0, phys.radius));
				delta.y = 0; 
				if (delta.magnitude > 0.001f ) {
					adjusted = true;
					transform.position += delta;
				}

			// Debug.Log(transform.position);
			// Debug.Log(velocity);
			}
		}
		}

	}


	void LateUpdate() {

	}

    Vector3 ClosestPointOnMesh(Mesh mesh, Vector3 point) {
        float shortestDistance = float.MaxValue;
        Vector3 closest = Vector3.zero;
        for (int i = 0; i < mesh.triangles.Length; i += 3) {
            Vector3 p1 = mesh.vertices[mesh.triangles[i]];
            Vector3 p2 = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 p3 = mesh.vertices[mesh.triangles[i + 2]];

            Vector3 nearest;
                
            ClosestPointOnTriangleToPoint(ref p1, ref p2, ref p3, ref point, out nearest);

            float distance = (point - nearest).sqrMagnitude;

            if (distance <= shortestDistance) {
                shortestDistance = distance;
                closest = nearest;
            }
        }
        return closest;
    }

    /// <summary>
    /// Determines the closest point between a point and a triangle.
    /// 
    /// The code in this method is copyrighted by the SlimDX Group under the MIT license:
    /// 
    /// Copyright (c) 2007-2010 SlimDX Group
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// 
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <param name="vertex1">The first vertex to test.</param>
    /// <param name="vertex2">The second vertex to test.</param>
    /// <param name="vertex3">The third vertex to test.</param>
    /// <param name="result">When the method completes, contains the closest point between the two objects.</param>
    void ClosestPointOnTriangleToPoint(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, ref Vector3 point, out Vector3 result)
    {
        //Source: Real-Time Collision Detection by Christer Ericson
        //Reference: Page 136

        //Check if P in vertex region outside A
        Vector3 ab = vertex2 - vertex1;
        Vector3 ac = vertex3 - vertex1;
        Vector3 ap = point - vertex1;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0.0f && d2 <= 0.0f)
        {
            result = vertex1; //Barycentric coordinates (1,0,0)
            return;
        }

        //Check if P in vertex region outside B
        Vector3 bp = point - vertex2;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0.0f && d4 <= d3)
        {
            result = vertex2; // barycentric coordinates (0,1,0)
            return;
        }

        //Check if P in edge region of AB, if so return projection of P onto AB
        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
        {
            float v = d1 / (d1 - d3);
            result = vertex1 + v * ab; //Barycentric coordinates (1-v,v,0)
            return;
        }

        //Check if P in vertex region outside C
        Vector3 cp = point - vertex3;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0.0f && d5 <= d6)
        {
            result = vertex3; //Barycentric coordinates (0,0,1)
            return;
        }

        //Check if P in edge region of AC, if so return projection of P onto AC
        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
        {
            float w = d2 / (d2 - d6);
            result = vertex1 + w * ac; //Barycentric coordinates (1-w,0,w)
            return;
        }

        //Check if P in edge region of BC, if so return projection of P onto BC
        float va = d3 * d6 - d5 * d4;
        if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
        {
            float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            result = vertex2 + w * (vertex3 - vertex2); //Barycentric coordinates (0,1-w,w)
            return;
        }

        //P inside face region. Compute Q through its barycentric coordinates (u,v,w)
        float denom = 1.0f / (va + vb + vc);
        float v2 = vb * denom;
        float w2 = vc * denom;
        result = vertex1 + ab * v2 + ac * w2; //= u*vertex1 + v*vertex2 + w*vertex3, u = va * denom = 1.0f - v - w
    }



	void OnTriggerEnter(Collider other)
	{
		if (!vis) {
			if (other.name == "floor" && other.transform.position.y < transform.position.y + phys.maxStepSize) {
				airborne = false;
				floorContacts.Add(other);
				// if (velocity.y < 0) {
				// 	velocity.y = 0;
				// }
			}
			if (other.transform.parent.name == "lowerPlatform") {
				if (other.name == "platTop" && other.transform.position.y < transform.position.y + phys.maxStepSize) {
					airborne = false;
					if (platContactL == null || other.transform.position.y < platContactL.transform.position.y) {
					platContactL = other;
					}
					floorContacts.Add(other);
					// if (velocity.y < 0) {
					// 	velocity.y = 0;
					// }
				}
			}
			if (other.transform.parent.name == "upperPlatform") {
				if (other.name == "platBottom" && other.transform.position.y < transform.position.y + phys.maxStepSize) {
					airborne = false;
					if (platContactU == null || other.transform.position.y > platContactU.transform.position.y) {
					platContactU = other;
					}
					if (velocity.y < 0) {
						velocity.y = 0;
					}
				}
			}
			if (other.name == "ceiling" ) {
				if (velocity.y > 0) {
					velocity.y = 0;
				}
			}

			if (other.name == "wall" 
			|| other.name == "lowerWall" 
			|| other.name == "middleWall" 
			|| other.name == "upperWall"
			|| other.name == "platSide"
			) {
				if (other.bounds.center.y + other.bounds.extents.y > phys.maxStepSize + transform.position.y) {
					if (!wallContacts.Contains(other)) {wallContacts.Add(other);return;}
				}
				if (other.transform.parent.tag == "polygon" && !wallContacts.Contains(other)) {
					MapSegment seg = other.transform.parent.GetComponent<MapSegment>();
					foreach (MapSegmentSide side in seg.sides) {
						if (side.lowerMeshItem == other.gameObject 
						|| side.middleMeshItem == other.gameObject
						|| side.upperMeshItem == other.gameObject) {
							if (side.connection == null || side.connection.height.y < phys.height) {
								wallContacts.Add(other);return;
							}
						}
					}
				}
			}
		}
	}
	void OnTriggerStay(Collider other) {
		if (!vis) {
			//if (other.gameObject != gameObject && other.transform.parent.tag == "polygon") {
			if (other.name == "floor" && other.transform.position.y < transform.position.y + phys.maxStepSize) {
				// Debug.Log(other.transform.parent.GetComponent<MapSegment>().id + "::" + floorContacts.Count);

				airborne = false;
				if (other.transform.parent.gameObject.GetComponent<MapSegment>().platform == null) {
					floorContacts.Add(other);
				}
				if (velocity.y < 0) {
					velocity.y = 0;
				}
				if (other.transform.position.y > transform.position.y) {
					climbing = true;
				}
			}
			if (other.transform.parent.name == "lowerPlatform") {
				if (other.name == "platTop" && other.transform.position.y < transform.position.y + phys.maxStepSize) {
					airborne = false;
					if (platContactL == null || other.transform.position.y < platContactL.transform.position.y) {
					platContactL = other;
					}
					floorContacts.Add(other);
					if (other.transform.position.y > transform.position.y) {
						climbing = true;
					}
					if (velocity.y < 0) {
						velocity.y = 0;
					}
				}
			}
			if (other.transform.parent.name == "upperPlatform") {
				if (other.name == "platBottom" && other.transform.position.y < transform.position.y + phys.maxStepSize) {
					airborne = false;
					if (platContactU == null || other.transform.position.y  > platContactU.transform.position.y) {
					platContactU = other;
					}
					if (velocity.y < 0) {
						velocity.y = 0;
					}
				}
			}			if (other.name == "ceiling" ) {
				if (velocity.y > 0) {
					velocity.y = 0;
				}
			}


			if (other.name == "wall" 
			|| other.name == "lowerWall" 
			|| other.name == "middleWall" 
			|| other.name == "upperWall"
			|| other.name == "platSide"
			) {
				if (other.bounds.center.y + other.bounds.extents.y > phys.maxStepSize + transform.position.y) {
					if (!wallContacts.Contains(other)) {wallContacts.Add(other);}
				}
				if (other.transform.parent.tag == "polygon" && !wallContacts.Contains(other)) {
					MapSegment seg = other.transform.parent.GetComponent<MapSegment>();
					foreach (MapSegmentSide side in seg.sides) {
						if (side.lowerMeshItem == other.gameObject 
						|| side.middleMeshItem == other.gameObject
						|| side.upperMeshItem == other.gameObject) {
							if (side.connection == null || side.connection.height.y < phys.height) {
								wallContacts.Add(other);return;
							}
						}
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!vis) {
		//if (other.transform.parent.tag == "polygon") {
			if (other.name == "floor" || other.name == "platTop") {
				// Debug.Log(other.transform.parent.GetComponent<MapSegment>().id + "exit");
				floorContacts.Remove(other);
				if (floorContacts.Count == 0) {
					bool higherContact = false;
					foreach(Collider col in floorContacts) {
						if (col.transform.position.y > transform.position.y) {
							higherContact = true;
						}
					}
					if (!higherContact) {
						if (velocity.y > 0) {velocity.y /= GlobalData.deBounceFactor;}
						if (velocity.y < phys.airborneDeceleration/GlobalData.deBounceFactor * Time.deltaTime  ) {
							//Debug.Log("setLevel");
							velocity.y = 0;
							if (other.transform.position.y < transform.position.y + 0.1) {
								transform.position = new Vector3 (transform.position.x, other.transform.position.y,transform.position.z);
							}
							airborne = false;

							Debug.Log(other.transform.parent.name);
						} else {
							// airborne = true;
						// 	touching = false;
						}
						climbing = false;
					}
				}
			}
		//}
		}
	}




	void getCurrentPolygon() {
		int pol = -1;
		RaycastHit hit;
		//GameObject camera = PlayerCamera.gameObject;
		if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 50)) {
			if (hit.collider.transform.parent != null && hit.collider.transform.parent.tag == "polygon") {
				pol = hit.collider.transform.parent.GetComponent<MapSegment>().id;
			} else if (hit.collider.transform.parent != null && (hit.collider.transform.parent.name == "upperPlatform" || hit.collider.transform.parent.name == "lowerPlatform")) {
				pol = hit.collider.transform.parent.parent.GetComponent<PlatformObject>().parent.id;
			}
		}
		if (pol == -1) {

			if (Physics.Raycast(new Vector3(gameObject.transform.position.x, 
											gameObject.transform.position.y + phys.height * 0.98f, 
											gameObject.transform.position.z),
								Vector3.up, out hit, 50)) {
				if (hit.collider.transform.parent != null && hit.collider.transform.parent.tag == "polygon") {
					pol = hit.collider.transform.parent.GetComponent<MapSegment>().id;
				} else if (hit.collider.transform.parent != null && (hit.collider.transform.parent.name == "upperPlatform" || hit.collider.transform.parent.name == "lowerPlatform")) {
					pol = hit.collider.transform.parent.parent.GetComponent<PlatformObject>().parent.id;
				}
			}
		}


		if (pol >= 0) {currentPolygon = pol;}

	}

	void playAmbientSound() {
		ambient = GlobalData.map.segments[currentPolygon].ambientSound;
		ambientLoopTimer += Time.deltaTime;
		AudioSource aud = GetComponent<AudioSource>();
		if (aud.clip == null || ambientLoopTimer >= aud.clip.length) {
			if (ambient == null) {
				aud.Stop();
			} else {
				aud.loop = true;
				aud.volume = ambient.volume;
				aud.clip = ambient.audio.sounds[Random.Range(0,ambient.audio.sounds.Count)];
				aud.Play();
				ambientLoopTimer = 0;
			}
		}
	}

	void playRandomSound() {
		if (GlobalData.map.segments[currentPolygon].randomSound != randomSound) {
			randomSound = GlobalData.map.segments[currentPolygon].randomSound;
			randomSoundTimer = 0;
			if (randomSound != null) {
				randomSoundNext = randomSound.period + Random.Range(0-randomSound.periodDelta, randomSound.periodDelta);
			}

		}

		List<GameObject> rsds = new List<GameObject>();
		foreach (GameObject rs in randomSounds) {
			if (!rs.GetComponent<AudioSource>().isPlaying) {
				Destroy(rs);
			} else {
				rsds.Add(rs);
			}
		}
		randomSounds = new List<GameObject>(rsds);


		if (randomSound != null) {
			randomSoundTimer+= Time.deltaTime;
			if (randomSoundTimer >= randomSoundNext) {
				randomSoundTimer = 0; 
				randomSoundNext = randomSound.period + Random.Range(0-randomSound.periodDelta, randomSound.periodDelta);
				Vector3 pos = transform.position;
				float dir = 0;
				if (!randomSound.nonDirectional) {
					dir = randomSound.direction + Random.Range(0-randomSound.directionDelta, randomSound.directionDelta) + 90;
				}						
				Quaternion facing = Quaternion.Euler(0, dir, 0);

				GameObject randomPlayer =  Instantiate(Resources.Load<GameObject>("randomSound"), pos, facing);
				if (!randomSound.nonDirectional) {
					randomPlayer.transform.Translate(Vector3.forward * 0.5f);
				}

				AudioSource aud = randomPlayer.GetComponent<AudioSource>();
				aud.clip = randomSound.audio.sounds[Random.Range(0,randomSound.audio.sounds.Count)];
				aud.volume = randomSound.volume + Random.Range(0-randomSound.volumeDelta, randomSound.volumeDelta);
				aud.pitch = randomSound.pitch + Random.Range(0-randomSound.pitchDelta, randomSound.pitchDelta);
				aud.Play();
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
				if (active[i]) {activePolygons.Add(i);};
			}
		}
	}
	void calculateVisibility() {
		//get list of active volumes that are visible
		List<ImpossibleVolume> ivs = new List<ImpossibleVolume>();
		List<int> ap = new List<int>(activePolygons);
		while (ap.Count > 0) {
			foreach(ImpossibleVolume iv in GlobalData.map.segments[ap[0]].ImpossibleVolumes) {
				ivs.Add(iv);
				foreach(int pol in iv.collisionPolygonsSelf) {
					ap.Remove(pol);
					GlobalData.map.segments[pol].showHide(false);
					GlobalData.map.segments[pol].setClippingPlanes(new List<Vector3>());//remove clipping 
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
		// List<float> added = new List<float>();
		while (distances.Count > 0) {
			float d = 7777777f;
			float[] lastf = distances[0];
			foreach(float[] f in distances) {
				if (f[0] < d) {d = f[0]; lastf = f;}
			}
			// if (!added.Contains(lastf[1])) {
			temp.Add(lastf);
			// }
			distances.Remove(lastf);
			// added.Add(lastf[1]);
		}
		distances = temp;
		// if we are in impossible space, then draw the volume we are in before doing anything else
		List<int> clippedSegments = new List<int>();
		List<int> shownSegments = new List<int>();
		List<int> shownD = new List<int>();

		if (selfIV >=0) {
			showVolume(ivs[selfIV], ref clippedSegments);
		}
		//draw impossible volumes in order of their closest connection to normal pols
		for (int d = 0; d < distances.Count; d++) {
			Vector3 point1, point2;
			MapSegment seg = GlobalData.map.segments[(int)distances[d][2]];
			MapSegment conn = seg.sides[(int)distances[d][3]].connection;
			float height = seg.height.y;

			point1 = seg.vertices[(int)distances[d][3]];
			if ((int)distances[d][3]+1 < seg.vertices.Count) {
				point2 = seg.vertices[(int)distances[d][3]+1];
			} else {
				point2 = seg.vertices[0];
			}
			point1 = seg.transform.TransformPoint(point1);
	 		point2 = seg.transform.TransformPoint(point2);

			if (conn.centerPoint.y > seg.centerPoint.y) {
				point1.y = conn.centerPoint.y;
				point2.y = conn.centerPoint.y;
				height -= (conn.centerPoint.y - seg.centerPoint.y);
			}
			if (conn.centerPoint.y + conn.height.y < seg.centerPoint.y + seg.height.y) {
				height -= ((seg.centerPoint.y + seg.height.y) - (conn.centerPoint.y + conn.height.y));
			}

			if (!shownSegments.Contains((int)distances[d][1]) && getRectVisibility(point1, point2, new Vector3(0,height,0))) {
				shownSegments.Add((int)distances[d][1]);
				showVolume(ivs[(int)distances[d][1]], ref clippedSegments);
				foreach(int i in ivs[(int)distances[d][1]].collisionPolygonsOther) {
					if (!GlobalData.map.segments[i].hidden) {
						int other = -1;
						foreach (int o in shownD) {
							if (ivs[(int)distances[o][1]].collisionPolygonsSelf.Contains(i) 
									  && ivs[(int)distances[o][1]].collisionPolygonsOther.Contains(ivs[(int)distances[d][1]].collisionPolygonsSelf[0])) {
								other = o; 
								break;
							}
						}
						// if (!clippedSegments.Contains(distances[d][2]) && !clippedSegments.Contains(distances[other][1])) {
						if (other > -1) {
							calculateClipping(ivs, distances, d, other, ref clippedSegments);
						break;
						}
					}
				}
				shownD.Add(d);
			}
		}
		foreach (int i in clippedSegments) {
			GlobalData.map.segments[i].showHide(true);
		}
	}

	void showVolume(ImpossibleVolume iv, ref List<int> clippedSegments, bool show = true) {
		foreach (int pol in iv.collisionPolygonsSelf) {
			if ((GlobalData.map.segments[currentPolygon].collidesWith != null &&
			GlobalData.map.segments[currentPolygon].collidesWith.Contains(pol) &&
			iv.collisionPolygonsSelf.Contains(currentPolygon)) || clippedSegments.Contains(pol)) {
				GlobalData.map.segments[pol].showHide(false);
			} else {
				GlobalData.map.segments[pol].showHide(show);
			}
		}
	}

	void calculateClipping (List<ImpossibleVolume> ivs, List<float[]> distances, int volSelf, int volOther, ref List<int> clippedSegments) {
		ImpossibleVolume iv = ivs[(int)distances[volSelf][1]];
		GameObject camera;
		camera = playerCamera.gameObject;
		Vector3 pp = camera.transform.position;

		//get clockwise entry point

		List<float> clipAngles = new List<float>();

		float distance = 7777777f;

		List<float> sideAngles = new List<float>();
		List<Vector3> points = new List<Vector3>();
		Vector3 pointSelf = new Vector3(0,0,0);
		Vector3	pointOther = new Vector3(0,0,0);
		Vector3 pointSelfFar = new Vector3(0,0,0);
		Vector3	pointOtherFar = new Vector3(0,0,0);


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
		for (int point = 0; point < 2; point++) {
			for (int p = 2; p < 4; p++) {
				float d = Vector3.Distance(points[point],points[p]);
				if (d > 0 && d < distance) {
					pointSelf = points[point];
					pointSelfFar = points[Mathf.Abs(1-point)];
					pointOther = points[p];
					pointOtherFar = points[Mathf.Abs(1-(p-2))+2];
					distance = d;
				}
			}
		}



		distance = 7777777;
		int closest = 0;
		for (int i = 0; i < iv.collisionPoints.Count; i++) {
			//distances.Add(Vector2.Distance(new Vector2(vec.x, vec.z), new Vector2(pp.x, pp.z)));
			float d = Vector2.Distance(new Vector2(iv.collisionPoints[i].x, iv.collisionPoints[i].z), new Vector2(pointSelf.x, pointSelf.z));
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
		// clipAngles.Add(Mathf.Atan2(pp.x-iv.collisionPoints[clockwise].x, pp.z-iv.collisionPoints[clockwise].z) * Mathf.Rad2Deg);
		// clipAngles.Add(Mathf.Atan2(pp.x-iv.collisionPoints[counterClockwise].x, pp.z-iv.collisionPoints[counterClockwise].z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(pointSelf.x-pointSelfFar.x, pointSelf.z-pointSelfFar.z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(pointOther.x-pointOtherFar.x, pointOther.z-pointOtherFar.z) * Mathf.Rad2Deg);
		// clipAngles.Add(Mathf.Atan2(iv.collisionPoints[closest].x-iv.collisionPoints[clockwise].x, iv.collisionPoints[closest].z-iv.collisionPoints[clockwise].z) * Mathf.Rad2Deg);
		// clipAngles.Add(Mathf.Atan2(iv.collisionPoints[closest].x-iv.collisionPoints[counterClockwise].x, iv.collisionPoints[closest].z-iv.collisionPoints[counterClockwise].z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(iv.collisionPoints[closest].x-pointSelfFar.x, iv.collisionPoints[closest].z-pointSelfFar.z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(iv.collisionPoints[closest].x-pointOtherFar.x, iv.collisionPoints[closest].z-pointOtherFar.z) * Mathf.Rad2Deg);
		// clipAngles.Add(Mathf.Atan2(iv.collisionPoints[closest].x-pointOther.x, iv.collisionPoints[closest].z-pointOther.z) * Mathf.Rad2Deg);
		// clipAngles.Add(Mathf.Atan2(iv.collisionPoints[closest].x-pointSelf.x, iv.collisionPoints[closest].z-pointSelf.z) * Mathf.Rad2Deg);
		//clipAngles.Add(Mathf.Atan2(iv.collisionPoints[clockwise].x-iv.collisionPoints[counterClockwise].x, iv.collisionPoints[clockwise].z-iv.collisionPoints[counterClockwise].z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(pp.x-pointSelfFar.x, pp.z-pointSelfFar.z) * Mathf.Rad2Deg);
		clipAngles.Add(Mathf.Atan2(pp.x-pointOtherFar.x, pp.z-pointOtherFar.z) * Mathf.Rad2Deg);

		

		sideAngles.Add(Mathf.Atan2(pp.x-pointSelf.x, pp.z-pointSelf.z) * Mathf.Rad2Deg);
		sideAngles.Add(Mathf.Atan2(pp.x-pointOther.x, pp.z-pointOther.z) * Mathf.Rad2Deg);
		//sideAngles.Add(Mathf.Atan2(pointSelf.x-pointOther.x, pointSelf.z-pointOther.z) * Mathf.Rad2Deg);
		sideAngles.Add(Mathf.Atan2(pointOther.x-pointSelf.x, pointOther.z-pointSelf.z) * Mathf.Rad2Deg);

		bool selfIsClockwise = (angleDifference(sideAngles[0], sideAngles[1]) < 0);
		bool inside = (angleDifference(clipAngles[0], sideAngles[0]) < 0) != (angleDifference(clipAngles[0], sideAngles[1]) < 0);
		if (!inside) {
			if (angleDifference(sideAngles[0], sideAngles[2]) < 0 != angleDifference(clipAngles[0], sideAngles[0]) < 0){
				// Debug.Log("bad?");
				selfIsClockwise = !selfIsClockwise;
			}
		}

		float a = 180f * (Input.GetKey("7") ? 1 :0);//s
		float b = 180f * (Input.GetKey("8") ? 1 :0);
		float c = 180f * (Input.GetKey("9") ? 1 :0);
		float dd = 180f * (Input.GetKey("u") ? 1 :0);
		float e = 180f * (Input.GetKey("i") ? 1 :0);
		float f = 180f * (Input.GetKey("o") ? 1 :0);
		float g = 180f * (Input.GetKey("j") ? 1 :0);
		float h = 180f * (Input.GetKey("k") ? 1 :0);//o
		float ii = 180f * (Input.GetKey("l") ? 1 :0);//o
		float j = 180f * (Input.GetKey("n") ? 1 :0);//o
		float k = 180f * (Input.GetKey("m") ? 1 :0);
		float l = 180f * (Input.GetKey(",") ? 1 :0);

		if (selfIsClockwise) {
			clipPlanes(pp,pp,iv.collisionPoints[closest],clipAngles[0]+180+a, clipAngles[5]+b, clipAngles[1]+c, true, iv.collisionPolygonsSelf, ref clippedSegments);
			clipPlanes(pp,pp,iv.collisionPoints[closest],clipAngles[0]+dd, clipAngles[6]+180+e, clipAngles[2]+180+f, true, iv.collisionPolygonsOther, ref clippedSegments);
			Debug.Log("self");
		} else {
			clipPlanes(pp,pp,iv.collisionPoints[closest],clipAngles[0]+g, clipAngles[5]+180+h, clipAngles[1]+180+ii, true, iv.collisionPolygonsSelf, ref clippedSegments);
			clipPlanes(pp,pp,iv.collisionPoints[closest],clipAngles[0]+180+j, clipAngles[6]+k, clipAngles[2]+l, true,iv.collisionPolygonsOther, ref clippedSegments);
			Debug.Log("other");
		}

		Debug.DrawRay(pointSelf,pp-pointSelf,  Color.cyan);
		Debug.DrawRay(pointOther,pp-pointOther,  Color.white);

		Debug.DrawRay(iv.collisionPoints[closest],pp-iv.collisionPoints[closest],  Color.blue);
		Debug.DrawRay(iv.collisionPoints[clockwise],pp-iv.collisionPoints[clockwise],  Color.yellow);
		Debug.DrawRay(iv.collisionPoints[counterClockwise],pp-iv.collisionPoints[counterClockwise],  Color.green);
		
		Debug.DrawRay(pointSelfFar,pp-pointSelfFar,  Color.magenta);
		Debug.DrawRay(pointOtherFar,pp-pointOtherFar,  Color.gray);
	}

	float angleDifference (float angle1, float angle2, bool halfPositive = true) {
		// 20, 30 = 10
		// 30, 20 = -10
		// -10, 10 = 20
		// 10, -10 = -20
		// -10, -20 = -10
		// -20, -10 = 10
		// 170, -170 = 20
		// -170, 170 = -20

		float ret = angle2 - angle1;
		if (ret > 180) {
			ret -= 360;
		}
		if (ret < -180) {
			ret += 360;
		}

		return ret;
	}




	void clipPlanes (Vector3 p1, Vector3 p2, Vector3 p3, float a1, float a2, float a3, bool additive, List<int> segments, ref List<int> clippedSegments) {
		List<Vector3> planes = new List<Vector3>();
		planes.Add(p1);
		planes.Add(new Vector3(90, 180f, 90f-a1));
		planes.Add(p2);
		planes.Add(new Vector3(90, 0, 90f-a2));
		planes.Add(new Vector3(p3.x, p1.y, p3.z));
		planes.Add(new Vector3(90, 0, 90f-a3));
		foreach(int pol in segments) {
			if (pol == currentPolygon) {
				GlobalData.map.segments[pol].setClippingPlanes(new List<Vector3>());//dont clip the poly we are in
			} else {
				if (!clippedSegments.Contains(pol)) {
					GlobalData.map.segments[pol].setClippingPlanes(planes, additive);
					clippedSegments.Add(pol);
					GlobalData.map.segments[pol].showHide(false);
				}
			}
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
		if (playerCamera.transform.position.y > point1.y && playerCamera.transform.position.y < point1.y+height.y ) {
			points.Add(p3 + point2 + new Vector3(0, playerCamera.transform.position.y - point1.y,0 ));
			points.Add(p3 + point2 + new Vector3(0, playerCamera.transform.position.y - point1.y + 0.7f, 0));
		}


		Vector3 castPoint;
		float rayCount = 3f;
		isVisible = false;

		isVisible = (Physics.Raycast(midpoint, playerCamera.transform.position-midpoint, out hit, 50)) 
			&& hit.collider.gameObject == playerCollider ;

		if (isVisible) {
			Debug.DrawRay(midpoint, playerCamera.transform.position-midpoint, Color.red);
		} else {
			Debug.DrawRay(midpoint, playerCamera.transform.position-midpoint, Color.green);
		}

		for (int i = 1; i <= rayCount && !isVisible; i++) {
			for (int p = 0; p < points.Count && !isVisible; p++){
				castPoint = (points[p]-midpoint)*((1f/rayCount)*(float)i) + midpoint;
				isVisible = (Physics.Raycast(castPoint, playerCamera.transform.position-castPoint, out hit, 50)) 
							&& hit.collider.gameObject == playerCollider ;
				if (isVisible) {
					Debug.DrawRay(castPoint, playerCamera.transform.position-castPoint, Color.red);
				} else {
					//Debug.DrawRay(castPoint, camera.transform.position-castPoint, Color.green);
				}
			}
		}
		
		return isVisible;
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		addInternalForces();
		addExternalForces();
		applyForces();
		floorContacts = new List<Collider>();
		platContactL = null;
		platContactU = null;
		
		// wallNormals = new List<Vector3>();
		// wallContacts = new List<Collider>();
	
		airborne = true;
		climbing = false;

	}

	public class playerPhysics {
		//these defaults are the standard physics running values
		public float maxStepSize = 0.3f;
		public float maxForwardSpeed = 0.125f * 30f;
		public float maxBackwardSpeed = 0.0833f * 30f;
		public float maxPerpSpeed = 0.0769f * 30f;

		public float acceleration = 0.0100f * 30f;
		public float deceleration = 0.0200f * 30f;
		public float airborneDeceleration = 0.0056f * 30f;
		public float gravity = 0.0025f * 30f;
		public float climbingAcceleration = 0.0050f * 30f;

		public float terminalVelocity = 0.1429f * 30f;
		public float externalDeceleration = 0.0050f * 30f;

		public float radius = 0.2500f;
		public float height = 0.8000f;
		public float stepDelta = 0.0500f;
		public float stepAmplitude = 0.1000f;
		public float cameraOffset = 0.2000f;
		public float cameraOffsetDead = 0.2500f;
		public float cameraoffsetSplash = 0.5000f;
		public float maximumCameraAngle = 42.6667f;
		public float halfCameraSeparation = 0.0312f;

		public float angularAcceleration = 1.2500f * 30f;
		public float angularDeceleration = 2.5000f * 30f;
		public float maxAngularVelocity = 10.0000f * 30f;
		public float angularRecenterVelocity = 1.5000f * 30f;
		public float headAngularVelocity = 21.3333f * 30f;
		public float headAngularMax = 128.0000f * 30f;
		public float externalAngularDeleceration = 0.3333f * 30f;
	}

}

