using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using SimpleJSON;
using Directions;


namespace Directions{
	public enum Direction {
		E, NE, NW, W, SW, SE
	}
}

public class GridManager : MonoBehaviour {

	public static GridManager instance = null;

	public int columns;
	public int rows;
	public int currentPlayer = 1;
	private int nbPlayers;

	//Allows to organize tiles in the scene.
	private static Transform boardHolder;
	//Allows to organize units in the scene.
	private static Transform unitsHolder;
	//Allows to organize Grid UI elements in the scene.
	private static Transform gridUIHolder;
	//Contains a representation of the board.
	private TerrainTile[,] board;

	public enum CurrentState {
		None, SelectedUnit, MovedUnit, TargetingEnemyUnit
	}

	private CurrentState currentState = CurrentState.None;
	private TerrainTile originTile = null;
	private TerrainTile destinationTile = null;
	private List<GameObject> gridUIMoveInstances;
	private List<GameObject> gridUIRangeInstances;
	private List<GameObject> availableTargetsGridUIInstances;

	/*
	 * Called by Unity before Start().
	 */
	void Awake () {
		if (instance == null) {
			boardHolder = new GameObject ("Board").transform;
			unitsHolder = new GameObject ("Units").transform;
			gridUIHolder = new GameObject ("GridUI").transform;
			Main.instance.ordersUI.SetActive (false);
			instance = this;
		}

		DontDestroyOnLoad (gameObject);
	}

	/*
	 * Setups the board with the given JSON-formated string.
	 */
	public void BoardSetup (String mapJson){

		JSONNode json = JSON.Parse (mapJson);
		string mapName = json["name"];
		nbPlayers = json["players"].AsInt;

		rows = json["map"]["rows"].AsInt;
		columns = json["map"]["columns"].AsInt;

		board = new TerrainTile[columns, rows];

		for(int y = 0; y < rows; y++){
			for(int x = 0; x < columns; x++){

				string tileType = json["map"]["tiles"][y*columns + x]["type"];
				GameObject tileToInstantiate = TerrainTiles.instance.getPrefabByString(tileType);
				if(tileToInstantiate != null){
					GameObject instance = Instantiate (tileToInstantiate) as GameObject;
					TerrainTile terrainTile = instance.GetComponent<TerrainTile>();
					terrainTile.init(new Vector2(x, y));
					board[x, y] = terrainTile;
					instance.transform.SetParent(boardHolder);
					if(tileToInstantiate.Equals(TerrainTiles.instance.cityPrefab) 
					|| tileToInstantiate.Equals (TerrainTiles.instance.factoryPrefab)
					|| tileToInstantiate.Equals (TerrainTiles.instance.airportPrefab)){
						int tileOwner = json["map"]["tiles"][y*columns + x]["tileOwner"].AsInt;
						terrainTile.setTileOwner(tileOwner);
					}
				} else {
					Debug.LogError("Woops, map seems wrong!");
				}

				string unitType = json["map"]["tiles"][y*columns + x]["unit"];
				GameObject unitToInstantiate = Units.instance.getPrefabByString(unitType);
				if(unitToInstantiate != null){
					int unitOwner = json["map"]["tiles"][y*columns + x]["unitOwner"].AsInt;
					createUnit(x, y, unitToInstantiate, unitOwner, true);
				}

			}
		}

		buildNeighbors();
	}

	/*
	 * Builds each tile's neighbors' list.
	 */
	private void buildNeighbors(){
		for(int y = 0; y < rows; y++){
			for(int x = 0; x < columns; x++){
				Dictionary<Direction, TerrainTile> neighbors = new Dictionary<Direction, TerrainTile>();
				foreach(Direction dir in Enum.GetValues (typeof(Direction))){
					Vector2 reference = getNeighborPosition((int) x, (int) y, dir);
					if(isPositionValid(reference)){
						neighbors.Add (dir, board[(int) reference.x, (int) reference.y]);
					}
				}

				board[x, y].neighbors = neighbors;
			}
		}
	}

	/*
	 * Utility method to see if a tile position is valid (Not out of bounds)
	 */
	public bool isPositionValid(Vector2 pos){
		return pos.x >= 0 && pos.x < columns && pos.y >= 0 && pos.y < rows;
	}

