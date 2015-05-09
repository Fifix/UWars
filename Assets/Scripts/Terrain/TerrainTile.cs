using UnityEngine;
using System.Collections.Generic;
using Directions;

public class TerrainTile : MonoBehaviour {

	public string tileType;

	public int mvtCostFoot;
	public int mvtCostBazooka;
	public int mvtCostTires;
	public int mvtCostCaterpillar;
	public int mvtCostAir = 1;

	public int defenseRating;

	public Vector2 gamePosition;

	public Unit unit;

	public Dictionary<Direction, TerrainTile> neighbors;

	public void init(Vector2 gamePosition){
		this.gamePosition = gamePosition;
		setWorldCoord(gamePosition);
	}

	//method used to convert hex grid coordinates to game world coordinates
	private void setWorldCoord(Vector2 gridPos)
	{
		float offset = 0;
		if (gridPos.y % 2 != 0) {
			offset = 0.5f;
		}
		this.transform.position = new Vector3(gridPos.x + offset, gridPos.y * 0.75f, 0f);
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

}
