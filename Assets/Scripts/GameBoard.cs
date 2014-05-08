/**
 * MATCH THREE GAME BOARD
 * 
 * copyright: @igrir
 * 
 * 
 * Class: GameBoard.cs
 * Main class of game board itself
 * 
 * 
 * Control:
 * - tap first node, then tap second node to move
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {

	public NodeType[] nodeType;
	
	private bool isPlay;

	public int boardRow;
	public int boardCol;

	public float nodeXScale;
	public float nodeYScale;
	public float nodeWidth;
	public float nodeHeight;
	public float minimumOccurence;

	public Node[,] nodeArr;

	private float time;
	private float startTime;

	private Transform _transform;

	NodePool nodePool;

	List<BoardPos> occurencePositions =  new List<BoardPos>();

	List<Node> occuredNodesList = new List<Node>();

	[HideInInspector]
	public List<NodeCouple> availablePositions = new List<NodeCouple>();
	
	private BoardPos firstPos;
	private BoardPos secondPos;
	bool firstTap;

	bool firstGameTapped = false;
	bool swapBacked = false;

	// number of node needed in column
	// this array needed when the board spawn new node
	private int[] numNodeInCol;

	public delegate void OnFinishAnimate();
	OnFinishAnimate onFinishAnimate;

	bool moving = false;

	public GameObject hintPointer1;
	public GameObject hintPointer2;

	private bool hitted = false;	//status player make a score

	private bool firstHit = true;

	public int combo = 0;
	public float score = 0;

	private bool didCombo = false;

	// Use this for initialization
	void Start () {
		
		_transform = GetComponent<Transform>();
		nodePool = new NodePool(boardRow * boardCol);

		firstPos = new BoardPos();
		secondPos = new BoardPos();

		init ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			if (hit.collider != null) {
				if(hit.collider.name == "node"){


					//first in game tap check
					if (!firstGameTapped) {
						firstGameTapped = true;
					}

					Node node = hit.collider.GetComponent<Node>();

					if (firstTap) {
						firstPos = node.boardPos;

						firstTap = false;

						swapBacked = false;
						hitted = false;
						didCombo = false;

					}else{

						//check it's right to select
						int distanceCol = Mathf.Abs(node.boardPos.col-firstPos.col);
						int distanceRow = Mathf.Abs(node.boardPos.row-firstPos.row);

						if (  (distanceCol == 0 && distanceRow == 1)	// 1 unit distance vertically
						    ||(distanceCol == 1 && distanceRow == 0)	// 1 unit distance horizontally
						    ) {
							secondPos = node.boardPos;
							
							swapNode();
							
							firstTap = true;
						}



					}

				}
			}

			//DEBUG
//			checkAvailablePositions();

		}

		animate();


	}

	public void init(){

		firstHit = true;
		didCombo = false;
		combo = 0;

		nodeArr = new Node[boardRow, boardCol];

		swapBacked = false;
		firstGameTapped = false;

		numNodeInCol = new int[boardCol];
		zeroNumNodeInCol();

		initNodesBoard();
		randomizeBoard();

		checkBoard();

		firstTap = true;
	}

	public void zeroNumNodeInCol(){
		for (int i = 0; i < boardCol; i++) {
			numNodeInCol[i] = 0;
		}
	}

	public void checkBoard(){

		hitted = false;

		zeroNumNodeInCol();

		//check vertically
		checkOccurence(true);
		//check horizontally
		checkOccurence(false);
		
		unaccessTypeIdInOccuredNodesList();
		
		reposition();
		unspawnNodesInOccuredNodesList();
		countEmptyNode();
		spawnNodesOnTop();


		doAnimate();


		if (firstHit && hitted) {
			firstHit = false;
			combo = 0;
			Debug.Log("first hit");
		}

		if (!firstHit && hitted) {
			Debug.Log ("Combo");

			combo++;

			didCombo = true;
		}

		if (!firstHit && !hitted) {
			firstHit = true;
			Debug.Log("COMBO BREAK");
		}

		if (!swapBacked && !didCombo) {
			//swap it back
			swapNode();
			swapBacked = true;
		}


	}


	// gems down animation
	private void doAnimate(){

		startTime = Time.time;
		moving = true;

	}

	private void animate(){

		time = (Time.time - startTime)*4;

		if (moving) {

			for (int i = 0; i < boardRow; i++) {
				for (int j = 0; j < boardCol; j++) {
					Node node = nodeArr[i,j];
					
					node.transform.position = Vector3.Lerp(node.startPosition, node.endPosition, time);

				}
			}
		}

		if (time >= 1 && moving) {
			moving = false;

			//set end position
			for (int i = 0; i < boardRow; i++) {
				for (int j = 0; j < boardCol; j++) {
					Node node = nodeArr[i,j];
					
					node.startPosition = node.endPosition;
					
				}
			}



			if (onFinishAnimate != null) {
				onFinishAnimate();
				
				//end doing check board
				onFinishAnimate -= checkBoard;
				
				

			}


			checkAvailablePositions();
			setActiveHint(false);


		}
	}

	private void setActiveHint(bool enable){
		hintPointer1.SetActive(enable);
		hintPointer2.SetActive(enable);
	}



	// gems swap when selected animation
	private void swapNode(){

		Node node1 = nodeArr[firstPos.row, firstPos.col];
		Node node2 = nodeArr[secondPos.row, secondPos.col];

		Vector3 pos1 = node1.transform.position;
		Vector3 pos2 = node2.transform.position;

		node1.endPosition = pos2;
		node2.endPosition = pos1;

		BoardPos bpTmp = node1.boardPos;
		node1.boardPos = node2.boardPos;
		node2.boardPos = bpTmp;

		Node tmp = nodeArr[firstPos.row, firstPos.col];
		nodeArr[firstPos.row, firstPos.col] = nodeArr[secondPos.row, secondPos.col];
		nodeArr[secondPos.row, secondPos.col] = tmp;

		onFinishAnimate += checkBoard;

		doAnimate();


	}

	private void swapNodePos(Node node1, Node node2){
		BoardPos bpTmp = node1.boardPos;
		node1.boardPos = node2.boardPos;
		node2.boardPos = bpTmp;
	}

	public void play(){
	}

	public void stop(){
	}
	
	public void unspawnNodesInOccuredNodesList(){

		for (int i = 0; i < occuredNodesList.Count; i++) {

			nodePool.unspawnNode(occuredNodesList[i]);

		}
		occuredNodesList.Clear();

	}

	public void unaccessTypeIdInOccuredNodesList(){
		for (int i = 0; i < occuredNodesList.Count; i++) {

			Node node = occuredNodesList[i];
			node.typeId = -1;

		}

	}

	private void countEmptyNode(){
		for (int i = 0; i < boardRow; i++) {
			//			string yeah = "";
			for (int j = 0; j < boardCol; j++) {
				if(nodeArr[i,j].typeId == -1){
					//count this column have empty node
					numNodeInCol[j] += 1;
				}
			}
		}

	}

	/** 
	 * spawn new node on top of board
	 */
	public void spawnNodesOnTop(){

		bool spawned = false;

		for (int i = 0 ; i < boardCol; i++) {

			int numNodes = numNodeInCol[i];

			for (int j = 0 ; j < numNodes; j++) {
				Node node = nodePool.spawnNode();

				if (node != null) {
					int nodeTypeId = Random.Range (0, nodeType.Length);
					node.typeId = nodeTypeId;				
					node.setImage(nodeType[nodeTypeId].image);
					node.score = nodeType[nodeTypeId].score;
					
					if (node != null) {

						nodeArr[j,i] = node;

						node.gameObject.transform.parent = _transform;
						
						node.gameObject.transform.localScale = new Vector3(nodeXScale, nodeYScale);
						node.gameObject.transform.localPosition = new Vector3(i*(nodeXScale+nodeWidth),
						                                                       (numNodes-j)*(nodeYScale+nodeHeight));
						
						// prepare for animation
						node.startPosition = node.gameObject.transform.position;
						node.endPosition = new Vector3(i*(nodeXScale+nodeWidth)+transform.localPosition.x,
						                               -j*(nodeYScale+nodeHeight)+transform.localPosition.y);

						spawned = true;

					}
				}else{
					Debug.Log ("NGGAK MENEMUKAN NODE!");
				}
			}
		}

		reindex();

		if(spawned){
			onFinishAnimate += checkBoard;
		}

	}
	
	public void reposition(){
		for (int i = 0; i < boardCol; i++) {
			for (int j = boardRow-1; j > 0; j--) {

				// found empty node
				if(nodeArr[j,i].typeId == -1){


					// Find enabled node on top of it
					int jj = j;
					bool found = false;
					while (jj >= 0 && !found) {

						if (nodeArr[jj,i].typeId != -1) {

							found = true;

							Vector3 targetNodePos = new Vector3(nodeArr[j,i].transform.position.x,
							                                nodeArr[j,i].transform.position.y,
							                                nodeArr[j,i].transform.position.z);

							// prepare for animation
							nodeArr[jj, i].startPosition = nodeArr[jj, i].transform.position;
							nodeArr[jj, i].endPosition = targetNodePos;

							nodeArr[j,i].transform.position = nodeArr[jj,i].transform.position;

							// swap nodes
							Node tmp = nodeArr[j,i];
							nodeArr[j,i] = nodeArr[jj,i];
							nodeArr[jj,i] = tmp;

						}else{
							jj--;
						}

					}


				}
			}
		}

//		reindex();


	}


	private void reindex(){
		for (int i = 0; i < boardRow; i++) {
			for (int j = 0; j < boardCol; j++) {
				nodeArr[i,j].boardPos.col = j;
				nodeArr[i,j].boardPos.row = i;
			}
		}
	}

	public void checkOccurence(bool vertical){

		int prevCode = -1;
		int currentCode = -1;

		List<BoardPos> currentOccurencePositions = new List<BoardPos>();

		int itI, itJ;
		if (vertical) {
			itI = boardCol;
			itJ = boardRow;
		}else{
			itI = boardRow;
			itJ = boardCol;
		}


		//check vertically
		for (int i = 0 ; i < itI; i++) {
			
			//new row, empty current occurence position
			currentOccurencePositions.Clear();
			
			for (int j = 0 ; j < itJ; j++) {
				
				prevCode = currentCode;

				if (vertical) {
					currentCode = nodeArr[j,i].typeId;	//dibalik
				}else{
					currentCode = nodeArr[i,j].typeId;
				}
				
				BoardPos boardPos = new BoardPos();

				if (vertical) {
					boardPos.row = j;
					boardPos.col = i;
				}else{
					boardPos.row = i;
					boardPos.col = j;
				}

				if ((currentOccurencePositions.Count == 0 || currentCode == prevCode) && currentCode!= -1) {
					currentOccurencePositions.Add(boardPos);								
				}
				
				if ( (vertical && j == boardRow-1)		//di ujung bawah
				    ||(!vertical && j == boardCol-1)	//di ujung kanan
				    || currentCode != prevCode //beda
				    ) {
				
					if (currentOccurencePositions.Count >= minimumOccurence) {
						for (int ii = 0; ii < currentOccurencePositions.Count; ii++) {
							occurencePositions.Add(currentOccurencePositions[ii]);

							Node node = nodeArr[currentOccurencePositions[ii].row,currentOccurencePositions[ii].col];

							occuredNodesList.Add(node);

							//add score
							score += node.score;
							
						}

						//set player hit true
						hitted = true;



					}

					currentOccurencePositions.Clear();

					//back one node if it's different node
					//because it haven't checked yet
					if (currentCode != prevCode && currentCode != -1) {
						j--;
					}

				}
				
			}
		}

	}



	public void initNodesBoard(){
		for (int i = 0 ; i < boardRow; i++) {
			for (int j = 0 ; j < boardCol; j++) {
				Node node = nodePool.spawnNode();

				node.gameObject.transform.parent = _transform;

				node.gameObject.transform.localScale = new Vector3(nodeXScale, nodeYScale);
				node.gameObject.transform.localPosition = new Vector3((j*(nodeXScale+nodeWidth)),
				                                    	 			  -(i*(nodeYScale+nodeHeight)));

				node.startPosition = node.gameObject.transform.position;
				node.endPosition = node.startPosition;

				node.boardPos.col = j;
				node.boardPos.row = i;

//				Debug.Log (node);

				nodeArr[i,j] = node;

			}
		}
	}

	public void randomizeBoard(){
		//randomize node type

		for (int i = 0 ; i < boardRow; i++) {
			for (int j = 0 ; j < boardCol; j++) {
				int nodeTypeId = Random.Range (0, nodeType.Length);

				Node node = nodeArr[i,j];

				node.typeId = nodeTypeId;				
				node.setImage(nodeType[nodeTypeId].image);
				node.score = nodeType[nodeTypeId].score;
				nodeArr[i, j].typeId = nodeTypeId;
			}
		}

	}

	private void checkAvailablePositions(){

		// Empty available list for initialization
		availablePositions.Clear();

		//      (0, -1)
		//(-1,0)       (1,0)
		//      (0,  1)

		for (int i = 0; i < boardRow; i++) {
			for (int j = 0; j < boardCol; j++) {

				Node originNode = nodeArr[i,j];

				//check four direction
//				Debug.Log ("CEK ("+i+","+j+")");
				for (int neighbourRow = -1; neighbourRow <= 1; neighbourRow++) {
					for (int neighbourCol = -1; neighbourCol <= 1; neighbourCol++) {

						// ignore these directions
						if (  !(neighbourRow == -1 && neighbourCol == -1)	//nw
						    &&!(neighbourRow == 1 && neighbourCol == -1)  //ne
						    &&!(neighbourRow == 0 && neighbourCol == 0)   //center
						    &&!(neighbourRow == -1 && neighbourCol == 1)  //sw
						    &&!(neighbourRow == 1 && neighbourCol == 1)   //se
						    ) {

							int posRow = i+neighbourRow;
							int posCol = j+neighbourCol;

							Node node;

							//check the starting node
							if (posCol >= 0
							    && posCol < boardCol
							    && posRow >= 0
							    && posRow < boardRow) {

								node = nodeArr[posRow,posCol];

								for (int nNeighbourRow = -1; nNeighbourRow <= 1; nNeighbourRow++) {
									for (int nNeighbourCol = -1; nNeighbourCol <= 1; nNeighbourCol++) {
										
										// ignore these directions
										if (  !(nNeighbourRow == -1 && nNeighbourCol == -1)	//nw
										    &&!(nNeighbourRow == 1 && nNeighbourCol == -1)  //ne
										    &&!(nNeighbourRow == 0 && nNeighbourCol == 0)   //center
										    &&!(nNeighbourRow == -1 && nNeighbourCol == 1)  //sw
										    &&!(nNeighbourRow == 1 && nNeighbourCol == 1)   //se
										    ) {

											// count of neighbour node.
											// why is it 1? Because we start from initial node
											int countNode = 1;
											
											int nPosRow = posRow+nNeighbourRow;
											int nPosCol = posCol+nNeighbourCol;

											// check not checking back to orign 
											if (nPosRow == posRow &&
											    nPosCol == posCol) {
												Debug.Log ("WAH, BALIK!");
												
											}


											Node neighbourNode;
											
											//CHECK THE OCCURENCE
											
											bool same = true;
												
											while(same
											      && (nPosCol >= 0 && nPosCol < boardCol)
											      && (nPosRow >= 0 && nPosRow < boardRow)
											      ){
												
												//check neighbour node
												neighbourNode = nodeArr[nPosRow, nPosCol];

												if (neighbourNode.typeId != originNode.typeId	//don't check anymore if it has same id type 
												    ||neighbourNode == originNode				//or it's the same node as origin. Why check it back?
												    ) {
													same = false;
												}else{
													countNode++;
													nPosCol += nNeighbourCol;
													nPosRow += nNeighbourRow;
													
												}
												
											}											
											
											if (countNode >= minimumOccurence) {
//												Debug.Log ("count node:" + countNode);
												NodeCouple nc = new NodeCouple(originNode, node);
												availablePositions.Add(nc);
											}
										}
									}
								}
							}
						}
					}
				}


			}
		}

		if (availablePositions.Count > 0) {
//			Debug.Log("ADA");

			for (int i = 0;i < availablePositions.Count; i++) {
				BoardPos startPos = availablePositions[i].firstNode.boardPos;
				BoardPos endPos = availablePositions[i].secondNode.boardPos;

//				Debug.Log ("("+startPos.row + "," + startPos.col+")"
//				           +"("+endPos.row + "," + endPos.col+")");
			}

		}else{
			Debug.Log("KAGAK ADA");
		}

	}


	public void reset(){
	}

	public void showHint(){

		//check there's available position
		if (availablePositions.Count > 0) {

			setActiveHint(true);

			//random hint position
			int rand = Random.Range(0, availablePositions.Count-1);

			NodeCouple randomNode = availablePositions[rand];
			Vector3 randomNodeFirstPosition = randomNode.firstNode.transform.position;
			Vector3 randomNodeSecondPosition = randomNode.secondNode.transform.position;

			Vector3 hintPosition1 = new Vector3(randomNodeFirstPosition.x + (nodeWidth/2),
			                                   randomNodeFirstPosition.y - (nodeHeight/2),
			                                   hintPointer1.transform.position.z);

			Vector3 hintPosition2 = new Vector3(randomNodeSecondPosition.x + (nodeWidth/2),
			                                    randomNodeSecondPosition.y - (nodeHeight/2),
			                                    hintPointer2.transform.position.z);


			hintPointer1.transform.position = hintPosition1;
			hintPointer2.transform.position = hintPosition2;

		}
	}


}
