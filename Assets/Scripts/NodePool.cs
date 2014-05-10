/**
 * MATCH THREE GAME BOARD
 * 
 * copyright: @igrir
 * 
 * 
 * Class: NodePool.cs
 * Node object pooling handler
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class NodePool: MonoBehaviour
{

	private List<Node> nodeList = new List<Node>();


//	public NodePool (int nodesSum)
//	{
//		prefabricateNodes(nodesSum);
//	}

	void Start(){
	}


	public void unspawnNode(Node node){

//		Node node = nodeGameObject.GetComponent<Node>();
		node.spawned = false;
		GameObject nodeGameObject = node.gameObject;

		nodeGameObject.SetActive(false);

	}

	public Node spawnNode(){
		//get the front
		
		//iterate at list to find the unspawned
		int i = 0;
		bool found = false;
		Node returnObject = null;

		while (i < nodeList.Count && !found) {
			
			Node node = nodeList[i];
			
			//get the unspawned node
			if (node.spawned == false) {

				node.spawned = true;
				node.gameObject.SetActive(true);
				node.boardPos.col = 0;
					
				returnObject = node;

				found = true;
			}else{
				i++;
			}
			
		}


		return returnObject;
		
	}
	
	public void prefabricateNodes(int nodesNum){
		for (int i = 0 ; i < nodesNum; i++) {

			GameObject go = Instantiate(Resources.Load("node"))as GameObject;
			Node node = go.GetComponent<Node>();
			go.name = "node";
//			Node node = go.AddComponent(typeof(Node)) as Node;


//			GameObject go = new GameObject();
//			go.name = "node";
//			Node node = go.AddComponent(typeof(Node)) as Node;
//			
//			SpriteRenderer spriteRenderer = go.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
//
//			CircleCollider2D cc2d = go.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;

			//MASIH DI HARDCOOODEE
//			cc2d.center = new Vector2(1.04f, -1.04f);
//			cc2d.radius = 1;
//
//			node._spriteRenderer = spriteRenderer;
//			node._collider = cc2d;

			node.spawned = false;
			
			go.SetActive(false);
			//				nodeList[i,j] = go;
			nodeList.Add(node);
		}
	}

}