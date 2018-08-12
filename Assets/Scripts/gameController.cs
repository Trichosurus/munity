using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class GlobalData
{
	public static string shapesFilePath = "choose me";
	public static string mapsFilePath = "choose me";
	public static string soundsFilePath = "choose me";
	public static string physicsFilePath = "choose me";
	public static string imagesFilePath = "choose me";

	public static string settingsFile = "settings.ini";

	public static Map map = null;

	public static InputController inputController = new InputController();

	public static bool skipOcclusion = true;
	public static int occlusionOverDraw = 1;
	public static float occlusionDensity = 0.5f;
	public static bool captureMouse = true;
	public static bool globalLighting = true;
	public static float globalLightingIntensity = 0.15f;
	public static bool playerLight = true;
	public static int playerLightType = 0;
	public static float playerLightIntensity = 0.7f;
	public static float playerLightRange = 3f;
	public static int playerLightPosition = 0;
	public static float playerLightDelay = 0;
	public static float playerFOV = 60f;
	public static int spriteType = 2;
	public static bool forceSpriteMultivews = false;
	public static int landscapeType = 5;
	public static bool alwaysRun = true;
	public static float graviyScaleFactor = 2f;
	public static float antiGraviyScaleFactor = 0.7f;
	public static float accellerationScaleFactor = 2f;
	public static float decellerationScaleFactor = 2f;
	public static float deBounceFactor = 4f;
	public static int mapToLoad = 0;
	public static float mouseSmooth = 0.05f;


	//----item definitions -- where are these defined in the data files ??????

	public static List<int> landscapeCollections = new List<int> {
		27,28,29,30
	};


	public static int itemCollection = 7;

	public static List<string> itemList = new List<string> {
		"pistol","pistol magazine",
		"fusion gun","fusion magazine",
		"assault rifle","assault magazine","assault grenades",
		"missile launcher","missile ammo",
		"invisibility","invincibility","infravision",
		"alien weapon","alien magazine",
		"flamethrower","flame cannister",
		"extravision","oxygen",
		"1x health","2x health","3x health",
		"shotgun","shotgun ammo",
		"door key","uplink chip",
		"unused",
		"the ball",
		"unused","unused","unused","unused","unused","unused",
		"smg","smg ammo"
	};
	public static List<int> itemSequences = new List<int> {
		0,3,
		1,4,
		2,5,6,
		12,7,
		8,9,14,
		13,13,
		10,11,
		15,23,
		20,21,22,
		18,19,
		17,16,
		9,
		8,
		9,9,9,9,9,9,
		25,24
	};

	//----scenery definitions -- where are these defined in the data files ??????
	public static List<string> sceneryList = new List<string> {
		"(L) Light Dirt",
		"(L) Dark Dirt",
		"(L) Bones",
		"(L) Bone",
		"(L) Ribs",
		"(L) Skull",
		"(L) Hanging Light #1",
		"(L) Hanging Light #2",
		"(L) Large Cylinder",
		"(L) Small Cylinder",
		"(L) Block #1",
		"(L) Block #2",
		"(L) Block #3",
		"(W) Pistol Clip",
		"(W) Short Light",
		"(W) Long Light",
		"(W) Siren",
		"(W) Rocks",
		"(W) Blood Drops",
		"(W) Filtration Device",
		"(W) Gun",
		"(W) Bob Remains",
		"(W) Puddles",
		"(W) Big Puddles",
		"(W) Security Monitor",
		"(W) Alien Supply Can",
		"(W) Machine",
		"(W) Fighter's Staff",
		"(S) Stubby Green Light",
		"(S) Long Green Light",
		"(S) Junk",
		"(S) Big Antenna #1",
		"(S) Big Antenna #2",
		"(S) Alien Supply Can",
		"(S) Bones",
		"(S) Big Bones",
		"(S) Pfhor Pieces",
		"(S) Bob Pieces",
		"(S) Bob Blood",
		"(P) Green Light",
		"(P) Small Alien Light",
		"(P) Alien Ceiling Rod Light",
		"(P) Bulbous Yellow Alien Object",
		"(P) Square Grey Organic Object",
		"(P) Pfhor Skeleton",
		"(P) Pfhor Mask",
		"(P) Green Stuff",
		"(P) Hunter Shield",
		"(P) Bones",
		"(P) Alien Sludge",
		"(J) Short Ceiling Light",
		"(J) Long Light",
		"(J) Weird Rod",
		"(J) Pfhor Ship",
		"(J) Sun",
		"(J) Large Glass Container",
		"(J) Nub #1",
		"(J) Nub #2",
		"(J) Lh'owon",
		"(J) Floor Whip Antenna",
		"(J) Ceiling Whip Antenna"
	};
	public static List<int> sceneryCollections = new List<int> {
		23,	23,	23,	23,	23,	23,	23,	23,	23,	23,	23,	23,	23,
		22,	22,	22,	22,	22,	22,	22,	22,	22,	22,	22,	22,	22,	22,	22,
		24,	24,	24,	24,	24,	24,	24,	24,	24,	24,	24,
		26,	26,	26,	26,	26,	26,	26,	26,	26,	26,	26,
		25,	25,	25,	25,	25,	25,	25,	25,	25,	25,	25
	};

	public static List<int> scenerySequences = new List<int> {
		3,4,5,6,7,8,9,10,11,12,13,14,15,
		4,5,7,9,10,21,11,12,13,14,15,16,17,18,20,
		5,7,4,9,10,11,13,17,12,14,15,4,14,16,
		6,7,9,10,11,12,13,18,
		5,7,4,9,10,11,13,17,12,14,15
	};


	//----liquid media definitions -- where are these defined in the data files ??????
	public static List<string> mediaList = new List<string> {
		"Water",
		"Lava",
		"Goo",
		"Sweage",
		"Jjaro"
	};

	public static List<int> mediaCollections = new List<int> {
		17,
		18,
		21,
		19,
		20
	};

	public static List<int> mediaBitmaps = new List<int> {
		19,
		12,
		5,
		13,
		13
	};

	public static List<Color> mediaColours = new List<Color> {
		new Color(123,158,166),
		new Color(210,160,49),
		new Color(210,19,139),
		new Color(105,114,30),
		new Color(205,207,87)		
	};

	public static List<float> mediaDensities = new List<float> {
		0.1f,
		0.3f,
		0.3f,
		0.2f,
		0.2f		
	};

	public static List<int> ambientSoundMappings = new List<int> {
		90,	// Water
		91,	// Sewage
		92,	// Lava
		93,	// Goo
		94,	// Under Media
		95,	// Wind
		96,	// Waterfall
		97,	// Siren
		98,	// Fan
		99,	// S'pht Door
		100,// S'pht Platform
		144,// Heavy S'pht Door
		102,// Heavy S'pht Platform
		103,// Light Machinery
		104,// Heavy Machinery
		105,// Transformer
		106,// Sparking Transformer
		169,// Machine Binder
		170,// Machine Bookpress
		171,// Machine Puncher
		172,// Electric Hum
		173,// Siren
		174,// Night Wind
		193,// Pfhor Door
		189,// Pfhor Platform
		201,// Pfhor Ship #1
		202,// Pfhor Ship #2
		101 // Jjaro
	};


	public static List<int> randomSoundMappings = new List<int> {
		107,	// dripping water
		175,	// thunder
		176,	// explosions
		29,	// loon
		4,	// jjaro ship
		176,	// explosions
	};

	public static void writeSettings() {
		string path = Application.persistentDataPath + "/settings.ini";
		if (!File.Exists(path)) {
			File.Create(path);
		}
		TextWriter tw = new StreamWriter(path);
		tw.WriteLine("shapesFilePath=" + GlobalData.shapesFilePath);
		tw.WriteLine("mapsFilePath=" + GlobalData.mapsFilePath);
		tw.WriteLine("soundsFilePath=" + GlobalData.soundsFilePath);
		tw.WriteLine("physicsFilePath=" + GlobalData.physicsFilePath);
		tw.WriteLine("imagesFilePath=" + GlobalData.imagesFilePath);
		tw.WriteLine("skipOcclusion=" + GlobalData.skipOcclusion);
		tw.WriteLine("occlusionOverDraw=" + GlobalData.occlusionOverDraw);
		tw.WriteLine("occlusionDensity=" + GlobalData.occlusionDensity);
		tw.WriteLine("captureMouse=" + GlobalData.captureMouse);
		tw.WriteLine("globalLighting=" + GlobalData.globalLighting);
		tw.WriteLine("globalLightingIntensity=" + GlobalData.globalLightingIntensity);
		tw.WriteLine("playerLight=" + GlobalData.playerLight);
		tw.WriteLine("playerLightType=" + GlobalData.playerLightType);
		tw.WriteLine("playerLightIntensity=" + GlobalData.playerLightIntensity);
		tw.WriteLine("playerLightPosition=" + GlobalData.playerLightPosition);
		tw.WriteLine("playerLightDelay=" + GlobalData.playerLightDelay);
		tw.WriteLine("playerLightRange=" + GlobalData.playerLightRange);
		tw.WriteLine("spriteType=" + GlobalData.spriteType);
		tw.WriteLine("landscapeType=" + GlobalData.landscapeType);
		tw.WriteLine("alwaysRun=" + GlobalData.alwaysRun);
		tw.WriteLine("graviyScaleFactor=" + GlobalData.graviyScaleFactor);
		tw.WriteLine("antiGraviyScaleFactor=" + GlobalData.antiGraviyScaleFactor);
		tw.WriteLine("accellerationScaleFactor=" + GlobalData.accellerationScaleFactor);
		tw.WriteLine("decellerationScaleFactor=" + GlobalData.decellerationScaleFactor);
		tw.WriteLine("deBounceFactor=" + GlobalData.deBounceFactor);
		tw.WriteLine("forceSpriteMultivews=" + GlobalData.forceSpriteMultivews);
		tw.WriteLine("playerFOV=" + GlobalData.playerFOV);
		tw.Close(); 


		path = Application.persistentDataPath + "/input.json";
		if (!File.Exists(path)) {
			File.Create(path);
		} 
		string json = JsonUtility.ToJson(GlobalData.inputController, true);
		tw = new StreamWriter(path);
		tw.Write(json);
		tw.Close(); 
	}

	public static void readSettings() {
		string path = Application.persistentDataPath + "/settings.ini";
		if (!File.Exists(path)) {
			writeSettings();
			return;
		}
		bool b;
		string[] lines = File.ReadAllLines(path);
		foreach (string line in lines) {
			if (line.StartsWith("shapesFilePath=")) {GlobalData.shapesFilePath = line.Replace("shapesFilePath=","").Trim();}
			if (line.StartsWith("mapsFilePath=")) {GlobalData.mapsFilePath = line.Replace("mapsFilePath=","").Trim();}
			if (line.StartsWith("soundsFilePath=")) {GlobalData.soundsFilePath = line.Replace("soundsFilePath=","").Trim();}
			if (line.StartsWith("physicsFilePath=")) {GlobalData.physicsFilePath = line.Replace("physicsFilePath=","").Trim();}
			if (line.StartsWith("imagesFilePath=")) {GlobalData.imagesFilePath = line.Replace("imagesFilePath=","").Trim();}
			// should set default values if the try parse fails... later
			if (line.StartsWith("skipOcclusion=")) {GlobalData.skipOcclusion = line.Replace("skipOcclusion=","").Trim() == "True";}
			if (line.StartsWith("occlusionOverDraw=")) {b = int.TryParse(line.Replace("occlusionOverDraw=","").Trim(), out GlobalData.occlusionOverDraw);}
			if (line.StartsWith("occlusionDensity=")) {b = float.TryParse(line.Replace("occlusionDensity=","").Trim(), out GlobalData.occlusionDensity);}
			if (line.StartsWith("captureMouse=")) {GlobalData.captureMouse = line.Replace("captureMouse=","").Trim() == "True";}
			if (line.StartsWith("globalLighting=")) {GlobalData.globalLighting = line.Replace("globalLighting=","").Trim() == "True";}
			if (line.StartsWith("globalLightingIntensity=")) {b = float.TryParse(line.Replace("globalLightingIntensity=","").Trim(), out GlobalData.globalLightingIntensity);}
			if (line.StartsWith("playerLight=")) {GlobalData.playerLight = line.Replace("playerLight=","").Trim() == "True";}
			if (line.StartsWith("playerLightIntensity=")) {b = float.TryParse(line.Replace("playerLightIntensity=","").Trim(), out GlobalData.playerLightIntensity);}
			if (line.StartsWith("playerLightType=")) {b = int.TryParse(line.Replace("playerLightType=","").Trim(), out GlobalData.playerLightType);}
			if (line.StartsWith("playerLightDelay=")) {b = float.TryParse(line.Replace("playerLightDelay=","").Trim(), out GlobalData.playerLightDelay);}
			if (line.StartsWith("playerLightPosition=")) {b = int.TryParse(line.Replace("playerLightPosition=","").Trim(), out GlobalData.playerLightPosition);}
			if (line.StartsWith("playerLightRange=")) {b = float.TryParse(line.Replace("playerLightRange=","").Trim(), out GlobalData.playerLightRange);}
			if (line.StartsWith("spriteType=")) {b = int.TryParse(line.Replace("spriteType=","").Trim(), out GlobalData.spriteType);}
			if (line.StartsWith("landscapeType=")) {b = int.TryParse(line.Replace("landscapeType=","").Trim(), out GlobalData.landscapeType);}
			if (line.StartsWith("alwaysRun=")) {GlobalData.alwaysRun = line.Replace("alwaysRun=","").Trim() == "True";}
			if (line.StartsWith("graviyScaleFactor=")) {b = float.TryParse(line.Replace("graviyScaleFactor=","").Trim(), out GlobalData.graviyScaleFactor);}
			if (line.StartsWith("antiGraviyScaleFactor=")) {b = float.TryParse(line.Replace("antiGraviyScaleFactor=","").Trim(), out GlobalData.antiGraviyScaleFactor);}
			if (line.StartsWith("accellerationScaleFactor=")) {b = float.TryParse(line.Replace("accellerationScaleFactor=","").Trim(), out GlobalData.accellerationScaleFactor);}
			if (line.StartsWith("decellerationScaleFactor=")) {b = float.TryParse(line.Replace("decellerationScaleFactor=","").Trim(), out GlobalData.decellerationScaleFactor);}
			if (line.StartsWith("deBounceFactor=")) {b = float.TryParse(line.Replace("deBounceFactor=","").Trim(), out GlobalData.deBounceFactor);}
			if (line.StartsWith("forceSpriteMultivews=")) {GlobalData.forceSpriteMultivews = line.Replace("forceSpriteMultivews=","").Trim() == "True";}
			if (line.StartsWith("playerFOV=")) {b = float.TryParse(line.Replace("playerFOV=","").Trim(), out GlobalData.playerFOV);}
		}

		path = Application.persistentDataPath + "/input.json";
		if (!File.Exists(path)) {
			writeSettings();
			return;
		}
		string json = File.ReadAllText(path);

		JsonUtility.FromJsonOverwrite(json, GlobalData.inputController);
	}
		

}

