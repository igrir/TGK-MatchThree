/**
 * MATCH THREE GAME BOARD
 * 
 * copyright: @igrir
 * 
 * 
 * Class: TestGUI.cs
 * Testing using GUI. D'oh :p
 * 
 */

using UnityEngine;
using System.Collections;

public class TestGUI : MonoBehaviour {

	
	public GameObject gameBoard;

	private GameBoard gameBoardScript;

	private bool no_move = false;

	void OnGUI(){

		if (gameBoardScript.availablePositions.Count <= 0) {
			GUI.TextArea(new Rect(30,30,100,50),"Gak ada jalan lagi");
		}

		if (GUI.Button(new Rect(0, 0, 100, 30),"Show hint")) {
			gameBoardScript.showHint();
		}

		if (GUI.Button(new Rect(100, 0, 100, 30),"Reset")) {
			gameBoardScript.reset();
		}

		GUI.TextArea(new Rect(0, 30, 100, 30), "score:"+gameBoardScript.score.ToString());
		GUI.TextArea(new Rect(Screen.width-100, 30, 100, 30), "combo:"+gameBoardScript.combo.ToString());
	}

	// Use this for initialization
	void Start () {
	
		gameBoardScript = gameBoard.GetComponent<GameBoard>();

	}
	
	// Update is called once per frame
	void Update () {
	



	}
}
