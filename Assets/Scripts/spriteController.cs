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

	

	private int lastframe = -1;
	// Use this for initialization
	void Start () {
		sides.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
		sides[0].transform.parent = gameObject.transform;
		sides[0].transform.position = gameObject.transform.position;
		sides[0].GetComponent<MeshCollider>().enabled = false;
		sides[0].transform.rotation = Quaternion.Euler(0,180,-90);
		sides[0].transform.localScale = new Vector3(scale,scale,scale);
	
		for (int i = 2; i < sideCount; i++) {
			sides.Add(GameObject.CreatePrimitive(PrimitiveType.Quad));
			sides[i-1].transform.parent = gameObject.transform;
			sides[i-1].transform.position = gameObject.transform.position;
			sides[i-1].transform.rotation = Quaternion.Euler(0,180,-90);

			sides[i-1].GetComponent<MeshCollider>().enabled = false;
			if (type == 2) {
				sides[i-1].transform.rotation = Quaternion.Euler(0,(360f/(float)sideCount) * i-1,-90);
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
						lastframe = frame;
					}
					sides[s].SetActive(true);
				} else {
					sides[s].SetActive(false);
				}
			}
			
			if (type == 1 || type == 2) {
				GameObject camera;
				camera = GameObject.Find("playerCamera").gameObject;
				gameObject.transform.rotation = Quaternion.Euler(camera.transform.rotation.eulerAngles.x + 180, camera.transform.rotation.eulerAngles.y , camera.transform.rotation.eulerAngles.z );
				//gameObject.transform.rotation = Quaternion.Inverse(camera.transform.rotation);
				// int i = 1;
			}

		}
	}
}
