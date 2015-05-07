using UnityEngine;
using System.Collections;

public class TerrainTiles : MonoBehaviour {
	//References the plains tile prefab.
	public GameObject plainsPrefab;
	//References the mountains tile prefab.
	public GameObject mountainsPrefab;
	//References the river tile prefab.
	public GameObject riverPrefab;
	//References the road tile prefab.
	public GameObject roadPrefab;
	//References the woods tile prefab.
	public GameObject woodsPrefab;

	public static TerrainTiles instance = null;

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
		case "Plains":
			result = plainsPrefab;
			break;
		case "Mountains":
			result = mountainsPrefab;
			break;
		case "River":
			result = riverPrefab;
			break;
		case "Road":
			result = roadPrefab;
			break;
		case "Woods":
			result = woodsPrefab;
			break;
		default:
			break;
		}
		return result;
	}
}
