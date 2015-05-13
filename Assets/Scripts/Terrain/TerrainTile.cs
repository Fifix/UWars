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

	public bool canBeCaptured = false;
	public int owner = 0;
	public int capturePoints = 20;

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

	public void setTileOwner(int owner){
		this.owner = owner;
		SpriteRenderer unitSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		if(owner == 0){ //Neutral.
			unitSpriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
		}
		else if(owner == 1){
			unitSpriteRenderer.color = new Color(1f, 0, 0);
		}
		else if(owner == 2){
			unitSpriteRenderer.color = new Color(0, 0, 1f);
		} else{
			Debug.LogError("setTileOwner must implement new players!");
		}
	}
	
	/*
	 * Utility method to try to capture the tile (only for cities, factories, airports).
	 */
	public void captureTile(int captureDamage, int capturingUnitOwner){
		capturePoints = capturePoints - captureDamage;
		if(capturePoints <= 0){
			setTileOwner(capturingUnitOwner);
			resetCaptureStatus();
		}
	}

	/*
	 * Utility method to reset the capture (capturing unit is dead, or moved out of the tile)
	 */
	public void resetCaptureStatus(){
		capturePoints = 20;
	}

}
