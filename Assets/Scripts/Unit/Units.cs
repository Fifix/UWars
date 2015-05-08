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
	//References the rocket launcher prefab.
	public GameObject rocketLauncherPrefab;
	//References the anti-air prefab.
	public GameObject antiAirPrefab;
	//References the missile launcher prefab.
	public GameObject missileLauncherPrefab;
	//References the helicopter prefab.
	public GameObject helicopterPrefab;
	//References the fighter prefab.
	public GameObject fighterPrefab;
	//References the bomber prefab.
	public GameObject bomberPrefab;


	
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
		case "Rocket Launcher":
			result = rocketLauncherPrefab;
			break;
		case "Anti Air":
			result = antiAirPrefab;
			break;
		case "Missile Launcher":
			result = missileLauncherPrefab;
			break;
		case "Helicopter":
			result = helicopterPrefab;
			break;
		case "Fighter":
			result = fighterPrefab;
			break;
		case "Bomber":
			result = bomberPrefab;
			break;
		default:
			break;
		}
		return result;
	}

}
