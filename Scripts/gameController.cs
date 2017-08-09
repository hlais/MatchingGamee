using UnityEngine;
using System.Collections;

public class gameController : MonoBehaviour {
	private int numRows = 5;   //number of rows in the grid
	private int numCols = 5;  //number of columns in the grid
	private GameObject[,] pieces;	//the game pieces out on my grid
	private Color[] pieceColors;		//the colors that a game piece can be
	
	private GameObject[] selectedPieces;	//the two pieces that player has clicked
	private int numSelected;	//how many pieces has the player selected
	private bool weSwitched;	//track if we have switched pieces or not
	
	private ArrayList matchedPieces;	//list of pieces that match
	
	// Use this for initialization
	void Start () {
		//----------------------------------------------------------
		//ALWAYS INITIALIZE YOUR ARRAY AND NON PRIMITIVE DATA TYPES
		pieces = new GameObject[numRows, numCols];
		pieceColors = new Color[6] {Color.red, Color.blue,
									Color.yellow, Color.green,
									Color.cyan, Color.magenta};
		selectedPieces = new GameObject[2];
		matchedPieces = new ArrayList();
		//DONE WITH ALL OF THE INITIALIZATIONS
		//----------------------------------------------------------

		//build the grid and put the pieces out there
		for (int i = 0; i < numRows; i++) {
			for (int j = 0; j < numCols; j++) {
				//create a game piece
				pieces[i,j] = Instantiate(Resources.Load ("Cube",
											typeof(GameObject)))
											as GameObject;
				//position on the screen
				pieces[i,j].transform.position = new Vector3(i*1.2f-3, j*-1.2f+3, 0);
				//give the game piece a random color
				pieces[i,j].GetComponent<pieceController>().setColor (pieceColors[Random.Range(0,6)]);
				//store our grid location for swapping later
				pieces[i, j].GetComponent<pieceController>().myGridX = i;
				pieces[i, j].GetComponent<pieceController>().myGridY = j;
			}//end of j loop
		}//end of i for loop
		//now lets MasterServerEvent if there were any matches made byte Random spawn
		while (isMatching ()) 
			swapColors ();
		matchedPieces.Clear ();  //clear them out just in case we found some
		weSwitched = false;  //re-initialize this variable to false for game play
	}//end of start
	
	// Update is called once per frame
	void Update () {
		//lets see if we should switch colors because 2 pieces have been selected
		if (numSelected == 2) {
			if (!weSwitched) {  //TEMPORARY FIX FOR DOUBLING OF GRID RESET
				//go switch out the colors
				switchPieces();
			}
			if (selectedPieces[0].GetComponent<pieceController>().isArrived () &&
			       selectedPieces[1].GetComponent<pieceController>().isArrived ()) {
				//if there are any matches then change the matching colors
				if (isMatching ()){
					weSwitched = true;
				}//cycles through the whole grid and finds all matching pieces
				//while (isMatching()) {
					//we found some matches, go swap the colors
					//swapColors();
					//matchedPieces.Clear();  //dump our list of matching pieces
				//}//end of while(isMatching)

				//what if we never found any matches????
				if (matchedPieces.Count < 3) {
					if (!weSwitched) {
						returnPieces();  //no matches found, put them back
					}
					//let the pieces know they have been unselected and also get numSelected back to 0!
					selectedPieces[1].GetComponent<pieceController>().OnMouseDown ();
					selectedPieces[0].GetComponent<pieceController>().OnMouseDown ();
					matchedPieces.Clear();  //dump our list of matching pieces
					weSwitched = false;
				}//end of !weSwitched
				else {
					//let the pieces know they have been unselected and also get numSelected back to 0!
					swapColors();
					//selectedPieces[1].GetComponent<pieceController>().OnMouseDown ();
					//selectedPieces[0].GetComponent<pieceController>().OnMouseDown ();
					matchedPieces.Clear();  //dump our list of matching pieces
				}//end else matchedPieces.Count < 3
			}//end of if the pieces have arrived
		}//end of if numSelected == 2
	}//end of update
	
	//method to handle the finding of matches
	private bool isMatching() {
		bool returnVal = false;
		//------------------------
		//check for matching cols
		for(int i = 0; i < numRows; i++ ) {
			for (int j = 0; j < numCols -2; j++) {
				//grab the colors of first three pieces
				Color col1 = pieces[i,j].GetComponent<pieceController>().getColor ();
				Color col2 = pieces[i,j+1].GetComponent<pieceController>().getColor ();
				Color col3 = pieces[i,j+2].GetComponent<pieceController>().getColor ();
				
				//lets see if we have a match
				if (col1 == col2 && col1 == col3) {
					//we have a match
					returnVal = true;
					//since we have a match, lets put those pieces in our list of matching pieces
					matchedPieces.Add (pieces[i,j]);
					matchedPieces.Add (pieces[i,j+1]);
					matchedPieces.Add (pieces[i,j+2]);
					//lets see if we can find some more in this row
					int offset = j+3;  //get ready to shift to past the matching set
					j = j+2;  //shift j over to the last piece in the matching set
					while (offset < numCols) {
						col2 = pieces[i,offset].GetComponent<pieceController>().getColor ();
						if (col1 == col2) {
							//we have found another match, add it to our set and increase offset
							matchedPieces.Add (pieces[i, offset]);
							j = offset;
							offset++;	
						}//end of color check
						else {
							//well darn, they did not match
							break;
						}//end of else
					}//end of while loop
				}//end of three color check
			}//end of j
		}//end of i
		//done checking for matching cols
		//--------------------------
		//------------------------
		//check for matching cols
		for(int i = 0; i < numRows; i++ ) {
			for (int j = 0; j < numCols -2; j++) {
				//grab the colors of first three pieces
				Color col1 = pieces[j,i].GetComponent<pieceController>().getColor ();
				Color col2 = pieces[j+1,i].GetComponent<pieceController>().getColor ();
				Color col3 = pieces[j+2,i].GetComponent<pieceController>().getColor ();

				//lets see if we have a match
				if (col1 == col2 && col1 == col3) {
					//we have a match
					returnVal = true;
					//since we have a match, lets put those pieces in our list of matching pieces
					matchedPieces.Add (pieces[j,i]);
					matchedPieces.Add (pieces[j+1,i]);
					matchedPieces.Add (pieces[j+2,i]);
					//lets see if we can find some more in this row
					int offset = j+3;  //get ready to shift to past the matching set
					j = j+2;  //shift j over to the last piece in the matching set
					while (offset < numCols) {
						col2 = pieces[offset,i].GetComponent<pieceController>().getColor ();
						if (col1 == col2) {
							//we have found another match, add it to our set and increase offset
							matchedPieces.Add (pieces[offset,i]);
							j = offset;
							offset++;	
						}//end of color check
						else {
							//well darn, they did not match
							break;
						}//end of else
					}//end of while loop
				}//end of three color check
			}//end of j
		}//end of i
		//done checking for matching rows
		//--------------------------
		
		return returnVal;
	}//end of isMatching
	
