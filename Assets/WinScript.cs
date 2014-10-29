using UnityEngine;
using System.Collections;

public class WinScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {

		GUI.Box (new Rect ( Screen.width / 2 , Screen.height / 3, 200, 50), "You win!!!! \n " +
			"Your score was:" + GameController.score.ToString());


		if (GUI.Button (new Rect (Screen.width / 2, (Screen.height / 2), 200, 50),
		                ("Play again?"))) {
			Application.LoadLevel (0);
			
		}
	}

}
