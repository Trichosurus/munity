using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteController : MonoBehaviour {
	public int type = 1;
	public int sideCount = 1;
	public int currentFrame = 0;
	public GameObject parent = null;
	public List<Material> frames = new List<Material>();
	public List<GameObject> sides = new List<GameObject>();
	public List<Vector2> offsets = new List<Vector2>();
	public float scale = 0.2f;
	public bool fromCeiling = false;

	

	private int lastframe = -1;
	// Use this for initialization
	void Start () {
		sides.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
		sides[0].transform.parent = gameObject.transform;
		sides[0].transform.position = gameObject.transform.position;
		sides[0].GetComponent<MeshCollider>().enabled = false;
		sides[0].transform.rotation = Quaternion.Euler(0,180,0);
		//sides[0].transform.localScale = new Vector3((frames[0].mainTexture.width/1024)*scale,(frames[0].mainTexture.height/1024)*scale,1);
	
		for (int i = 2; i < sideCount; i++) {
			sides.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
			sides[i-1].transform.parent = gameObject.transform;
			sides[i-1].transform.position = gameObject.transform.position;
			sides[i-1].transform.rotation = Quaternion.Euler(0,180,0);
			//sides[i-1].transform.localScale = new Vector3((frames[i-1].mainTexture.width/1024)*scale,(frames[i-1].mainTexture.height/1024)*scale,1);

			sides[i-1].GetComponent<MeshCollider>().enabled = false;
			if (type == 3) {
				sides[i-1].transform.rotation = Quaternion.Euler(0,(360f/(float)sideCount) * i-1,0);
			}
		}
		// if (type == 1) {gameObject.transform.fix}
	}
	
	// Update is called once per frame
	void Update () {
		if (parent != null && Camera.main != null)  {
			Vector3 point = Camera.main.transform.position;
			if (type < 3) {
				if (type == 2) {point.y = gameObject.transform.position.y;}
				transform.LookAt(point);
			}
			
			float rot = transform.rotation.y;
			if (rot < 0) {rot += 180f;}
			float aps = 360/sideCount;
			rot -= aps/2;
			int angle = (int)(rot/aps);

			for (int s = 0; s < sides.Count; s++) {
				if (s == angle) {
					int frame = currentFrame * sideCount + s;
					if (frame != lastframe) {
						sides[s].GetComponent<MeshRenderer>().material = frames[frame];
						float scalex = ((float)frames[frame].mainTexture.width/1024f)*scale;
						float scaley = ((float)frames[frame].mainTexture.height/1024f)*scale;
						float lastHeight = 0;
					
						sides[s].transform.localScale = new Vector3(scalex,scaley,1);
						if (parent.GetComponent<CapsuleCollider>() != null) {
							CapsuleCollider cc = parent.GetComponent<CapsuleCollider>();
							lastHeight = cc.height;
							if (scaley > scalex) {
								cc.radius = scalex/2;
								cc.height = scaley;
							} else {
								cc.height = scaley;
								cc.radius = scaley/2;
							}

						} else {
							BoxCollider bc = parent.GetComponent<BoxCollider>();
							lastHeight = bc.size.x;
							bc.size = new Vector3(scalex, scaley, scalex);
						}
						// Debug.Log(lastHeight);
						// Debug.Log(scalex);
						float vChange = (lastHeight - scaley)/2f;
						if (fromCeiling) {vChange = 0-vChange;}
						parent.transform.position = new Vector3(parent.transform.position.x, 
																parent.transform.position.y - vChange, 
																parent.transform.position.z);
						lastframe = frame;
					}
					sides[s].SetActive(true);


					if (type == 1 || type == 2) {
						if (GlobalData.map.player != null) {
							Quaternion cameraQ = GlobalData.map.player.transform.rotation;

							//camera = GameObject.Find("playerCamera");
							if (type == 1) {
								sides[s].transform.rotation = Quaternion.Euler(cameraQ.eulerAngles.x, cameraQ.eulerAngles.y , cameraQ.eulerAngles.z);
							} else {
								sides[s].transform.rotation = Quaternion.Euler(0, cameraQ.eulerAngles.y, 0);
							}
						}
					}



				} else {
					sides[s].SetActive(false);
				}
			}
			

		}
	}
}