	/*
	 * Obtains the X/Y position of a neighboring tile.
	 */
	public Vector2 getNeighborPosition(int origX, int origY, Direction dir){
		int neighborX = -1;
		int neighborY = -1;
		if(origY%2 == 0){
			switch(dir){
			case Direction.W:
				neighborX = origX - 1;
				neighborY = origY;
				break;
			case Direction.SW:
				neighborX = origX - 1;
				neighborY = origY - 1;
				break;
			case Direction.SE:
				neighborX = origX;
				neighborY = origY - 1;
				break;
			case Direction.E:
				neighborX = origX + 1;
				neighborY = origY;
				break;
			case Direction.NE:
				neighborX = origX ;
				neighborY = origY + 1;
				break;
			case Direction.NW:
				neighborX = origX - 1;
				neighborY = origY + 1;
				break;
			default:
				break;
			}
		}
		else{
			switch(dir){
			case Direction.W:
				neighborX = origX - 1;
				neighborY = origY;
				break;
			case Direction.SW:
				neighborX = origX;
				neighborY = origY - 1;
				break;
			case Direction.SE:
				neighborX = origX + 1;
				neighborY = origY - 1;
				break;
			case Direction.E:
				neighborX = origX + 1;
				neighborY = origY;
				break;
			case Direction.NE:
				neighborX = origX + 1;
				neighborY = origY + 1;
				break;
			case Direction.NW:
				neighborX = origX;
				neighborY = origY + 1;
				break;
			default:
				break;
			}
		}

		return new Vector2(neighborX, neighborY);
	}

	/*
	 * Obtains tiles at X tiles distance.
	 */
	public List<TerrainTile> cube_ring(TerrainTile orig, int radius){
		List<TerrainTile> results = new List<TerrainTile>();
		Vector2 reference = orig.gamePosition;
		for(int i = 0; i < radius; i++){
			reference = getNeighborPosition((int) reference.x, (int) reference.y, Direction.SW);
		}
		if(isPositionValid(reference)){
			results.Add (board[(int) reference.x, (int) reference.y]);
		}

		foreach(Direction dir in Enum.GetValues (typeof(Direction))){
			for(int j = 0; j < radius; j++){
				reference = getNeighborPosition((int) reference.x, (int) reference.y, dir);
				if(isPositionValid(reference)){
					results.Add (board[(int) reference.x, (int) reference.y]);
				}
			}

		}

		return results;
	}

	/*
	 * Gets the tiles a unit can move to.
	 */
	public List<TerrainTile> getTilesInMovementRange(TerrainTile orig){
		Dictionary<TerrainTile, int> bestPathForTile = new Dictionary<TerrainTile, int> ();
		if(orig != null && orig.unit != null){
			bestPathForTile.Add (orig, orig.unit.movementPts);
			getTilesInMovementRange(orig, orig, orig.unit.movementPts, bestPathForTile);
		}
		List<TerrainTile> results = new List<TerrainTile>();
		foreach(KeyValuePair<TerrainTile, int> tileDictionaryElt in bestPathForTile){
			results.Add (tileDictionaryElt.Key);
		}
		return results;
	}

	/*
	 * Recursive function to build the tile list of possible tiles a unit can move to.
	 */
	public void getTilesInMovementRange(TerrainTile currTile, TerrainTile unitTile, int remainingMovement, Dictionary<TerrainTile, int> bestPathForTile){
		foreach(KeyValuePair<Direction, TerrainTile> tileDictionaryElt in currTile.neighbors){
			int simulatedRemainingMovement = unitTile.unit.simulateRemainingMovement(tileDictionaryElt.Value, remainingMovement);

			//Do we have enough movement points to keep going? Is the tile empty or has a friendly unit on it?
			if(simulatedRemainingMovement >= 0 && (tileDictionaryElt.Value.unit == null || tileDictionaryElt.Value.unit.owner == unitTile.unit.owner)){

				//Now, let's see if we already did find that tile as a viable destination.
				int bestRemainingMovement;
				bool foundElement = bestPathForTile.TryGetValue (tileDictionaryElt.Value, out bestRemainingMovement);

				if(foundElement){
					//It seems we've already found it before! Is it a better solution than before?
					if(bestRemainingMovement < simulatedRemainingMovement){
						//Yup! Let's update the dictionary, and recalculate everything else from there!
						if(tileDictionaryElt.Value.unit == null){
							bestPathForTile.Remove (tileDictionaryElt.Value);
							bestPathForTile.Add (tileDictionaryElt.Value, simulatedRemainingMovement);
						}
						getTilesInMovementRange(tileDictionaryElt.Value, unitTile, simulatedRemainingMovement, bestPathForTile);
					}
					else{
						//We're not enhancing the solution, so we stop right there.
					}
				}
				else{
					//Nope, the element isn't in the dictionary. Let's add it, and check its neighbors to see if we can add more tiles.
					if(tileDictionaryElt.Value.unit == null){
						bestPathForTile.Add (tileDictionaryElt.Value, simulatedRemainingMovement);
					}
					getTilesInMovementRange(tileDictionaryElt.Value, unitTile, simulatedRemainingMovement, bestPathForTile);
				}
			}
		}
	}
	
