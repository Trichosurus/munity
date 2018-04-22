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




	//----item definitions -- where are these defined in the data files ??????
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
