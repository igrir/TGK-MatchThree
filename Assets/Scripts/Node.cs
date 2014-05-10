/**
 * MATCH THREE GAME BOARD
 * 
 * copyright: @igrir
 * 
 * 
 * Class: Node.cs
 * Component of node
 * 
 */


using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

//	public Sprite image;

	public float score;
//	public SpriteRenderer _spriteRenderer;

	public Vector3 endPosition;
	public Vector3 startPosition;

	public bool spawned;

	[SerializeField]
	public BoardPos boardPos = new BoardPos();

	private Sprite currentSprite;

	public int typeId;	//id for the node type

	private Transform _transform;
	public Collider2D _collider;
	

	// Use this for initialization
	void Start () {
		_transform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void setImage(Sprite sprite){
		currentSprite = sprite;
		GetComponent<SpriteRenderer>().sprite = sprite;
	}
	
	public void setColor(float r, float g, float b, float a){
		GetComponent<SpriteRenderer>().color = new Color(r, g, b, a);
	}

	public void setAlpha(float a){
		Color color = GetComponent<SpriteRenderer>().color;
		GetComponent<SpriteRenderer>().color = new Color(color.r,
		                                                 color.g,
		                                                 color.b, a);
	}


}
