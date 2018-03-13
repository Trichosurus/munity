using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// OnMouseUp is called when the user has released the mouse button.
	/// </summary>
	void OnMouseUp()
	{
		
		 SceneManager.LoadScene("gameScene", LoadSceneMode.Single);

		 StartCoroutine (WaitForSceneLoad (SceneManager.GetSceneByName("gameScene")));
	}
	public IEnumerator WaitForSceneLoad(Scene scene){
	while(!scene.isLoaded){
		yield return null;  
	}
	Debug.Log("Setting active scene..");
	SceneManager.SetActiveScene (scene);
}

	
}