	/*
	 * Gets the tiles a unit could be able to fire on.
	 */
	public List<TerrainTile> getTilesInFiringRange(TerrainTile orig){
		List<TerrainTile> results = new List<TerrainTile> ();
		if(orig != null && orig.unit != null){
			//Two cases to handle :
			//- Indirect fire unit : Can't move, but can hit X to Y tiles away.
			//- Direct fire unit : Can move, can hit units on adjacent tiles. Easy to handle, as we know where a unit can go.
			if(orig.unit.canMoveAndFire){
				List<TerrainTile> recursiveResults = getTilesInMovementRange(orig);
				foreach(TerrainTile recursiveResult in recursiveResults){
					TerrainTile toFindRecursive = results.Find ((TerrainTile obj) => obj.gamePosition.Equals(recursiveResult.gamePosition));
					if(toFindRecursive == null){
						results.Add (recursiveResult);
					}
					foreach(KeyValuePair<Direction, TerrainTile> tileDictionaryElt in recursiveResult.neighbors){
						TerrainTile toFindNeighbor = results.Find ((TerrainTile obj) => obj.gamePosition.Equals(tileDictionaryElt.Value.gamePosition));
						if(toFindNeighbor == null){
							results.Add (tileDictionaryElt.Value);
						}
					}
				}
			} else {
				for(int i = orig.unit.minRange; i <= orig.unit.maxRange; i++){
					results.AddRange (cube_ring(orig, i));
				}
			}
		}
		return results;
	}

	/*
	 * Utility method to quickly create a unit.
	 */
	public void createUnit(int gridX, int gridY, GameObject unitPrefab, int owner, bool initialState){
		GameObject instance = Instantiate (unitPrefab, board[gridX, gridY].transform.position, Quaternion.identity) as GameObject;
		Unit unitInstance = instance.GetComponent<Unit>();
		unitInstance.init(owner);
		unitInstance.setUnitState(initialState);

		board[gridX, gridY].unit = unitInstance;
		instance.transform.SetParent(unitsHolder);
	}

	/*
	 * Utility method to handle unit damage and death.
	 */
	public void damageUnit(int gridX, int gridY, int damage){
		int hp = board[gridX, gridY].unit.hitPoints;
		hp = hp - damage;
		//Do a random between 0.0 and 10.0
		double rand = Random.value * 10.0;
		//If the unit has 59HP, it has 10% chance to lose another 9HP.
		//If the random doesn't pass, it doesn't lose more HP.
		if(rand > hp % 10){
			hp = hp - hp % 10;
		}

		board[gridX, gridY].unit.setHPText (calculateHPBaseTen(hp));

		board[gridX, gridY].unit.hitPoints = hp;
		if(hp <= 0){
			Destroy(board[gridX, gridY].unit.gameObject);
			board[gridX, gridY].unit = null;
			if(board[gridX, gridY].canBeCaptured){
				board[gridX, gridY].resetCaptureStatus();
			}
		}
	}

	/*
	 *  Converts a value from 1 to 100 to a value from 1 to 10, 1-10 being 1, 11-20 being 2, and so on.
	 *  Used when the game needs to know how many capture points are taken away on a Capture command, 
	 *  or which number it has to show near the unit sprite.
	 */
	public int calculateHPBaseTen(int hpBaseHundred){
		int hpBaseTen = hpBaseHundred/10;
		if(hpBaseHundred%10 != 0){
			hpBaseTen++;
		}
		return hpBaseTen;
	}

