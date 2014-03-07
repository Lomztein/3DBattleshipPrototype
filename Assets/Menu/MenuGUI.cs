using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {

	/*public Vector3 size;
	public int shipAmount;
	public int aiAmount;
	public int heighestSize;
	public StatsCarrier statsCarrier;
	
	// Use this for initialization
	void Start () {
		statsCarrier = GameObject.Find ("StatsCarrier").GetComponent<StatsCarrier>();
	}
	
	void SendData () {
		statsCarrier.size = size;
		statsCarrier.shipAmount = shipAmount;
		statsCarrier.aiAmount = aiAmount;
	}

	void Update () {
		float s = 0;
		if (size.x > s) {
			s = size.x;
		}
		if (size.y > s) {
			s = size.y;
		}
		if (size.z > s) {
			s = size.z;
		}
		heighestSize = Mathf.RoundToInt(s-1);
	}

	void OnGUI () {
		float newX = 10;
		float newY = 10;
		float newZ = 10;
		float newShipAmount = 5;
		float newAIAmount = 0;

		GUI.Label (new Rect(10,10,Screen.width,20),"3D BATTLESHIP PROTOTYPE THINGIE");
		GUI.Label (new Rect(10,30,Screen.width,20),"INTRODUCING LAZIEST MENU EVER");
		GUI.Label (new Rect(10,50,Screen.width,20),"PROGRAMMED BY LOMZTEIN");

		newX = GUI.HorizontalSlider (new Rect(10,80,200,20),size.x,1,20);
		newY = GUI.HorizontalSlider (new Rect(10,100,200,20),size.y,1,20);
		newZ = GUI.HorizontalSlider (new Rect(10,120,200,20),size.z,1,20);
		GUI.Label (new Rect(10,140,Screen.width,20),"Battlefield size: " + size.ToString());

		newShipAmount = GUI.HorizontalSlider (new Rect(10,170,200,20),shipAmount,1,(float)heighestSize);
		GUI.Label (new Rect(10,190,Screen.width,20),"Amount of ships: " + ((float)shipAmount).ToString() + ". Max ships: " + heighestSize);

		newAIAmount = GUI.HorizontalSlider (new Rect(10,220,200,20),(float)aiAmount,0,2);
		GUI.Label (new Rect(10,240,Screen.width,20),"AIs: " + ((float)aiAmount).ToString() + ". 0 = PVP, 1 = PVE, 2 = EVE");
		
		if (newShipAmount >= (float)heighestSize) { newShipAmount = (float) heighestSize; }
		size = new Vector3 (Mathf.Round (newX),Mathf.Round (newY),Mathf.Round (newZ));
		shipAmount = Mathf.RoundToInt(newShipAmount);
		aiAmount = Mathf.RoundToInt(newAIAmount);

		if (GUI.Button (new Rect(10,270,200,50),"READY?")) {
			Application.LoadLevel ("3dbp_play");
		}
		if (GUI.Button (new Rect(10,330,200,50),"QUIT TO DESKTOP")) {
			Application.Quit();
		}
		statsCarrier.size = size;
		statsCarrier.shipAmount = shipAmount;
		statsCarrier.aiAmount = aiAmount;
	}*/
}
