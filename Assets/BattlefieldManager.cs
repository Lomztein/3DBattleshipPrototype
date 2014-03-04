using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]

public class BattlefieldManager : MonoBehaviour {

	public Vector3 size;
	public Vector3 center;
	public int[,,] coordinates;
	public Vector3[] blocksPos;
	public GameObject[] blocks;

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

	//0 = Nothing
	//1 = Ship part
	//2 = Destroyed ship part
	//3 = Splash

	// Use this for initialization
	void Start () {
		GenerateBattlefield ();
		bcol = GetComponent<BoxCollider>();
	}

	void Update () {
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
			
			if (Input.GetButton ("Fire1")) {
				ChangeBlock (focusPoint,1);
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
		if (GetBlock (focusPoint) > 1) {
			focusPointer.renderer.material.color = Color.red;
		}
		if (GetBlock (focusPoint) == 0) {
			focusPointer.renderer.material.color = Color.blue;
		}

		Camera.main.transform.LookAt (center-Vector3.one/2);
		
	}
	
	void GenerateBattlefield () {

		coordinates = new int[(int)size.x,(int)size.y,(int)size.z];
		blocksPos = new Vector3[Mathf.RoundToInt (size.x*size.y*size.z)];
		//int blockAmount = coordinates.Length;

		int blockIndex = 0;
		for (int x=0;x<size.x;x++) {
			for (int y=0;y<size.y;y++) {
				for (int z=0;z<size.z;z++) {
					coordinates[x,y,z] = Random.Range (0,4);
					blocksPos[blockIndex] = new Vector3 ( x,y,z );
					blockIndex++;
				}
			}
		}
		UpdateBattlefield ();
	}

	int GetBlock (Vector3 pos) {
		return coordinates[(int)pos.x,(int)pos.y,(int)pos.z];
	}

	void ChangeBlock (Vector3 pos, int newBlock) {
		coordinates[(int)pos.x,(int)pos.y,(int)pos.z] = newBlock;
	}

	void UpdateBattlefield () {
		for (int i=0;i<blocksPos.Length;i++) {
			if (GetBlock (blocksPos[i]) == 0) {
				if (blocks[i]) {
					DestroyImmediate (blocks[i]);
					blocks[i] = null;
				}
			}
			if (GetBlock (blocksPos[i]) == 1) {
				GameObject newBlock = (GameObject)Instantiate(shipBlock,blocksPos[i],Quaternion.identity);
				newBlock.transform.parent = transform;
				blocks[i] = newBlock;
			}
			if (GetBlock (blocksPos[i]) == 2) {
				GameObject newBlock = (GameObject)Instantiate(eksplosionBlock,blocksPos[i],Quaternion.identity);
				newBlock.transform.parent = transform;
				blocks[i] = newBlock;
			}
			if (GetBlock (blocksPos[i]) == 3) {
				GameObject newBlock = (GameObject)Instantiate(splashBlock,blocksPos[i],Quaternion.identity);
				newBlock.transform.parent = transform;
				blocks[i] = newBlock;
			}
		}
	}
	void FireRandomly () {
		Vector3 randomPos = new Vector3 (Random.Range (0,(int)size.x),Random.Range (0,(int)size.y),Random.Range (0,(int)size.z));
		if (GetBlock(randomPos) != 1) {
			ChangeBlock(randomPos,3);
		}else{
			ChangeBlock(randomPos,1);
		}
	}

	void OnDrawGizmos () {
		Gizmos.DrawWireCube (center-Vector3.one/2,size);
	}
}