	/*
	 * Shows the tiles a unit can move to. Returns a GameObject list, keep it to remove them later when necessary.
	 */
	public List<GameObject> showTilesInMvtRange(int x, int y){
		List<TerrainTile> tilesToDraw = getTilesInMovementRange(board[x,y]);
		List<GameObject> gridUITilesInstances = new List<GameObject>();
		
		foreach(TerrainTile tile in tilesToDraw){
			GameObject toInstantiate = Instantiate(GridUITiles.instance.mvtTileUIPrefab, tile.transform.position, Quaternion.identity) as GameObject;
			gridUITilesInstances.Add(toInstantiate);
			toInstantiate.transform.SetParent(gridUIHolder);
		}

		return gridUITilesInstances;
	}

	/*
	 * Shows the tiles a unit can fire into. Returns a GameObject list, keep it to remove them later when necessary.
	 */
	public List<GameObject> showTilesInFiringRange(int x, int y){
		List<TerrainTile> tilesToDraw = getTilesInFiringRange(board[x,y]);
		List<GameObject> gridUITilesInstances = new List<GameObject>();
		
		foreach(TerrainTile tile in tilesToDraw){
			GameObject toInstantiate = Instantiate(GridUITiles.instance.rangeTileUIPrefab, tile.transform.position, Quaternion.identity) as GameObject;
			gridUITilesInstances.Add(toInstantiate);
			toInstantiate.transform.SetParent(gridUIHolder);
		}
		
		return gridUITilesInstances;
	}

	/*
	 * Utility method to quickly hide previously generated UI tiles.
	 */
	public void hideUITiles(List<GameObject> instances){
		if(instances != null){
			foreach(GameObject instance in instances){
				Destroy(instance);
			}
		}
	}

	/*
	 * Moves a unit from a tile to another.
	 */
	public void moveUnitToTile(TerrainTile origTile, TerrainTile destTile){
		if(!origTile.Equals(destTile)){
			Unit unit = origTile.unit;
			destTile.unit = unit;
			origTile.unit = null;
			
			unit.transform.position = destTile.transform.position;
		}
	}

	private List<TerrainTile> getAvailableTargets(TerrainTile tile){
		List<TerrainTile> results = new List<TerrainTile>();
		if(tile != null && tile.unit != null){
			List<TerrainTile> tilesInFiringRange = new List<TerrainTile>();
			if(tile.unit.canMoveAndFire){
				foreach(KeyValuePair<Direction, TerrainTile> tileDictionaryElt in tile.neighbors){
					tilesInFiringRange.Add (tileDictionaryElt.Value);
				}
			} else{
				tilesInFiringRange = getTilesInFiringRange(tile);
			}
			foreach(TerrainTile availableTile in tilesInFiringRange){
				//To be able to fire on a unit, it must belong to another player, and the attacking unit must be able to damage it.
				if(availableTile.unit != null && availableTile.unit.owner != tile.unit.owner && tile.unit.canDamageUnit(availableTile.unit.unitType)){
					Debug.Log ("Valid tile to fire at at position " + availableTile.gamePosition);
					results.Add(availableTile);
				}
			}
		}
		return results;
	}

	/*
	 * Called when the player clicks on the Capture command.
	 */
	public void OnCaptureCommandClick(){
		destinationTile.captureTile(calculateHPBaseTen(destinationTile.unit.hitPoints), destinationTile.unit.owner);
		finishUnitAction();
	}

	/*
	 * Called when the player clicks on the Attack command.
	 */
	public void onAttackCommandClick(){
		availableTargetsGridUIInstances = new List<GameObject>();
		currentState = CurrentState.TargetingEnemyUnit;
		Main.instance.ordersUI.SetActive(false);
		List<TerrainTile> targets = getAvailableTargets(destinationTile);
		foreach(TerrainTile target in targets){
			GameObject toInstantiate = Instantiate(GridUITiles.instance.rangeTileUIPrefab, target.transform.position, Quaternion.identity) as GameObject;
			availableTargetsGridUIInstances.Add(toInstantiate);
			toInstantiate.transform.SetParent(gridUIHolder);
		}
	}

	/*
	 * Called when the player clicks on the Wait command.
	 */
	public void onWaitCommandClick(){
		finishUnitAction();
	}

	/*
	 * Handles clicks on the End Turn button.
	 */
	public void OnEndTurnClick(){
		foreach(TerrainTile tile in board){
			if(tile.unit != null){
				tile.unit.setUnitState(true);
			}
		}
		currentPlayer = currentPlayer + 1;
		if(currentPlayer > nbPlayers){
			currentPlayer = 1;
		}

		Main.instance.currentPlayerText.GetComponent<Text>().text = "Player " + currentPlayer + " Turn";
	}

