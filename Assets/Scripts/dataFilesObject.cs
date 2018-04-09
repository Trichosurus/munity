using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;

public class dataFilesObject : MonoBehaviour {

	public string type;

	// Use this for initialization
	void Start () {
		drawString();
	}
	
	/// <summary>
	/// OnMouseUp is called when the user has released the mouse button.
	/// </summary>
	void OnMouseUp()
	{
		if (!settingsScreen.choosingFile) {
			FileBrowser.SetFilters( true, new FileBrowser.Filter( "Maps", ".scea", ".sceA" ), 
											new FileBrowser.Filter( "Shapes", ".shpa", ".shpA" ),
											new FileBrowser.Filter( "Sounds", ".snda", ".sndA" ),
											new FileBrowser.Filter( "Physics", ".phya", ".phyA" ),
											new FileBrowser.Filter( "Images", ".imga", ".imgA" )
									);
			FileBrowser.SetDefaultFilter( type );
			// FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );
			// FileBrowser.AddQuickLink( "Data", "?game path?recources folder?", null );
			StartCoroutine( ShowLoadDialogCoroutine() );
		}

	}

	IEnumerator ShowLoadDialogCoroutine()
	{
		settingsScreen.choosingFile = true; 
		// Show a load file dialog and wait for a response from user
		// Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
		yield return FileBrowser.WaitForLoadDialog( false, null, "Choose " + type + " file", "Select" );

		// Dialog is closed
		// Print whether a file is chosen (FileBrowser.Success)
		// and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
		Debug.Log( FileBrowser.Success + " " + FileBrowser.Result );
		if (FileBrowser.Success) {
			switch (type) {
				case "Maps":
					GlobalData.mapsFilePath = FileBrowser.Result;
					break;
				case "Shapes":
					GlobalData.shapesFilePath = FileBrowser.Result;
					break;
				case "Sounds":
					GlobalData.soundsFilePath = FileBrowser.Result;
					break;
				case "Physics":
					GlobalData.physicsFilePath = FileBrowser.Result;
					break;
				case "Images":
					GlobalData.imagesFilePath = FileBrowser.Result;
					break;	
			}
			writeSettings();
		}
				settingsScreen.choosingFile = false; 


	}

	// Update is called once per frame
	void Update () {
		
	}

	void drawString() {
		int lineChars = 44;
		string text = "";
		GameObject textObj = gameObject.transform.Find("text").gameObject;
		switch (type) {
			case "Maps":
				text = GlobalData.mapsFilePath;
				break;
			case "Shapes":
				text = GlobalData.shapesFilePath;
				break;
			case "Sounds":
				text = GlobalData.soundsFilePath;
				break;
			case "Physics":
				text = GlobalData.physicsFilePath;
				break;
			case "Images":
				text = GlobalData.imagesFilePath;
				break;	
		}

		// if (lineChars > text.Length) {
		// 	lineChars = text.Length;
		//}
		string wrappedText="";
		for (int i = 0; (i) * lineChars < text.Length; i++) {
			int len = lineChars;
			if (lineChars*(i+1) > text.Length) {
				len = lineChars - (lineChars*(i+1) - text.Length);
			}
			wrappedText += text.Substring(i*lineChars,len) + "\n";
		}

		textObj.GetComponent<TextMesh>().text = wrappedText;
		
	}

	void writeSettings() {
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
		tw.Close(); 
		drawString();

	}
}
