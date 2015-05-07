using UnityEngine;
using System.Collections;

public class Units : MonoBehaviour {
	//References the infantry prefab.
	public GameObject infantryPrefab;
	//References the mech prefab.
	public GameObject mechPrefab;
	//References the recon prefab.
	public GameObject reconPrefab;
	//References the tank prefab.
	public GameObject tankPrefab;
	//References the heavy tank prefab.
	public GameObject heavyTankPrefab;
	//References the artillery prefab.
	public GameObject artilleryPrefab;
	//References the missile launcher prefab.
	public GameObject missileLauncherPrefab;


	
	public static Units instance = null;
	
	/*
	 * Called by Unity before Start().
	 */
	void Awake () {
		if (instance == null) {
			instance = this;
		}
		
		DontDestroyOnLoad (gameObject);
	}

	public GameObject getPrefabByString(string type){
		GameObject result = null;
		switch(type){
		case "Infantry":
			result = infantryPrefab;
			break;
		case "Mech":
			result = mechPrefab;
			break;
		case "Recon":
			result = reconPrefab;
			break;
		case "Tank":
			result = tankPrefab;
			break;
		case "Heavy Tank":
			result = heavyTankPrefab;
			break;
		case "Artillery":
			result = artilleryPrefab;
			break;
		case "Missile Launcher":
			result = missileLauncherPrefab;
			break;
		default:
			break;
		}
		return result;
	}

}
