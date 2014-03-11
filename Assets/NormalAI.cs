using UnityEngine;
using System.Collections;

public class NormalAI : MonoBehaviour {

	public BattlefieldManager f;
	public Vector3 lastRandomHit;
	public Vector3 lastHit;
	public int aiPlayer = 2;
	public Vector3[] randomDirs;
	public Vector3 expectedDir;
	public int hitsInRow;
	public bool isSearching = true;
	Vector3 tempPos;

	void Start () {
		f = GetComponent<BattlefieldManager>();
		randomDirs = f.randomDirs;
	}
	void Update () {
		if (f.currentAction == "Waiting" && f.otherPlayer == aiPlayer) {
			//Debug.Log ("AI forced player change");
			f.ChangePlayer ();
		}
		if (f.currentAction == "Placing" && f.activePlayer == aiPlayer) {
			while (f.shipIndex[aiPlayer-1] > 0) {
				f.PlaceShipsRandomly (aiPlayer);
			}
			if (f.shipIndex[aiPlayer-1] <= 0) {
				f.ChangePlayer ();
			}
		}
		if (f.currentAction == "Firing" && f.activePlayer == aiPlayer) {
			Vector3 newPos = new Vector3(0,0,0);
			if (isSearching) {
				hitsInRow = 0;
				newPos = GetRandomPos ();
			}
			if (hitsInRow == 1) {
				expectedDir = randomDirs[Random.Range (0,randomDirs.Length)];
				newPos = lastHit + expectedDir;
			}
			if (hitsInRow > 1) {
				newPos = lastHit + expectedDir;
			}
			if (TestNearbyBlocks(aiPlayer,newPos)) {
				hitsInRow = 0;
				isSearching = true;
			}
			if (f.IsInsideBattlefield(newPos) == false) {
				hitsInRow = 1;
			}
			int newBlock = AIFire (f.otherPlayer,newPos);
			if (newBlock == 1) {
				hitsInRow++;
				if (isSearching) {
					isSearching = false;
					lastRandomHit = newPos;
					lastHit = lastRandomHit;
				}
				if (hitsInRow > 0) {
					lastHit = newPos;
				}
			}else if (newBlock == 0) {
				if (isSearching == false) {
					isSearching = true;
				}
			}else{
				if (TestNearbyBlocks(aiPlayer,newPos)) {
					if (lastHit != lastRandomHit) {
						lastHit = lastRandomHit;
					}else{
						isSearching = true;
						hitsInRow = 0;
					}
				}
			}
			tempPos = newPos;
		}
	}

	int AIFire (int player, Vector3 pos) {
		return f.FireAtPlayer (player,pos);
	}

	Vector3 GetRandomPos () {
		return new Vector3 (Random.Range(0,(int)f.size.x-1),Random.Range(0,(int)f.size.y-1),Random.Range(0,(int)f.size.z-1));
	}
		                   
	bool TestNearbyBlocks (int player, Vector3 pos) {
		bool filledNearby = true;
		for (int i=0;i<randomDirs.Length;i++) {
			if (f.IsInsideBattlefield(pos + randomDirs[i])) {
				if (f.GetBlock (player,pos + randomDirs[i]) < 1) {
					filledNearby = false;
				}
			}
		}
		return filledNearby;
	}

	void OnDrawGizmos () {
		Gizmos.DrawSphere (lastHit,0.25f);
		Gizmos.DrawSphere (lastRandomHit,0.5f);
		Gizmos.DrawSphere (tempPos,0.1f);
	}
}
