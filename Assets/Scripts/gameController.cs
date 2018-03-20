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
