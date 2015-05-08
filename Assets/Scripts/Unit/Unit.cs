using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

	public enum MovementType {
		Foot,
		Bazooka,
		Tires,
		Caterpillar,
		Flying
	}

	public enum UnitType {
		Infantry, Mech, Recon, Tank, HTank, Artillery, RocketLauncher, AntiAir, MissileLauncher,
		Helicopter, Fighter, Bomber
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
	public bool canMoveAndFire; //Indirect fire units can't move AND fire on the same turn.

	public int minRange;
	public int maxRange;


	
	public void init(int owner){
		this.owner = owner;
	}

	/*
	 * Utility method to determine whether a unit can deal damage to another unit, depending on both units' types.
	 */
	public bool canDamageUnit(UnitType defender){
		foreach(DamageTableStruct dmg in damageTable){
			if(dmg.unitType == defender){
				return true;
			}
		}
		return false;
	}

	/*
	 * Utility method to determine how much damage a unit will deal, depending on various parameters.
	 */
	public int calculateDamage(Unit defender, int attackerHP, int defenderTileDef){
		Debug.Log ("Calculating damage...Attacker type = " + this.unitType + " / defender type = " + defender.unitType + " / attacker HP = " + attackerHP + " / defender Tile Def. rating = " + defenderTileDef);
		//Sanity check, just in case...
		if(!canDamageUnit(defender.unitType)){
			return 0;
		}

		int baseDamage = 0;
		foreach(DamageTableStruct dmg in damageTable){
			if(dmg.unitType == defender.unitType){
				baseDamage = dmg.damage;
				break;
			}
		}
		//Special case : Air units can't use tile defenses!
		if(defender.movementType == MovementType.Flying){
			defenderTileDef = 0;
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

	/*
	 * Utility method to quickly calculate how much mvt points a unit will still have after moving to a given tile.
	 */
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
			case MovementType.Flying:
				mvtCost = tile.mvtCostAir;
			break;
			default:
				Debug.LogWarning("Unit.simulateRemainingMovement : A movement type isn't tied to a tile movement cost!");
			break;

		}
		return remainingMvtPoints - mvtCost;
	}

}
