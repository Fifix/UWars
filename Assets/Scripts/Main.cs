using UnityEngine;
using System.Collections;
using System.IO;

public class Main : MonoBehaviour {

	public static Main instance = null;

	public GameObject gridManager;

	public GameObject terrainTiles;

	public GameObject units;

	public GameObject gridUITiles;

	public GameObject tileStatsUI;
	public GameObject tileType;
	public GameObject defenseRating;

	public GameObject unitStatsUI;
	public GameObject unitType;
	public GameObject unitHP;

	public GameObject ordersUI;
	public GameObject waitButton;
	public GameObject attackButton;

	public GameObject dmgCalculationUI;
	public GameObject dmgToDefender;
	public GameObject dmgToAttacker;

	public GameObject currentPlayerText;
	public GameObject endTurnButton;

	void Awake () {
		instance = this;

		if(TerrainTiles.instance == null){
			Instantiate (terrainTiles);
		}

		if(Units.instance == null){
			Instantiate (units);
		}

		if(GridUITiles.instance == null){
			Instantiate (gridUITiles);
		}

		if (GridManager.instance == null) {
			Instantiate (gridManager);

			StreamReader reader = new StreamReader(Application.dataPath + "/Resources/" + "MyFirstMap.json");
			string mapData = reader.ReadToEnd ();
			reader.Close ();
			GridManager.instance.BoardSetup(mapData);
		}
	}

	public void OnAttackCommandClick() {
		Debug.Log ("ONATKCMDCLICK!");
		GridManager.instance.onAttackCommandClick();
	}

	public void OnWaitCommandClick() {
		Debug.Log ("ONWAITCMDCLICK!");
		GridManager.instance.onWaitCommandClick();
	}

	public void OnEndTurnClick() {
		Debug.Log ("OnEndTurnClick!");
		GridManager.instance.OnEndTurnClick();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
