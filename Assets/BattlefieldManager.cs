using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]

public class BattlefieldManager : MonoBehaviour {

	public Vector3 size;
	public Vector3 center;
	public int[,,] coordinates;
	public Vector3[] blocks;

	public GameObject mousePointer;
	public GameObject focusPointer;
	public Transform aim;
	public int mouseDepth;
	public Vector3 focusPoint;
	public float sensitivity;

	public bool buildNew;
	
	BoxCollider bcol;

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
			Debug.Log (hit.normal);
			aim.LookAt ( mousePointer.transform.position + hit.normal );
			if (Mathf.RoundToInt(hit.normal.y) == 0) {
				mousePointer.transform.rotation = Quaternion.Euler (aim.rotation.eulerAngles + new Vector3(0,90,0));
			}else{
				mousePointer.transform.rotation = Quaternion.Euler (aim.rotation.eulerAngles + new Vector3(0,0,0));
			}
			mousePointer.transform.position = new Vector3 ((int)hit.point.x,(int)hit.point.y,(int)hit.point.z);
			focusPointer.transform.localPosition = new Vector3 (mouseDepth,0,0);
			focusPoint = focusPointer.transform.position;
		}

		if (Input.GetButton ("Fire2")) {
			float mouseX = Input.GetAxis ("Mouse X") * Time.deltaTime * -sensitivity;
			float mouseY = Input.GetAxis ("Mouse Y") * Time.deltaTime * -sensitivity;
			float mouseZ = Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * sensitivity*2;
			Camera.main.transform.Translate ( mouseX, mouseY, mouseZ );
		}else{
			mouseDepth += (int)Input.GetAxis ("Mouse ScrollWheel");
		}

		if (Input.GetButton ("Fire1")) {
			ChangeBlock (focusPoint,1);
		}

		Camera.main.transform.LookAt (center);
		
	}
	
	void GenerateBattlefield () {

		coordinates = new int[(int)size.x,(int)size.y,(int)size.z];
		blocks = new Vector3[Mathf.RoundToInt (size.x*size.y*size.z)];
		//int blockAmount = coordinates.Length;

		int blockIndex = 0;
		for (int x=0;x<size.x;x++) {
			for (int y=0;y<size.y;y++) {
				for (int z=0;z<size.z;z++) {
					coordinates[x,y,z] = 0;
					blocks[blockIndex] = new Vector3 ( x,y,z );
					blockIndex++;
				}
			}
		}
	}

	int GetBlock (Vector3 pos) {
		return coordinates[(int)pos.x,(int)pos.y,(int)pos.z];
	}

	void ChangeBlock (Vector3 pos, int newBlock) {
		coordinates[(int)pos.x,(int)pos.y,(int)pos.z] = newBlock;
	}

	void OnDrawGizmos () {
		Gizmos.DrawWireCube (center-Vector3.one/2,size);
		if (bcol) {
			for (int i=0;i<blocks.Length;i++) {
				if (GetBlock(blocks[i]) == 1) {
					Gizmos.DrawCube (blocks[i],Vector3.one);
				}
			}
		}
	}
}