	//method to swap the colors of the matching pieces
	void swapColors() {
		//dump our list into an array
		GameObject[] tempPieces = new GameObject[matchedPieces.Count];
		matchedPieces.CopyTo(tempPieces);
		//spin through this array and generate new random colors
		print("-------------------" + matchedPieces.Count + " MATCHES FOUND--------------");
		for (int i = 0; i < matchedPieces.Count; i++) {
			print(tempPieces[i].GetComponent<pieceController>().myGridX + "," + tempPieces[i].GetComponent<pieceController>().myGridY + " --- " + tempPieces[i].GetComponent<pieceController>().getColor());
		}
		for (int i = 0; i < matchedPieces.Count; i++) {
			tempPieces[i].GetComponent<pieceController>().setColor(pieceColors[Random.Range (0,6)]);
		}//end of for loop
		print("-------------------------------------------------");
	}//end of swapColors
	
	//method to switch the selected pieces
	void switchPieces() {
		//store the destination for each piece....but only do this the first time through
		if (!selectedPieces[0].GetComponent<pieceController>().isMoving &&
		    !selectedPieces[1].GetComponent<pieceController>().isMoving) {
			//set the values of the destinations
			selectedPieces[0].GetComponent<pieceController>().myDestination = selectedPieces[1].transform.position;
			selectedPieces[1].GetComponent<pieceController>().myDestination = selectedPieces[0].transform.position;

			//switch the toggles to true so that we will not come in here again
			selectedPieces[0].GetComponent<pieceController>().isMoving = true;
			selectedPieces[1].GetComponent<pieceController>().isMoving = true;

		}//end of initial check

		//OK now that they know where to go get them moving along and keep going until arrived
		selectedPieces [0].transform.position = Vector3.MoveTowards (selectedPieces [0].transform.position,
		                                                           selectedPieces [0].GetComponent<pieceController> ().myDestination,
		                                                           1f * Time.deltaTime);
		selectedPieces [1].transform.position = Vector3.MoveTowards (selectedPieces [1].transform.position,
		                                                             selectedPieces [1].GetComponent<pieceController> ().myDestination,
		                                                             1f * Time.deltaTime);

		//lets see if they have gotten where they are going yet
		if (selectedPieces[0].GetComponent<pieceController>().isArrived () &&
		       selectedPieces[1].GetComponent<pieceController>().isArrived ()) {
			//selectedPieces [0].GetComponent<pieceController> ().isMoving = false;
			//selectedPieces [1].GetComponent<pieceController> ().isMoving = false;
			//final little issue, we need to switch the pieces in the actual array so are matcher works correctly
			//first lets grab the array locations
			int i1 = selectedPieces [0].GetComponent<pieceController> ().myGridX;
			int j1 = selectedPieces [0].GetComponent<pieceController> ().myGridY;
			int i2 = selectedPieces [1].GetComponent<pieceController> ().myGridX;
			int j2 = selectedPieces [1].GetComponent<pieceController> ().myGridY;
			//now switch their assignments in the array
			pieces [i1, j1] = selectedPieces [1];
			pieces [i2, j2] = selectedPieces [0];
			//and now let the pieces know where their new location is
			selectedPieces [0].GetComponent<pieceController> ().myGridX = i2;
			selectedPieces [0].GetComponent<pieceController> ().myGridY = j2;
			selectedPieces [1].GetComponent<pieceController> ().myGridX = i1;
			selectedPieces [1].GetComponent<pieceController> ().myGridY = j1;
		}//end check to see if we have arrived yet
	}//end of swapColors

	//method to return the selected pieces when no match was found
	void returnPieces() {
		//we will utilize our original color swapping code here
		//grab the colors of the selected pieces
		Color color0 = selectedPieces[0].GetComponent<pieceController>().getColor ();
		Color color1 = selectedPieces[1].GetComponent<pieceController>().getColor ();
		
		//swap the colors out
		selectedPieces[0].GetComponent<pieceController>().setColor(color1);
		selectedPieces[1].GetComponent<pieceController>().setColor(color0);
	}//end of returnPieces
	
	//method to update the array of selected pieces
	public void selected(GameObject piece, bool addPiece) {
		//are we adding or removing a game piece
		if (addPiece) {
			//we are going to add a game piece
			selectedPieces[numSelected] = piece;
			numSelected++;
		}
		else {
			//we are going to remove a game piece
			numSelected--;
			selectedPieces[numSelected] = null;
		}
	}//end of selected
}//end of game controller