	/*
	 * Called when the unit must be de-activated (Wait order, after attacking a unit, etc...)
	 */
	public void finishUnitAction(){
		currentState = CurrentState.None;
		//If attacker is still alive, deactive it.
		if(destinationTile.unit != null){
			destinationTile.unit.setUnitState(false);
		}
		Main.instance.playerTurnUI.SetActive(true);

		//If the origin and destination tiles are different (indicating a movement), reset its capture points to 20.
		if(originTile.canBeCaptured && !originTile.Equals(destinationTile)){
			originTile.resetCaptureStatus();
		}

		originTile = null;
		destinationTile = null;
		hideUITiles(gridUIMoveInstances);
		hideUITiles(availableTargetsGridUIInstances);
		Main.instance.ordersUI.SetActive(false);
	}

	/*
	 *  Called by Unity at each frame.
	 */
	void Update () {

		RaycastHit2D hit = Physics2D.Raycast(Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.collider != null) {
			TerrainTile tile = hit.collider.gameObject.GetComponent<TerrainTile>();
			if(currentState == CurrentState.None){
				Main.instance.dmgCalculationUI.SetActive(false);

				Main.instance.tileStatsUI.SetActive (true);
				Main.instance.tileType.GetComponent<Text>().text = tile.tileType;
				Main.instance.defenseRating.GetComponent<Text>().text = tile.defenseRating.ToString();

				if(tile.unit != null){
					Main.instance.unitStatsUI.SetActive (true);
					Main.instance.unitType.GetComponent<Text>().text = tile.unit.unitType.ToString();
					Main.instance.unitHP.GetComponent<Text>().text = tile.unit.hitPoints.ToString();
				}
				else{
					Main.instance.unitStatsUI.SetActive (false);
				}

				if(tile.canBeCaptured){
					Main.instance.captureStateUI.SetActive (true);
					if(tile.owner == 0){
						Main.instance.tileOwner.GetComponent<Text>().text = "Neutral";
					}
					else{
						Main.instance.tileOwner.GetComponent<Text>().text = "Player " + tile.owner;
					}
					Main.instance.capturePoints.GetComponent<Text>().text = tile.capturePoints.ToString();

				}
				else{
					Main.instance.captureStateUI.SetActive (false);
				}
			} else {
				Main.instance.tileStatsUI.SetActive (false);
				Main.instance.unitStatsUI.SetActive (false);
				Main.instance.captureStateUI.SetActive (false);
				if(currentState == CurrentState.TargetingEnemyUnit){

					if(tile.unit != null){
						//We must show the Damage Calculation UI only if we can fire at that unit...
						GameObject target = availableTargetsGridUIInstances.Find ((GameObject obj) => obj.transform.position.Equals(tile.gameObject.transform.position));
						if(target != null){
							Main.instance.dmgCalculationUI.SetActive(true);

							int simulatedDamageToDefender = destinationTile.unit.calculateDamage(tile.unit, destinationTile.unit.hitPoints, tile.defenseRating);
							//Unit with 97HP, the attacker deals 41 dmg. It will either do 41 dmg or "crit" and deal 47 dmg.
							int simulatedDefRemainingHP = tile.unit.hitPoints - simulatedDamageToDefender;
							if(simulatedDefRemainingHP > 0){
								int maxSimDmgToDefender = simulatedDamageToDefender + simulatedDefRemainingHP % 10;
								if(simulatedDamageToDefender == maxSimDmgToDefender){
									//Show that damage dealt is X. No possibility to "crit".
									Main.instance.dmgToDefender.GetComponent<Text>().text = simulatedDamageToDefender.ToString();
								}
								else{
									//Show that damage dealt is either X or Y ("crit").
									Main.instance.dmgToDefender.GetComponent<Text>().text = simulatedDamageToDefender.ToString() + " or " + maxSimDmgToDefender.ToString();
								}
							}
							else{
								//Show that the defender doesn't stand a single chance.
								Main.instance.dmgToDefender.GetComponent<Text>().text = simulatedDamageToDefender.ToString() + " (SURE KILL)";
							}

							if(simulatedDefRemainingHP > 0){
								//Okay, the defender is still alive. Can it fire back?
								if(tile.unit.canMoveAndFire && destinationTile.unit.canMoveAndFire){
									int simulatedDamageToAttacker = tile.unit.calculateDamage(destinationTile.unit, tile.unit.hitPoints - simulatedDamageToDefender, destinationTile.defenseRating);
									int simulatedAtkRemainingHP = destinationTile.unit.hitPoints - simulatedDamageToAttacker;
									if(simulatedAtkRemainingHP > 0){
										int maxSimDmgToAttacker = simulatedDamageToAttacker + simulatedAtkRemainingHP % 10;
										if(simulatedDamageToAttacker == maxSimDmgToAttacker){
											//Show that the counter-strike deas  X. No possibility to "crit".
											Main.instance.dmgToAttacker.GetComponent<Text>().text = simulatedDamageToAttacker.ToString();
										}
										else{
											//Show that the counter-strike deals either X or Y ("crit").
											Main.instance.dmgToAttacker.GetComponent<Text>().text = simulatedDamageToAttacker.ToString() + " or " + maxSimDmgToAttacker.ToString();
										}
									}
									else{
										//Show that the attacker doesn't stand a single chance.
										Main.instance.dmgToAttacker.GetComponent<Text>().text = simulatedDamageToAttacker.ToString() + " (SURE KILL)";
									}
								}
								else{
									//Show that the defender can't fire back (indirect fire unit, or hit by an indirect fire enemy).
									Main.instance.dmgToAttacker.GetComponent<Text>().text = "0 (NO COUNTER)";
								}
							}
							else{
								//Show that the defender can't fire back, since it dies on that strike.
								Main.instance.dmgToAttacker.GetComponent<Text>().text = "0 (DEAD)";
							}

						}
						else{
							Main.instance.dmgCalculationUI.SetActive(false);
						}
					}
					else{
						Main.instance.dmgCalculationUI.SetActive(false);
					}
				}
				else{
					Main.instance.dmgCalculationUI.SetActive(false);
				}
			}
		} else {
			Main.instance.tileStatsUI.SetActive (false);
			Main.instance.unitStatsUI.SetActive (false);
			Main.instance.dmgCalculationUI.SetActive(false);
			Main.instance.captureStateUI.SetActive (false);
		}

		if(currentState == CurrentState.None){
			if (Input.GetMouseButtonDown(0)){ // if left button pressed...
				Debug.Log ("Left click at CurrentState.None");
				if(hit.collider != null)
				{
					TerrainTile tile = hit.collider.gameObject.GetComponent<TerrainTile>();
					if(tile != null){
						if(tile.unit != null && tile.unit.isAvailable && tile.unit.owner == currentPlayer){
							Debug.Log ("Selected an available unit! Switching to CurrentState.SelectedUnit");
							Main.instance.playerTurnUI.SetActive(false);
							currentState = CurrentState.SelectedUnit;
							originTile = tile;
							hideUITiles(gridUIMoveInstances);
							gridUIMoveInstances = showTilesInMvtRange((int) tile.gamePosition.x, (int) tile.gamePosition.y);
						} else {
							//TODO : Implement factory selection.
							Debug.LogWarning ("No available unit or factory there!");
						}
					}
					else{
						Debug.LogWarning ("This ain't a TerrainTile!");
					}
				} else {
					Debug.LogWarning ("Touched nothing.");
				}

			}
			if(Input.GetMouseButtonDown(1)){ // if right button pressed...

				if(hit.collider != null)
				{
					TerrainTile tile = hit.collider.gameObject.GetComponent<TerrainTile>();
					if(tile != null && tile.unit != null){
						gridUIRangeInstances = showTilesInFiringRange((int) tile.gamePosition.x, (int) tile.gamePosition.y);
					}
					else{
						Debug.LogWarning ("No terrain tile, or no unit on that terrain tile!");
					}
				}
			}
			if(Input.GetMouseButtonUp (1)){ // if right button released...
				hideUITiles(gridUIRangeInstances);
			}
		}
		else if(currentState == CurrentState.SelectedUnit){
			//On a left click, the unit moves, and the Orders UI is shown.
			if(Input.GetMouseButtonDown(0)){
				if(hit.collider != null)
				{
					TerrainTile tile = hit.collider.gameObject.GetComponent<TerrainTile>();
					if(tile != null){
						if(tile.unit != null && tile.unit.Equals(originTile.unit)){
							//User doesn't want to move the unit.
							Debug.Log("Unit keeps its position.");
							destinationTile = tile;
							currentState = CurrentState.MovedUnit;
							hideUITiles(gridUIMoveInstances);
							Main.instance.ordersUI.SetActive(true);
							Main.instance.attackButton.SetActive(getAvailableTargets(destinationTile).Count > 0);
							Main.instance.captureButton.SetActive(destinationTile.canBeCaptured && destinationTile.unit.canCapture && destinationTile.owner != destinationTile.unit.owner);
						}
						else {
							GameObject dest = gridUIMoveInstances.Find ((GameObject obj) => obj.transform.position.Equals(tile.gameObject.transform.position));
							if(dest != null){
								Debug.Log ("Move is valid, destination is " + tile);
								destinationTile = tile;
								moveUnitToTile(originTile, destinationTile);
								currentState = CurrentState.MovedUnit;
								hideUITiles(gridUIMoveInstances);
								Main.instance.ordersUI.SetActive(true);
								Main.instance.attackButton.SetActive(destinationTile.unit.canMoveAndFire && getAvailableTargets(destinationTile).Count > 0);
								Main.instance.captureButton.SetActive(destinationTile.canBeCaptured && destinationTile.unit.canCapture && destinationTile.owner != destinationTile.unit.owner);
							}
							else{
								Debug.LogWarning("Nope, not a valid move.");
							}
						}
					} else{
						Debug.LogWarning ("Did not touch a TerrainTile.");
					}
				}else{
					Debug.LogWarning ("Touched nothing.");
				}
			}

			//On a right click, the unit selection is canceled.
			if(Input.GetMouseButtonDown(1)){
				originTile = null;
				hideUITiles(gridUIMoveInstances);
				currentState = CurrentState.None;
				Main.instance.playerTurnUI.SetActive(true);
			}

		}
		else if(currentState == CurrentState.MovedUnit){
			//Do nothing on a left click! The player must use the orders UI to continue.

			//On a right click, the move order is canceled.
			//The unit is back to its initial position, ready to be moved again.
			if(Input.GetMouseButtonDown(1)){
				moveUnitToTile(destinationTile, originTile);
				destinationTile = null;
				currentState = CurrentState.SelectedUnit;
				Main.instance.ordersUI.SetActive(false);
				gridUIMoveInstances = showTilesInMvtRange((int) originTile.gamePosition.x, (int) originTile.gamePosition.y);
			}
		} 
		else if(currentState == CurrentState.TargetingEnemyUnit){
			//On a left click, we must determine if the player clicked on a valid target.
			if(Input.GetMouseButtonDown(0)){
				if(hit.collider != null)
				{
					TerrainTile tile = hit.collider.gameObject.GetComponent<TerrainTile>();
					if(tile != null){
						GameObject target = availableTargetsGridUIInstances.Find ((GameObject obj) => obj.transform.position.Equals(tile.gameObject.transform.position));
						if(target != null){
							int damage = destinationTile.unit.calculateDamage(tile.unit, destinationTile.unit.hitPoints, tile.defenseRating);
							damageUnit((int) tile.gamePosition.x, (int) tile.gamePosition.y, damage);
							//If both units are melee and alive, the defender can fire back at the attacker.
							if(tile.unit != null && tile.unit.canMoveAndFire && destinationTile.unit.canMoveAndFire){
								damage = tile.unit.calculateDamage(destinationTile.unit, tile.unit.hitPoints, destinationTile.defenseRating);
								damageUnit((int) destinationTile.gamePosition.x, (int) destinationTile.gamePosition.y, damage);
							}
							finishUnitAction();
						}
						else{
							Debug.LogWarning ("Not a valid target.");
						}
					}
					else{
						Debug.LogWarning ("Did not touch a TerrainTile.");
					}
				}else{
					Debug.LogWarning ("Touched nothing.");
				}
			}
			
			//On a right click, the attack order is canceled.
			//The unit doesn't return to its initial position, we'll just show the orders UI again.
			if(Input.GetMouseButtonDown(1)){
				currentState = CurrentState.MovedUnit;
				hideUITiles (availableTargetsGridUIInstances);
				Main.instance.ordersUI.SetActive(true);
			}
		}
		else{
			Debug.LogError("ERROR : Unhandled CurrentState element!");
		}

	}
}
