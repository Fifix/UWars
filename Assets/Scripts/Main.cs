using UnityEngine;
using System.Collections;
using System.IO;

public class Main : MonoBehaviour {

	public static Main instance = null;

	[Header("Singletons")]
	public GameObject gridManager;
	public GameObject terrainTiles;
	public GameObject units;
	public GameObject gridUITiles;

	[Header("Tile stats UI")]
	public GameObject tileStatsUI;
	public GameObject tileType;
	public GameObject defenseRating;

	[Header("Capture state UI")]
	public GameObject captureStateUI;
	public GameObject tileOwner;
	public GameObject capturePoints;

	[Header("Unit stats UI")]
	public GameObject unitStatsUI;
	public GameObject unitType;
	public GameObject unitHP;

	[Header("Orders UI")]
	public GameObject ordersUI;
	public GameObject waitButton;
	public GameObject attackButton;
	public GameObject captureButton;

	[Header("Damage calculation UI")]
	public GameObject dmgCalculationUI;
	public GameObject dmgToDefender;
	public GameObject dmgToAttacker;

	[Header("Player turn UI")]
	public GameObject playerTurnUI;
	public GameObject currentPlayerText;
	public GameObject currentMoneyText;
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

	public void OnCaptureCommandClick() {
		Debug.Log ("OnCaptureCommandClick!");
		GridManager.instance.OnCaptureCommandClick();
	}

	public void OnAttackCommandClick() {
		Debug.Log ("OnAttackCommandClick!");
		GridManager.instance.onAttackCommandClick();
	}

	public void OnWaitCommandClick() {
		Debug.Log ("OnWaitCommandClick!");
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
