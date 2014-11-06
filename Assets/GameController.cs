using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

		//connects the cube and blank text prefabs
		public GameObject aCube;
		public GameObject textPrefab;
	
		//create an array to contain the cubes and a game object to hold the "next cube"
		private GameObject[,] allCubes;
		private GameObject predictorCube;

		//creates two ints to control the dimensions of the grid, edit these to change the dimensions of the grid
		public int gridHeight = 5;
		public int gridWidth = 8;


		//create an array of colors to be defined in the start function, and an int to hold the number of colors
		public static Color[] colors;
		public int numberOfColors = 5;
		
		//create an array of gameobjects to hold the textmesh above each cube, then another gameobject to hold the textmesh above the predictor
		public GameObject[,] textMesh;
		private GameObject predictorMesh;

		//creates the gameobject to hold the numbers to the left of each of the rows
		public GameObject[] numberMesh;

		//create a Color to hold the color of the cubes while they are being placed
		public Color cubeColor;

		//create two ints to keep track of the active x and y, instantiate off grid
		public int activeX = -4;
		public int activeY = -4;
	
		//create a bool to keep track of if a cube was placed this turn
		public bool alreadyPlaced;
	
		//create a bool to keep track of if there is an active cube
		public bool activeExists;

		//create two floats to control the game's allowed time and time between turns,
		//edit these if you want to change game legnth or turn legnth
		public float turnTime = 2;
		public float gameTimer;

		//create a timer to keep track of the game's turns
		public float turnTimer;

		//create a color to keep track of the last color that was spawned
		public Color oldColor;

		//create an int to control how many points are awarded for scoring a plus and one to control the bonus points for scoring a double plus
		//create another int to give more points for cubes in the middle and one for points for separate partitions
		public int plusPoints = 5;
		public int doublePoints = 10;
		public int middlePoints = 7;
		public int separatePoints = 8;

		//create an int to check how many turns it has been since the last turn that was scored, and set it to 0
		public int turnMultipliercount = 0;

		//create ints to control how many turns are allowed between scores to earn the bonus and how many times the bonus will multiply the points earned by
		public int withinTurns = 5;
		public int speedMultiplier = 2;

		//create an int for lost points if the cube is destroyed
		public int destroyedLostPoints = 1;
			
		//create an int that is static so it is accesible from the other scenes to control the score
		public static int score;

		//create floats to decide how much the active cube and moused over cubes will grow
		public float activeGrow = 1.5f;
		public float mouseOverGrow = 1.1f;

		//create a bool to check if the game has been lost
		public bool lost;

		

		// Use this for initialization
		void Start ()
		{

				// tell the game that the player hasnt lost yet and set the score to 0
				lost = false;
				score = 0;

				//set turntimer to 0 and the gametimer to 60
				turnTimer = 0;
				gameTimer = 60;


	
				//tells the arrays how many things they will contain
				allCubes = new GameObject[gridWidth, gridHeight];
				colors = new Color[numberOfColors + 2];
				textMesh = new GameObject[gridWidth, gridHeight];
				numberMesh = new GameObject[gridHeight];
	

				//sets the colors in the array, 0-4 are for active, 5 is empty and 6 is inactive
				colors [0] = Color.black;
				colors [1] = Color.blue;
				colors [2] = Color.green;
				colors [3] = Color.red;
				colors [4] = Color.yellow;
				colors [5] = Color.white;
				colors [6] = Color.gray;

	


				//instantiates the grid of cubes and a grid of textmeshes on top of them
				for (int x = 0; x < gridWidth; x++) {
						for (int y = 0; y < gridHeight; y++) {
								allCubes [x, y] = (GameObject)Instantiate (aCube, new Vector3 (x * 2, y * 2, 0), Quaternion.identity);
								textMesh [x, y] = (GameObject)Instantiate (textPrefab, new Vector3 (x * 2 - 0.4f, y * 2 + 0.4f, 0), Quaternion.identity);
								textMesh [x, y].GetComponent<TextMesh> ().text = " ";
								//sets the x and y components in the cube behavior script and set the color to the inactive color
								allCubes [x, y].GetComponent<CubeBehavior> ().x = x;
								allCubes [x, y].GetComponent<CubeBehavior> ().y = y;
								allCubes [x, y].renderer.material.color = colors [5];

						}		
				}
				//instantiate the numbers to the left of the grid telling you which to input
				for (int y = 0; y < gridHeight; y++) {
						numberMesh [y] = (GameObject)Instantiate (textPrefab, new Vector3 (-2, y * 2 + 0.7f, 0), Quaternion.identity);
						//it is abs(y-gridHeight) in order to reverse the order, if i just did gridheight - 1 (-1 to compensate for the array) it would go 4 3 2 1 0
						numberMesh [y].GetComponent<TextMesh> ().text = (Mathf.Abs (y - gridHeight)).ToString ();
				}

				//instantiates the cube which lets you know which color will be next, puts a mesh on top of it and a label over it
				predictorCube = (GameObject)Instantiate (aCube, new Vector3 (-2, 10, 0), Quaternion.identity);
				predictorMesh = (GameObject)Instantiate (textPrefab, new Vector3 (-2.3f, 10.5f, 0), Quaternion.identity);
				predictorMesh.GetComponent<TextMesh> ().text = " ";
				GameObject predictorLabel = (GameObject)Instantiate (textPrefab, new Vector3 (-6.7f, 10.5f, 0), Quaternion.identity);
				predictorLabel.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				predictorLabel.GetComponent<TextMesh> ().text = "Next Cube: ";
			
				//sets activeExists to false
				activeExists = false;
				
				//makes the game start during a turn instead of making the player wait turntime seconds
				ProcessNewTurn ();

		}
	
		// Update is called once per frame
		void Update ()
		{

				scoreCubes ();

				//every turn length, reset the timer and do the new turn function
				if (turnTimer < turnTime) {
						turnTimer += Time.deltaTime;
				} else {
						ProcessNewTurn ();
						turnTimer = 0;
				}
			
				//end the game after the time limit
				if (0 < gameTimer) {
						gameTimer -= Time.deltaTime;
						// if the time is up, check the score.  If it is greater than 0, process a win, otherwise process a loss
				} else if (score > 0) {
						//give bonus for separate partitions if they won then load the win scene
						GiveSeparatePartitionBonus ();
						Application.LoadLevel (2);
						
				
				
				} else {
						//load the loss screen
						lost = true;
				}

				//check if anything was entered in the keyboard
				ProcessKeyboardInput ();
				
				//if the player loses at any point, bring them to the loss screen
				if (lost == true) {
						Application.LoadLevel (3);
				}
				

		}

		//this method creates a GUI.Label to display the score and timer
		void OnGUI ()
		{       
				GUI.Label (new Rect (10, 380, 100, 20), "Score: " + score.ToString ());   
		
				//if there are less than ten seconds, turn the timer red
				if (gameTimer < 10) {
						GUI.contentColor = Color.red;
				}
				GUI.Label (new Rect (10, 400, 100, 40), "Time: " + (Mathf.Round (gameTimer * 10) / 10).ToString ());
		
		
		
		
		}

		//this method deals with everything that happens on a new turn
		public void ProcessNewTurn ()
		{

		//if a cube wasnt used last turn, subtract 1 from the score up to 0, and destroy a random white cube. 
		//If there is no white cube, lose the game

				if (alreadyPlaced == false) {
						score = Mathf.Max (0, score - destroyedLostPoints);
			}
 
		
				if (alreadyPlaced == false && Time.time > (turnTime - 0.1f)) {
			//check if there is a white cube to destroy
			bool fullGrid = true;
			for (int i = 0; i < gridWidth; i++) {
				for (int j = 0; j < gridHeight; j++) {
					if (allCubes [i, j].renderer.material.color == colors [5] && allCubes [i, j].renderer.enabled == true) {
						fullGrid = false;
					}
				}
			}
			if (fullGrid == true) {
				//application.loadlevel3 is repeated here instead of lost = true so that the game doesn't try to go through the 
				//rest of the function before it goes to the load screen since that would cause a crash
				Application.LoadLevel (3);

					
			}
			// if the grid isn't full, it will look at random cubes until it finds a white one, then disable the renderer and set it to inactive
			else {
						int randomx, randomy;
						bool hasDestroyed = false;
						do {
								randomx = Random.Range (0, gridWidth);
								randomy = Random.Range (0, gridHeight);

								if (allCubes [randomx, randomy].renderer.material.color == colors [5]) {

										allCubes [randomx, randomy].renderer.material.color = colors [6];
										allCubes [randomx, randomy].renderer.enabled = false;
										hasDestroyed = true;
								}

						} while (hasDestroyed == false);
			}
						
				}
				
				//if a cube was placed last turn then the predictable cube disappeared, undo this on the new turn
				predictorCube.renderer.enabled = true;
				predictorMesh.renderer.enabled = true;


				//next line was taken from http://answers.unity3d.com/questions/603000/generating-a-good-random-seed.html, it makes the random call more random
				Random.seed = (int)System.DateTime.Now.Ticks;

				//keep generating a random color until it is different than the one that was used last turn
				do {
						cubeColor = colors [(Random.Range (0, numberOfColors))];
				} while (cubeColor == oldColor);
				
				//set the current color into old color for the next turn, then color the predictor cube according to the color
				oldColor = cubeColor;
				ColorCubes (predictorCube, cubeColor, -2, 10);

				//set already placed to false
				alreadyPlaced = false;
			
				//take one away fromt he multiplier countdown
				turnMultipliercount --;
		}

		//this method handles keyboard input
		public void ProcessKeyboardInput ()
		{
				//if you haven't placed a cube yet, let the user enter a row to put the cube into
				if (alreadyPlaced == false) {
						//place a cube in the set column
						if (Input.GetKeyDown ("1")) {
								PlaceCube (gridHeight - 1, cubeColor);
			
						} else if (Input.GetKeyDown ("2")) {
								PlaceCube (gridHeight - 2, cubeColor);
			
						} else if (Input.GetKeyDown ("3")) {
								PlaceCube (gridHeight - 3, cubeColor);
			
						} else if (Input.GetKeyDown ("4")) {
								PlaceCube (gridHeight - 4, cubeColor);
			
						} else if (Input.GetKeyDown ("5")) {
								PlaceCube (gridHeight - 5, cubeColor);
			
						}
				}
		}
		//this method will be called each time a cube is moused over and will increase the scale of the object by 1.2
		public void ProcessMouseOver (GameObject mousedOverCube, int x, int y)
		{
				//if a cube is clickable and moused over, make it grow
				//a cube is clickable if there  is no active cube and it is colored, OR if there is an active cube and it is adjacent
				if ((allCubes [x, y].renderer.material.color != colors [5] && allCubes [x, y].renderer.material.color != colors [6]
						&& activeExists == false)
						|| (activeExists == true 
						&& allCubes [x, y].renderer.material.color == colors [5]
						&& allCubes [x, y].renderer.material.color != colors [6]
						&& (activeX == x || activeX == x + 1 || activeX == x - 1)
						&& (activeY == y || activeY == y + 1 || activeY == y - 1))) {
						allCubes [x, y].transform.localScale = new Vector3 (mouseOverGrow, mouseOverGrow, mouseOverGrow);
				}
		}

		//this method will be called on the mouse exiting and will take away the increase from ProcessMouseOver
		public void ProcessMouseExit (GameObject mousedExitedCube, int x, int y)
		{
				//if the cube isn't active, when the mouse is no longer over it, reset its size to normal
				if (allCubes [x, y].GetComponent<CubeBehavior> ().isActive == false) {
						allCubes [x, y].transform.localScale = new Vector3 (1f, 1f, 1f);
				}
		}

		//this method processes every time a cube is clicked
		public void ProcessClickedCube (GameObject clickedCube, int x, int y)
		{
		
				//if there is no active cube and one is clicked, highlight it then set it to active and activeExists to true
				if (allCubes [x, y].renderer.material.color != colors [5] && allCubes [x, y].renderer.material.color != colors [6] && activeExists == false) {
						allCubes [x, y].transform.localScale = new Vector3 (activeGrow, activeGrow, activeGrow);
						allCubes [x, y].GetComponent<CubeBehavior> ().isActive = true;
						activeExists = true;
						activeX = x;
						activeY = y;
			
				}
		
		//if the player clicks an active cube, it should deactivate
		else if (activeExists == true && allCubes [x, y].GetComponent<CubeBehavior> ().isActive) {
						allCubes [x, y].transform.localScale = new Vector3 (1f, 1f, 1f);
						allCubes [x, y].GetComponent<CubeBehavior> ().isActive = false;
						activeExists = false;
						activeX = -4;
						activeY = -4;
			
				}
		
		
				//if there is an active cube and a nonactive cube is clicked, check if it is adjacent (includes diagonals)
				if (allCubes [x, y].renderer.material.color == colors [5] && activeExists == true) {
						//check all directions, after checking if there is a cube in that location first
						if (x > 0 && y < gridHeight - 1) {
								if (allCubes [x - 1, y + 1].GetComponent<CubeBehavior> ().isActive) {
										ShiftCube (x - 1, y + 1, x, y);
								}
						}
						if (x > 0 && y > 0) {
								if (allCubes [x - 1, y - 1].GetComponent<CubeBehavior> ().isActive) {
										ShiftCube (x - 1, y - 1, x, y);
					
								}
						}
						if (x < gridWidth - 1 && y > 0) {
								if (allCubes [x + 1, y - 1].GetComponent<CubeBehavior> ().isActive) {
										ShiftCube (x + 1, y - 1, x, y);
					
								}
						}
						if (x < gridWidth - 1 && y < gridHeight - 1) {
								if (allCubes [x + 1, y + 1].GetComponent<CubeBehavior> ().isActive) {
										ShiftCube (x + 1, y + 1, x, y);
					
								}
						}
						if (y < gridHeight - 1) {
								if (allCubes [x, y + 1].GetComponent<CubeBehavior> ().isActive) {
										ShiftCube (x, y + 1, x, y);
					
								}
						}
						if (y > 0) {
								if (allCubes [x, y - 1].GetComponent<CubeBehavior> ().isActive) {
										ShiftCube (x, y - 1, x, y);
					
								}
						}
						if (x < gridWidth - 1) {
								if (allCubes [x + 1, y].GetComponent<CubeBehavior> ().isActive) {
										ShiftCube (x + 1, y, x, y);
					
								}
						}
						if (x > 0) {
								if (allCubes [x - 1, y].GetComponent<CubeBehavior> ().isActive) {
										ShiftCube (x - 1, y, x, y);
					
								}
						}
				}
		
		}


		// this method will place the new cube in the inputted row
		public void PlaceCube (int y, Color color)
		{
				//set alreadyPlaced to false

				alreadyPlaced = false;


				//check if the row is full before attempting to place it: fixed a crash error
				//gridwidth - 1 to account for the fact that the grid is an array

				
				//create a bool named fullRow to keep track of if the entered row is full.  Set its default to true, then 
				//check if there is an empty space.  If there is, change the bool to false.
				bool fullRow = true;
				for (int i = 0; i < gridWidth - 1; i++) {
						if (allCubes [i, y].renderer.material.color == colors [5]) {
								fullRow = false;
						}

		
				}
				//if a player tries to place a cube in a full row, they lose

				if (fullRow == true) {
						lost = true;		
				}

				//if the row isnt full, place the cube randomly in the designated row, then clear the next cube slot. 
				if (fullRow == false) {
						do {
								//tell it to go into a random place in the row and place it if the selected random spot is empty.
								//if it is empty, keep going until an empty slot is found. 
								int x = (Random.Range (0, gridWidth));
								if (allCubes [x, y].renderer.material.color == colors [5]) {
										ColorCubes (allCubes [x, y], color, x, y);
										//once it is placed, set already placed to true and disable the predictor cube's renderer
										alreadyPlaced = true;
										predictorCube.renderer.enabled = false;
										predictorMesh.renderer.enabled = false;

								}
						} while (alreadyPlaced == false);
				}
		}

		//makes a method to set the designated cube to the designated color 
		public void ColorCubes (GameObject cube, Color color, int x, int y)
		{

		
				

				//set the correct cube to the correct color
				if (cube != predictorCube) {
						allCubes [x, y].renderer.material.color = color;
						ProcessMesh (x, y, allCubes [x, y]);
				} else {
						predictorCube.renderer.material.color = color;
						ProcessMesh (x, y, predictorCube);
				}

		}
	
		//this method processes the mesh on top of each cube
		void ProcessMesh (int x, int y, GameObject thisCube)
		{
				//set the mesh to correspond with the color
				if (thisCube != predictorCube) {
						if (thisCube.renderer.material.color == colors [0]) {
								textMesh [x, y].GetComponent<TextMesh> ().text = "A";
						}
						if (thisCube.renderer.material.color == colors [1]) {
								textMesh [x, y].GetComponent<TextMesh> ().text = "B";
						}
						if (thisCube.renderer.material.color == colors [2]) {
								textMesh [x, y].GetComponent<TextMesh> ().text = "C";
						}
						if (thisCube.renderer.material.color == colors [3]) {
								textMesh [x, y].GetComponent<TextMesh> ().text = "D";
						}
						if (thisCube.renderer.material.color == colors [4]) {
								textMesh [x, y].GetComponent<TextMesh> ().text = "E";
						}
						if (thisCube.renderer.material.color == colors [5] || thisCube.renderer.material.color == colors [6]) {
								textMesh [x, y].GetComponent<TextMesh> ().text = " ";
				
						}
				} else {
						if (thisCube.renderer.material.color == colors [0]) {
								predictorMesh.GetComponent<TextMesh> ().text = "A";
						}
						if (thisCube.renderer.material.color == colors [1]) {
								predictorMesh.GetComponent<TextMesh> ().text = "B";
						}
						if (thisCube.renderer.material.color == colors [2]) {
								predictorMesh.GetComponent<TextMesh> ().text = "C";
						}
						if (thisCube.renderer.material.color == colors [3]) {
								predictorMesh.GetComponent<TextMesh> ().text = "D";
						}
						if (thisCube.renderer.material.color == colors [4]) {
								predictorMesh.GetComponent<TextMesh> ().text = "E";
						}
			
				}
		
		}


		// this method moves a cube to an adjacent one
		public void ShiftCube (int oldx, int oldy, int newx, int newy)
		{
				//move the attributes to the new cube
				allCubes [newx, newy].transform.localScale = new Vector3 (activeGrow, activeGrow, activeGrow);
				allCubes [newx, newy].GetComponent<CubeBehavior> ().isActive = true;
				activeX = newx;
				activeY = newy;
				allCubes [newx, newy].GetComponent<CubeBehavior> ().renderer.material.color = allCubes [oldx, oldy].GetComponent<CubeBehavior> ().renderer.material.color;
		
				ColorCubes (allCubes [newx, newy], allCubes [oldx, oldy].renderer.material.color, newx, newy);
			
	
				//set the old cube back to normal
				allCubes [oldx, oldy].transform.localScale = new Vector3 (1f, 1f, 1f);
				allCubes [oldx, oldy].GetComponent<CubeBehavior> ().isActive = false;
				allCubes [oldx, oldy].renderer.material.color = colors [5];
				ProcessMesh (oldx, oldy, allCubes [oldx, oldy]);

		}

		//this method takes care of scoring the cubes
		public void scoreCubes ()
		{
				//check all non edge cubes for scoring
				for (int x = 1; x < gridWidth - 1; x++) {
						for (int y = 1; y < gridHeight - 1; y++) {
								//set variables to gather the colors from cubes in a plus shape around x, y
								Color color1 = allCubes [x, y + 1].renderer.material.color;
								Color color2 = allCubes [x - 1, y].renderer.material.color;
								Color color3 = allCubes [x, y].renderer.material.color;
								Color color4 = allCubes [x + 1, y].renderer.material.color;
								Color color5 = allCubes [x, y - 1].renderer.material.color;
			
								//create a bool called isPlus and use it to check if there is a valid plus to be scored
								bool isPlus = CheckAdjacent (x, y);
					
								//if none of the colors are blank and there is a plus, score check for adjacent plusses and score it
								if (color1 != colors [5] && color2 != colors [5] && color3 != colors [5] && color4 != colors [5] && color5 != colors [5]) {
										if (isPlus) {
												//check for double plus
												bool isDoublePlus = false;
												if (x < 5) { 
														isDoublePlus = CheckAdjacent (x + 2, y);
														if (isDoublePlus == true) {
																//process double plus
																ProcessPlus (x + 2, y);

														} 
														

																		
												}
												if (y < 2) { 
														isDoublePlus = CheckAdjacent (x, y + 2);
														if (isDoublePlus) {
																//process double plus
																ProcessPlus (x, y + 2);
														}
												}
												//deactivate the cubes and score
												ProcessPlus (x, y);
										}

								}
			

					
						}

				
			
				}
		}
				
		
		//checks around a cube for a valid plus to score, returns true if it is scorable
		public bool CheckAdjacent (int x, int y)
		{
				//create variables to contain the colors of the plus around x, y
				Color c1 = allCubes [x, y + 1].GetComponent<CubeBehavior> ().renderer.material.color;
				Color c2 = allCubes [x - 1, y].GetComponent<CubeBehavior> ().renderer.material.color;
				Color c3 = allCubes [x, y].GetComponent<CubeBehavior> ().renderer.material.color;
				Color c4 = allCubes [x + 1, y].GetComponent<CubeBehavior> ().renderer.material.color;
				Color c5 = allCubes [x, y - 1].GetComponent<CubeBehavior> ().renderer.material.color;
				// if none of them are blank or inactive, check for if all of the colors match or are differen
				// if all three of the conditions above are matched return true, otherwise return false
				if (c1 != colors [5] && c2 != colors [5] && c3 != colors [5] && c4 != colors [5] && c5 != colors [5]) {
						if (c1 != colors [6] && c2 != colors [6] && c3 != colors [6] && c4 != colors [6] && c5 != colors [6]) {

								if (c1 == c2 && c2 == c3 && c3 == c4 && c4 == c5 ||
										c1 != c2 && c1 != c3 && c1 != c4 && c1 != c5 && 
										c2 != c3 && c2 != c4 && c2 != c5 && 
										c3 != c4 && c3 != c5 &&
										c4 != c5) {
										return true;
								} else {
										return false;
								}
						} else {
								return false;
						}
		
				} else {
						return false;
				}
				
	
		}
		
		// this method is called when something is scored
		
		public void ProcessPlus (int x, int y)
		{
				//If there has been a match within the last "withinTurns" turns and where the cube is and give points accordingly
				if (turnMultipliercount > 0) {
						if (x > 2 && x < gridWidth - 2 && y > 2 && y < gridHeight - 2) {
								score += (middlePoints * speedMultiplier);
						} else {
								score += (plusPoints * speedMultiplier);
						}
				} else {

						if (x > 2 && x < gridWidth - 2 && y > 2 && y < gridHeight - 2) {
								score += middlePoints;
						} else {
								score += plusPoints;
						}
				}
				//reset the turn countdown, and disble all of the cubes in the scored plus
				turnMultipliercount = withinTurns;
				DisableCube (x, y);
				DisableCube (x, y + 1);
				DisableCube (x - 1, y);
				DisableCube (x + 1, y);
				DisableCube (x, y - 1);


		
		}

		//this method handles any cubes that have been scored
		public void DisableCube (int x, int y)
		{
				//color the cube at the specified location with the inactive color
				allCubes [x, y].renderer.material.color = colors [6];
				textMesh [x, y].renderer.enabled = false;

			
				//if the cube is active, set it to inactive
				if (allCubes [x, y].GetComponent<CubeBehavior> ().isActive == true) {
						activeExists = false;
						activeX = -4;
						activeY = -4;
						allCubes [x, y].transform.localScale = new Vector3 (1f, 1f, 1f);
						allCubes [x, y].GetComponent<CubeBehavior> ().isActive = false;
				}
	
		}



		//this method checks for if cubes are touching each other or not and gibes a bonus if the cubes are separate; it
		void GiveSeparatePartitionBonus ()
		{
				// check the middle of the plus for if there is any gray cubes diagonally touching it.  if not, give +separatepoints
				for (int x = 1; x < gridWidth - 1; x++) {
						for (int y = 1; y < gridHeight - 1; y++) {
								if (allCubes [x, y].renderer.material.color == colors [6] && allCubes [x + 1, y].renderer.material.color == colors [6] &&
										allCubes [x - 1, y].renderer.material.color == colors [6] && allCubes [x, y + 1].renderer.material.color == colors [6] &&
										allCubes [x, y - 1].renderer.material.color == colors [6]) {
										if (allCubes [x - 1, y - 1].renderer.material.color != colors [6] && allCubes [x + 1, y - 1].renderer.material.color != colors [6] &&
												allCubes [x - 1, y + 1].renderer.material.color != colors [6] && allCubes [x + 1, y + 1].renderer.material.color != colors [6]) {
												score += separatePoints;
										}
								}
						}
				}

		}



}