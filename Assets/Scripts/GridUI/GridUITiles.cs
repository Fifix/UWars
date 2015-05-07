using UnityEngine;
using System.Collections;

public class GridUITiles : MonoBehaviour {
	//References the prefab showing that a unit can move to that position.
	public GameObject mvtTileUIPrefab;
	//References the prefab showing that a unit can fire on that position.
	public GameObject rangeTileUIPrefab;

	public static GridUITiles instance = null;

	/*
	 * Called by Unity before Start().
	 */
	void Awake () {
		if (instance == null) {
			instance = this;
		}
		
		DontDestroyOnLoad (gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
