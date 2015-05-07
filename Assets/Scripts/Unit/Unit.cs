using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

	public enum MovementType {
		Foot,
		Bazooka,
		Tires,
		Caterpillar
	}

	public enum UnitType {
		Infantry, Mech, Recon, Tank, HTank, Artillery, MissLauncher
	}

	public UnitType unitType;

	[Serializable]
	public struct DamageTableStruct {
		public UnitType unitType;
		public int damage;
	}

	public DamageTableStruct[] damageTable;

	public int owner;
	public int hitPoints = 100;

	public bool isAvailable = true;
	public int movementPts;
	public MovementType movementType;
	public bool canMoveAndFire;

	public int minRange;
	public int maxRange;


	
	public void init(int owner){
		this.owner = owner;
	}

	public int calculateDamage(UnitType defender, int attackerHP, int defenderTileDef){
		Debug.Log ("Calculating damage...Attacker type = " + this.unitType + " / defender type = " + defender + " / attacker HP = " + attackerHP + " / defender Tile Def. rating = " + defenderTileDef);
		int baseDamage = 0;
		foreach(DamageTableStruct dmg in damageTable){
			if(dmg.unitType == defender){
				baseDamage = dmg.damage;
				break;
			}
		}
		int damageToDeal = (int) (baseDamage * (1 - 0.10f * defenderTileDef) * (attackerHP / 100.0f));
		//An attack does at least 1 damage.
		if(damageToDeal == 0){
			Debug.Log ("Damage calculated = 0, let's bump it to 1.");
			damageToDeal = 1;
		}
		Debug.Log ("Damage calculated = " + damageToDeal);
		return damageToDeal;

	}

	public int simulateRemainingMovement(TerrainTile tile, int remainingMvtPoints){
		int mvtCost = 9999;
		switch(movementType){
			case MovementType.Foot:
				mvtCost = tile.mvtCostFoot;
			break;
			case MovementType.Bazooka:
				mvtCost = tile.mvtCostBazooka;
			break;
			case MovementType.Tires:
				mvtCost = tile.mvtCostTires;
			break;
			case MovementType.Caterpillar:
				mvtCost = tile.mvtCostCaterpillar;
			break;
			default:
			break;

		}
		return remainingMvtPoints - mvtCost;
	}
		
		// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
