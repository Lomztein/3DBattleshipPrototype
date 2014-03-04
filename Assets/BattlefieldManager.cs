using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class BattlefieldManager : MonoBehaviour {

	public int activePlayer;
	public int otherPlayer;
	public bool[] battlefieldGenerated;
	public bool showShipBlocks;
	public bool showEksplosionBlocks;
	public bool showSplashBlocks;

	public Vector3 size;
	public Vector3 center;
	public int[,,,] coordinates;
	public Vector3[,] blocksPos;
	public GameObject[,] blocks;

	public GameObject mousePointer;
	public GameObject focusPointer;
	public GameObject middlePointer;
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
	//0 = None
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
		activePlayer = 1;
		battlefieldGenerated = new bool[2];
		coordinates = new int[3,(int)size.x,(int)size.y,(int)size.z];
		blocksPos = new Vector3[3,Mathf.RoundToInt (size.x*size.y*size.z)];
		blocks = new GameObject[3,blocksPos.Length];
		battlefieldGenerated[activePlayer-1] = true;
		bcol = GetComponent<BoxCollider>();
		GenerateBattlefield (activePlayer);
	}

	void ChangePlayer () {
		if (activePlayer == 1) {
			activePlayer = 2;
		}else{
			activePlayer = 1;
		}
		if (battlefieldGenerated[activePlayer-1] == false) {
			GenerateBattlefield (activePlayer);
		}
		UpdateBattlefield (activePlayer);
		Debug.Log ("Changed player!");
	}

	void Update () {
		if (activePlayer == 1) {
			otherPlayer = 2;
		}else{
			otherPlayer = 1;
		}
		if (buildNew == true) {
			Start ();
			buildNew = false;
		}
		center = size / 2;
		bcol.center = center-Vector3.one/2;
		bcol.size = size;

		Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );
		RaycastHit hit;
		if (collider.Raycast (ray,out hit, Mathf.Infinity)) {
			hitNormal = hit.normal;
			aim.LookAt ( mousePointer.transform.position + hitNormal );
			mousePointer.transform.rotation = aim.rotation;
			mousePointer.transform.position = new Vector3 ((int)hit.point.x,(int)hit.point.y,(int)hit.point.z);
			focusPoint = focusPointer.transform.position;
			
			if (Input.GetButtonDown ("Fire1")) {
				if (GetBlock (activePlayer,focusPoint) < 3) {
					ChangeBlock (activePlayer,focusPoint,GetBlock (activePlayer,focusPoint)+1);
				}else{
					ChangeBlock (activePlayer,focusPoint,0);
				}
				UpdateBattlefield (activePlayer);
			}
		}

		if (Input.GetButton ("Fire2")) {
			float mouseX = Input.GetAxis ("Mouse X") * Time.deltaTime * -sensitivity;
			float mouseY = Input.GetAxis ("Mouse Y") * Time.deltaTime * -sensitivity;
			float mouseZ = Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * sensitivity*2;
			Camera.main.transform.Translate ( mouseX, mouseY, mouseZ );
		}else{
			mouseDepth += (int)Input.GetAxis ("Mouse ScrollWheel");
			middlePointer.transform.localScale = new Vector3 ((float)mouseDepth,0.75f,0.75f);
			middlePointer.transform.localPosition = new Vector3 (0,0,-(float)mouseDepth/2+0.5f);
			focusPointer.transform.localPosition = new Vector3 (0,0,-mouseDepth);
		}
		if (mouseDepth <= 0) {
			mouseDepth = 0;
		}
		if (hitNormal.x != 0) {
			if ((float)mouseDepth >= size.x-1) {
				mouseDepth = (int)size.x-1;
			}
		}
		if (hitNormal.y != 0) {
			if ((float)mouseDepth >= size.y-1) {
				mouseDepth = (int)size.y-1;
			}
		}
		if (hitNormal.z != 0) {
			if ((float)mouseDepth >= size.z-1) {
				mouseDepth = (int)size.z-1;
			}
		}
		if (GetBlock (activePlayer,focusPoint) > 1) {
			focusPointer.renderer.material.color = Color.red;
		}
		if (GetBlock (activePlayer,focusPoint) == 1) {
			focusPointer.renderer.material.color = Color.blue;
		}
		if (GetBlock (activePlayer,focusPoint) == 0) {
			focusPointer.renderer.material.color = Color.green;
		}

		Camera.main.transform.LookAt (center-Vector3.one/2);
		
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
	}

	int GetBlock (int player, Vector3 pos) {
		return coordinates[player, (int)pos.x,(int)pos.y,(int)pos.z];
	}

	void ChangeBlock (int player, Vector3 pos, int newBlock) {
		coordinates[player, (int)pos.x,(int)pos.y,(int)pos.z] = newBlock;
	}

	void UpdateBattlefield (int player) {
		Debug.Log (blocksPos.Length/3);
		for (int i=0;i<blocksPos.Length;i++) {
			Debug.Log (player + ", " + i);
			if (blocks[player,i]) {
				Destroy (blocks[player,i]);
			}
			if (GetBlock (player,blocksPos[player,i]) == 1) {
				GameObject newBlock = (GameObject)Instantiate(shipBlock,blocksPos[player,i],Quaternion.identity);
				newBlock.transform.parent = transform;
				blocks[player,i] = newBlock;
			}
			if (GetBlock (player,blocksPos[player,i]) == 2) {
				GameObject newBlock = (GameObject)Instantiate(eksplosionBlock,blocksPos[player,i],Quaternion.identity);
				newBlock.transform.parent = transform;
				blocks[player,i] = newBlock;
			}
			if (GetBlock (player,blocksPos[player,i]) == 3) {
				GameObject newBlock = (GameObject)Instantiate(splashBlock,blocksPos[player,i],Quaternion.identity);
				newBlock.transform.parent = transform;
				blocks[player,i] = newBlock;
			}
		}
	}
	void FireRandomly (int player) {
		Vector3 randomPos = new Vector3 (Random.Range (0,(int)size.x),Random.Range (0,(int)size.y),Random.Range (0,(int)size.z));
		if (GetBlock(player,randomPos) != 1) {
			ChangeBlock(player,randomPos,3);
		}else{
			ChangeBlock(player,randomPos,1);
		}
	}

	void OnDrawGizmos () {
		Gizmos.DrawWireCube (center-Vector3.one/2,size);
	}

	void OnGUI () {
		if (GUI.Button (new Rect(10,10,200,30),"Change player!")) {
			ChangePlayer();
		}
	}
}