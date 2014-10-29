using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI () {
		//set instructions
		GUI.Box (new Rect ( Screen.width / 2 - 400 , Screen.height / 3 - 100, 800, 200), "\n Instructions: \n " +
		         "The game begins with a blank grid which you will fill and try to make plus shapes \n" +
		         "You will get a randomly colored cube in the top left on an interval, and will choose a row for the cube using the number keys \n" +
		         "You can move the colored cubes into adjacent white squares by first clicking on the cube to activate it, then clicking the white square \n" +
		         "Once you are happy with the cube's location, deselect it by clicking it again in order to be able to select other cubes  \n" +
		         "Try to make plus shapes that are either all one color, or one cube of every different color \n" +
		         "If you have a positive score when the time runs out, you win! \n" +
		         "You will be awarded bonuses for making multiple plusses at a time or for making plusses that are not right next to each other \n" +
		         "If you do not place a cube by the end of a turn, you will lose a point \n" +
		         "You will lose if you try to place a cube into a full row \n" +
		         "Good luck!");
		
		// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
		if(GUI.Button(new Rect(Screen.width / 2 - 100 , Screen.height / 4 * 2 + 50, 200, 100), "Start")) {
			Application.LoadLevel(0);

		}
		

	}
}
