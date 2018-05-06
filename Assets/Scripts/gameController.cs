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

	public static bool skipOcclusion = true;
	public static int occlusionOverDraw = 3;

	public static bool captureMouse = true;

	public static bool globalLighting = true;
	public static bool playerLight = true;
	public static int spriteType = 2;




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
}

public class gameController : MonoBehaviour {

	// Use this for initialization
	void Start () {

		string path = Application.persistentDataPath + "/settings.ini";
		if (!File.Exists(path)) {
			File.Create(path);
			TextWriter tw = new StreamWriter(path);
			tw.WriteLine("shapesFilePath=" + GlobalData.shapesFilePath);
			tw.WriteLine("mapsFilePath=" + GlobalData.mapsFilePath);
			tw.WriteLine("soundsFilePath=" + GlobalData.soundsFilePath);
			tw.WriteLine("physicsFilePath=" + GlobalData.physicsFilePath);
			tw.WriteLine("imagesFilePath=" + GlobalData.imagesFilePath);
			tw.Close(); 
		} else {
			string[] lines = System.IO.File.ReadAllLines(path);
			foreach (string line in lines) {
				if (line.StartsWith("shapesFilePath=")) {GlobalData.shapesFilePath = line.Replace("shapesFilePath=","").Trim();}
				if (line.StartsWith("mapsFilePath=")) {GlobalData.mapsFilePath = line.Replace("mapsFilePath=","").Trim();}
				if (line.StartsWith("soundsFilePath=")) {GlobalData.soundsFilePath = line.Replace("soundsFilePath=","").Trim();}
				if (line.StartsWith("physicsFilePath=")) {GlobalData.physicsFilePath = line.Replace("physicsFilePath=","").Trim();}
				if (line.StartsWith("imagesFilePath=")) {GlobalData.imagesFilePath = line.Replace("imagesFilePath=","").Trim();}
			}
		}


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
