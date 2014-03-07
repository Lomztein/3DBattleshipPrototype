using UnityEngine;
using System.Collections;

public class NormalAI : MonoBehaviour {

	public BattlefieldManager f;
	public Vector3 lastHit;
	public int aiPlayer = 2;

	void Start () {
		f = GetComponent<BattlefieldManager>();
	}
	void Update () {
		if (f.currentAction == "Waiting" && f.otherPlayer == aiPlayer) {
			//Debug.Log ("AI forced player change");
			f.ChangePlayer ();
		}
		if (f.currentAction == "Placing" && f.activePlayer == aiPlayer) {
			if (f.shipIndex[aiPlayer-1] > 0) {
				f.PlaceShipsRandomly (aiPlayer);
				//Debug.Log ("AI placed ships!");
			}else{
				f.ChangePlayer ();
			}
		}
		if (f.currentAction == "Firing" && f.activePlayer == aiPlayer) {
			Vector3 randomPos = new Vector3 (Random.Range (0,(int)f.size.x),Random.Range (0,(int)f.size.y),Random.Range (0,(int)f.size.z));
			//Debug.Log ("AI fired at " + randomPos);
			if (f.FireAtPlayer(f.otherPlayer,randomPos) == 1) {
				lastHit = randomPos;
			}else{
				f.ChangePlayer ();
			}
		}
	}
}
