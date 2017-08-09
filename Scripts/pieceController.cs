using UnityEngine;
using System.Collections;

public class pieceController : MonoBehaviour {
	public Vector3 myDestination;	//where this game piece is heading when it moves
	public int myGridX, myGridY;	//store where we are in the grid
	public bool isMoving;	//I have a destination and am off and going
	private Color myColor;	//the color of this game piece
	private bool isSelected;	//track if the game piece has been selected
	private gameController myController;	//reference to the game controller
	private GameObject label;	//used to display the grid position of pieces for testing
	
	// Use this for initialization
	void Start () {
		//initialize the vars
		myController = GameObject.Find ("Main Camera").GetComponent<gameController>();
		label = new GameObject();
		label.AddComponent(typeof(GUIText));
	}//end start

	void Update() {
		label.GetComponent<GUIText>().text = myGridX.ToString () + "," + myGridY.ToString ();
		Camera gameCam = GameObject.Find ("Main Camera").GetComponent<Camera>();
		label.transform.position = gameCam.WorldToViewportPoint(this.transform.position);
	}
	
	//set the color of this game piece
	public void setColor(Color newColor) {
		myColor = newColor;
		this.GetComponent<Renderer>().material.color = myColor;
		GameObject razzle = Instantiate (Resources.Load ("Fireworks", typeof(GameObject))) as GameObject;
		razzle.transform.position = this.transform.position;
	}//end of setColor
	
	//return my current color
	public Color getColor() {
		return myColor;
	}//end of getColor
	
	//respond to the user clicking on me
	public void OnMouseDown() {
		//grab a reference to my halo
		Behaviour myHalo = (Behaviour) GetComponent("Halo");
		//if I am not selected, become selected and show my halo
		if (!isSelected) {
			myHalo.enabled = true;
			isSelected = true;
		}//end of if not selected
		else {
			//otherwise, become unselected and disappear my halo
			myHalo.enabled = false;
			isSelected = false;
			isMoving = false;
		}//end of else
		//call the game controller to updated the selection array
		myController.selected(this.gameObject, isSelected);
	}//end of OnMouseDown

	//return if we have reached our destination or not
	public bool isArrived() {
		bool returnVal = false;
		//lets see if we are at our destination or not
		if (this.transform.position == myDestination) 
			returnVal = true;
		return returnVal;
	}//end of isArrived
}//end pieceController
