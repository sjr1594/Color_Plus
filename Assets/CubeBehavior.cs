using UnityEngine;
using System.Collections;

public class CubeBehavior : MonoBehaviour {

	public int x, y;
	public bool isActive;

	//lets this script access the GameController object
	GameController aGameController;

	// Use this for initialization
	void Start () {
		//lets this script access the GameController script
		aGameController = GameObject.Find ("GameController").GetComponent<GameController> ();

	

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver ()
	{
		aGameController.ProcessMouseOver (this.gameObject, x, y);
		
	}
	
	void OnMouseExit ()
	{
		aGameController.ProcessMouseExit (this.gameObject, x, y);
		
	}
	


	
	void OnMouseDown ()
	{
		//every time a cube is clicked, it will call the method ProcessClickedCube on the clicked game object
		aGameController.ProcessClickedCube (this.gameObject, x, y);
		
		
	}


	
	
}