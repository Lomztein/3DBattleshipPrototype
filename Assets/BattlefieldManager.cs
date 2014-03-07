using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class BattlefieldManager : MonoBehaviour {

	public int activePlayer;
	public int otherPlayer;
	public string currentAction;
	public int shipAmount;
	public bool[] battlefieldGenerated;
	public bool showShipBlocks;
	public bool showEksplosionBlocks;
	public bool showSplashBlocks;
	public bool gameOver;
	public int winnerPlayer;
	public string waitText;
	public int[] shipIndex;
	public Vector3[] randomDirs;
	public int aiAmount;
	NormalAI ai;
	
	public Vector3 size;
	public Vector3 center;
	public int[,,,] coordinates;
	public Vector3[,] blocksPos;
	public GameObject[,] blocks;
	public GameObject[,] shipBlocks;
	public int[] shipsLeft;

	public GameObject mousePointer;
	public GameObject focusPointer;
	public GameObject middlePointer;
	public GameObject shipPlacementPointer;
	public Transform battlefieldWireframe;
	public Transform aim;
	public int mouseDepth;
	public Vector3 focusPoint;
	public float sensitivity;

	public GameObject shipBlock;
	public GameObject eksplosionBlock;
	public GameObject splashBlock;

	public bool buildNew;
	
	BoxCollider bcol;
	Vector3 hitNormal;

	//Players
	//0 = Null
	//1 = 1
	//2 = 2
	//Duh!..

	//Blocks
	//0 = Nothing
	//1 = Ship part
	//2 = Destroyed ship part
	//3 = Splash

	// Use this for initialization
	void Start () {
		/*ai = GetComponent<NormalAI>();
		if (aiEnabled) {
			ai.enabled = true;
		}else{
			ai.enabled = false;
		}*/
		center = size / 2;
		activePlayer = 1;
		shipIndex = new int[2];
		battlefieldGenerated = new bool[2];
		shipsLeft = new int[2];
		coordinates = new int[3,(int)size.x,(int)size.y,(int)size.z];
		blocksPos = new Vector3[3,Mathf.RoundToInt (size.x*size.y*size.z)];
		blocks = new GameObject[3,blocksPos.Length];
		shipBlocks = new GameObject[3,blocksPos.Length];
		battlefieldGenerated[activePlayer-1] = true;
		bcol = GetComponent<BoxCollider>();
		GenerateBattlefield (activePlayer);
		shipIndex[0] = shipAmount;
		shipIndex[1] = shipAmount;

		Camera.main.transform.position = center + new Vector3 (0,0,-size.z*2);
	}

	public void ChangePlayer () {
		if (activePlayer == 1) {
			activePlayer = 2;
			otherPlayer = 1;
		}else{
			activePlayer = 1;
			otherPlayer = 2;
		}
		if (battlefieldGenerated[activePlayer-1] == false) {
			GenerateBattlefield (activePlayer);
		}
		if (shipIndex[activePlayer-1] == 0 && shipIndex[otherPlayer-1] == 0) {
			currentAction = "Firing";
		}else{
			currentAction = "Placing";
		}
		UpdateEverything ();
	}

	public void WaitForPlayer (int player) {
		CleanBattlefield (player);
		CleanBattlefield (otherPlayer);
		currentAction = "Waiting";
	}

	public void PlaceShipsRandomly (int player) {
		while (shipIndex[player-1] > 0) {
			Vector3 randomPos = new Vector3 (Random.Range (0,(int)size.x),Random.Range (0,(int)size.y),Random.Range (0,(int)size.z));
			Vector3 randomDir = randomDirs[Random.Range (0,randomDirs.Length)];
			//Debug.Log (randomPos + ", " + randomDir);
			if (PlaceShip(player,randomPos - randomDir * shipIndex[player-1],shipIndex[player-1]+1,randomDir) == true) {
				shipIndex[player-1]--;
			}
		}
		if (shipIndex[player-1] == 0) {
			WaitForPlayer (otherPlayer);
		}
	}

	public void UpdateEverything () {
		/*if (aiEnabled) {
			TestShipBlocks (ai.aiPlayer-1);
		}
		if (activePlayer == ai.aiPlayer && aiEnabled == true) {
			return;
		}*/
		CleanBattlefield (activePlayer);
		CleanBattlefield (otherPlayer);
		
		UpdateBattlefield (activePlayer,0.25f);
		UpdateBattlefield (otherPlayer,1f);
		
		UpdateShips (activePlayer,0.25f);
	}

	void Update () {
		if (buildNew == true) {
			Start ();
			buildNew = false;
		}
		center = size / 2;
		bcol.center = center-Vector3.one/2;
		bcol.size = size;
		battlefieldWireframe.position = center-Vector3.one/2;
		battlefieldWireframe.localScale = size;

		Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
		RaycastHit hit;
		if (collider.Raycast (ray,out hit, Mathf.Infinity)) {
			hitNormal = hit.normal;
			if (!Input.GetButton ("Fire2")) {
				aim.LookAt ( mousePointer.transform.position + hitNormal );
				mousePointer.transform.rotation = aim.rotation;
				mousePointer.transform.position = new Vector3 ((int)hit.point.x,(int)hit.point.y,(int)hit.point.z);
				focusPoint = focusPointer.transform.position;
			}
			
			if (Input.GetButtonDown ("Fire1")) {
				if (currentAction == "Firing") {
					FireAtPlayer(otherPlayer,focusPoint);
				}
				if (currentAction == "Placing") {
					if (PlaceShip (activePlayer, focusPoint, shipIndex[activePlayer-1]+1, hitNormal)) {
						shipIndex[activePlayer-1]--;
						if (shipIndex[activePlayer-1] == 0) {
							WaitForPlayer (otherPlayer);
						}
					}
				}
			}
		}

		if (Input.GetButton ("Fire2")) {
			float mouseX = Input.GetAxis ("Mouse X") * Time.deltaTime * -sensitivity;
			float mouseY = Input.GetAxis ("Mouse Y") * Time.deltaTime * -sensitivity;
			float mouseZ = Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * sensitivity*2;
			Camera.main.transform.Translate ( mouseX, mouseY, mouseZ );
		}else{
			int newMouseDepth = mouseDepth + Mathf.RoundToInt(Input.GetAxis ("Mouse ScrollWheel"));
			middlePointer.transform.localScale = new Vector3 ((float)mouseDepth,0.5f,0.5f);
			middlePointer.transform.localPosition = new Vector3 (0,0,-(float)mouseDepth/2+0.5f);
			focusPointer.transform.localPosition = new Vector3 (0,0,-mouseDepth);
			newMouseDepth = TestPointerDepth(newMouseDepth);
			mouseDepth = newMouseDepth;
		}
		if (currentAction == "Placing") {
			shipPlacementPointer.transform.localPosition = new Vector3 (-(float)shipIndex[activePlayer-1]+2,0,0);
			shipPlacementPointer.transform.localScale = new Vector3 (1,1,(float)shipIndex[activePlayer-1]+1);
		}else{
			shipPlacementPointer.transform.localScale = new Vector3 (0,0,0);
		}

		Camera.main.transform.LookAt (center-Vector3.one/2);
		
	}

	int TestPointerDepth (int value) {
		
		focusPoint = new Vector3 (Mathf.RoundToInt(focusPoint.x),Mathf.RoundToInt(focusPoint.y),Mathf.RoundToInt(focusPoint.z));
		if (value <= 0) {
			value = 0;
		}
		if (hitNormal.x != 0) {
			if ((float)value >= size.x-1) {
				value = Mathf.RoundToInt(size.x-1);
			}
		}
		if (hitNormal.y != 0) {
			if ((float)value >= size.y-1) {
				value = Mathf.RoundToInt(size.y)-1;
			}
		}
		if (hitNormal.z != 0) {
			if ((float)value >= size.z-1) {
				value = Mathf.RoundToInt(size.z)-1;
			}
		}
		return value;
	}
	
	void GenerateBattlefield (int player) {

		int blockIndex = 0;
		for (int x=0;x<size.x;x++) {
			for (int y=0;y<size.y;y++) {
				for (int z=0;z<size.z;z++) {
					coordinates[player,x,y,z] = 0;
					blocksPos[player,blockIndex] = new Vector3 ( x,y,z );
					blockIndex++;
				}
			}
		}
		battlefieldGenerated[player-1] = true;
	}

	public int GetBlock (int player, Vector3 pos) {
		if (IsInsideBattlefield(pos)) {
			return coordinates[player, Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y),Mathf.RoundToInt(pos.z)];
		}
		return 0;
	}

	public void ChangeBlock (int player, Vector3 pos, int newBlock) {
		if (IsInsideBattlefield(pos)) {
			coordinates[player, Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y),Mathf.RoundToInt(pos.z)] = newBlock;
		}
	}

	public void CleanBattlefield (int player) {
		GameObject[] sh = GameObject.FindGameObjectsWithTag("ShipBlock");
		GameObject[] ex = GameObject.FindGameObjectsWithTag("EksplosionBlock");
		GameObject[] sp = GameObject.FindGameObjectsWithTag("SplashBlock");
		for (int i=0;i<sh.Length;i++) {
			Destroy(sh[i]);
		}
		for (int i=0;i<ex.Length;i++) {
			Destroy(ex[i]);
		}
		for (int i=0;i<sp.Length;i++) {
			Destroy(sp[i]);
		}
	}

	public void UpdateBattlefield (int player, float alpha) {
		for (int i=0;i<blocksPos.Length/3;i++) {
			if (GetBlock (player,blocksPos[player,i]) == 2 && showEksplosionBlocks) {
				GameObject newBlock = (GameObject)Instantiate(eksplosionBlock,blocksPos[player,i],Quaternion.identity);
				newBlock.renderer.material.color = new Color (1,0,0,alpha);
				newBlock.transform.parent = transform;
				blocks[player,i] = newBlock;
			}
			if (GetBlock (player,blocksPos[player,i]) == 3 && showSplashBlocks) {
				GameObject newBlock = (GameObject)Instantiate(splashBlock,blocksPos[player,i],Quaternion.identity);
				newBlock.renderer.material.color = new Color (0,0,1,alpha);
				newBlock.transform.parent = transform;
				blocks[player,i] = newBlock;
			}
		}
		//Debug.Log ("Updated battlefield data from player " + player);
		//int shipsLeft = TestShipBlocks (player);
	}

	public void UpdateShips (int player, float alpha) {
		int n = 0;
		for (int i=0;i<blocksPos.Length/3;i++) {
			if (GetBlock (player,blocksPos[player,i]) == 1 && showShipBlocks) {
				GameObject newBlock = (GameObject)Instantiate(shipBlock,blocksPos[player,i],Quaternion.identity);
				newBlock.renderer.material.color = new Color (1,1,1,alpha);
				newBlock.transform.parent = transform;
				shipBlocks[player,i] = newBlock;
				n++;
			}
		}
		shipsLeft[player-1] = n;
		//Debug.Log ("Updated battleship data from player " + player);
	}

	public Vector3 FireRandomly (int player) {
		Vector3 randomPos = new Vector3 (Random.Range (0,(int)size.x),Random.Range (0,(int)size.y),Random.Range (0,(int)size.z));
		FireAtPlayer (player,randomPos);
		return randomPos;
	}

	public int TestShipBlocks (int player) {
		int n = 0;
		for (int i=0;i<blocksPos.Length/3;i++) {
			if (GetBlock (player,blocksPos[player,i]) == 1) {
				n++;
			}
		}
		//Debug.Log ("Player " + player + " has " + n + " ship blocks left!");
		return n;
	}

	public bool IsInsideBattlefield (Vector3 pos) {
		bool inside = true;
		if (pos.x > size.x) { inside = false; }
		if (pos.x < -0.1) { inside = false; }
		if (pos.y > size.y) { inside = false; }
		if (pos.y < -0.1) { inside = false; }
		if (pos.z > size.z) { inside = false; }
		if (pos.z < -0.1) { inside = false; }
		//if (inside == false) { Debug.Log ("Tested pos: " + inside + ", " + pos); }
		return inside;
	}

	public int FireAtPlayer (int player, Vector3 pos) {
		int block = GetBlock (player,pos);
		if (block == 1) {
			ChangeBlock (player,pos,2);
			UpdateEverything ();
		}
		if (block == 0) {
			ChangeBlock (player,pos,3);
			WaitForPlayer (otherPlayer);
		}
		return block;
	}

	public bool PlaceShip (int player, Vector3 pos, int length, Vector3 direction) {
		Debug.DrawRay (pos,direction*length,Color.white,5);
		bool canPlace = true;
		for (int i=0;i<length;i++) {
			Vector3 newPos = pos + direction * i;
			//Debug.Log (newPos);
			if (IsInsideBattlefield(newPos) == false || GetBlock (player,pos) == 1) {
				canPlace = false;
			}
		}
		if (canPlace) {
			for (int i=0;i<length;i++) {
				Vector3 newPos = pos + direction * i;
				ChangeBlock (player,newPos,1);
			}
		}

		UpdateShips (player,1);
		return canPlace;
	}

	bool TestForNearbyShip (Vector3 pos) {
		bool isNearby = false;
		if (Physics.CheckSphere (pos,0.75f)) {
			isNearby = true;
		}
		return isNearby;
	}

	void OnDrawGizmos () {
		Gizmos.DrawWireCube (center-Vector3.one/2,size);
	}

	void OnGUI () {
		if (currentAction == "Waiting") {
			if (GUI.Button (new Rect(Screen.width/2-100,30,200,30),"Change player!")) {
				ChangePlayer();
			}
		}
		if (currentAction == "Placing") {
			if (GUI.Button (new Rect(10,10,200,30),"Place ships randomly.")) {
				PlaceShipsRandomly(activePlayer);
			}
		}
		GUI.Label (new Rect(10,40,Screen.width,20),"Pointer position: " + focusPointer.transform.position);
		GUI.Label (new Rect(10,60,Screen.width,20),"Player: " + activePlayer.ToString() + ". Mode: " + currentAction.ToString());
		GUI.Label (new Rect(10,80,Screen.width,20),"Player 1 ship blocks left: " + shipsLeft[0].ToString());
		GUI.Label (new Rect(10,100,Screen.width,20),"Player 2 ship blocks left: " + shipsLeft[1].ToString());
	}
}