public class gameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GlobalData.inputController = new InputController();
		GlobalData.inputController.setDefaultKeys();
		GlobalData.readSettings();
	}
	// Update is called once per frame
	void Update () {
		
	}
	
}

public class AudioDefinition {
	public List<AudioClip> sounds;
	public bool cannotBeRestarted;
	public bool doesNotSelfAbort;
	public bool resistsPitchChanges ;
	public bool cannotChangePitch ;
	public bool cannotBeObstructed;
	public bool cannotBeMediaObstructed;
	public bool isAmbient;
	public double chance;
	public double lowPitch;
	public double highPitch;

}
	
[System.Serializable]
public class InputController {

	public List<button> buttons = new List<button>();
	public List<axis> axes = new List<axis>();

	public void setDefaultKeys() {
		axes.Add(new axis{
			axisName = "Mouse X",
			name = "Mouse X",
			sensitivity = 1
		});
		axes.Add(new axis{
			axisName = "Mouse Y",
			name = "Mouse Y",
			sensitivity = 1
		});


		buttons.Add(new button() {
			type = "button",
			keyCode = KeyCode.W,
			name = "Forwards",
			buttonName = "W"
		});
		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.S,
			name = "Backwards",
			buttonName = "S"
		});
		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.A,
			name = "Left",
			buttonName = "A"
		});
		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.D,
			name = "Right",
			buttonName = "D"
		});
		buttons.Add(new button {
			type = "axis",
			name = "Next Weap",
			axisName = "Mouse ScrollWheel",
			axisValue = 0.0001f,
			buttonName = "ScrUp"
		})	;
		buttons.Add(new button {
			type = "axis",
			name = "Prev Weap",
			axisName = "Mouse ScrollWheel",
			axisValue = -0.0001f,
			buttonName = "ScrDn"
		});


		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.Mouse0,
			name = "Trigger1",
			buttonName = "Mouse0"
		});
		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.Mouse1,
			name = "Trigger2",
			buttonName = "Mouse1"
		});

		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.Escape,
			name = "Menu",
			buttonName = "Escape"
		});
		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.E,
			name = "Action",
			buttonName = "E"
		});
		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.Tab,
			name = "Map",
			buttonName = "Tab"
		});

		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.LeftShift,
			name = "Run/Walk",
			buttonName = "LeftShift"
		});

		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.R,
			name = "Swim",
			buttonName = "R"
		});

		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.Tab,
			name = "Action",
			buttonName = "Tab"
		});

		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.LeftBracket,
			name = "Prev Inv",
			buttonName = "LeftBracket"
		});
		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.RightBracket,
			name = "Next Inv",
			buttonName = "RightBracket"
		});
		buttons.Add(new button {
			type = "button",
			keyCode = KeyCode.LeftAlt,
			name = "Show Cursor",
			buttonName = "LeftAlt"
		});

	}
	
	public bool getButton(string button) {
		foreach (button b in buttons) {
			if (b.name == button) {
				if (b.type == "button") {
					if (Input.GetKey(b.keyCode)){
						return true;
					}
				} else if (b.type == "axis") {
					if (Input.GetAxis(b.axisName) > b.axisValue && b.axisValue > 0) {
						return true;
					}
					if (Input.GetAxis(b.axisName) < b.axisValue && b.axisValue < 0) {
						return true;
					}
				}
			}
		}
		return false;
	}
	public float getAxis(string axis, bool raw = false) {
		foreach (axis a in axes) {
			if (a.name == axis) {
				if (raw) {
					return Input.GetAxisRaw(a.axisName) * a.sensitivity;
				} else {
					return Input.GetAxis(a.axisName) * a.sensitivity;
				}
			}
		}
		return 0;
	}

	[System.Serializable]
	public struct button {
		public string type;
		public KeyCode keyCode;
		public string buttonName;
		public string name;
		public string axisName;
		public float axisValue;
	}
	[System.Serializable]
	public struct axis {
		public string axisName;
		public string name;
		public float sensitivity;
	}


}
	