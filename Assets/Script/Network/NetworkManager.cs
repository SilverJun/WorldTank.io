using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Photon.PunBehaviour 
{   

	// Use this for initialization
	private void Awake()
	{
		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnJoinedLobby()
	{
		//RoomOptions roomOptions = new RoomOptions();
        //roomOptions.customRoomPropertiesForLobby = { "map", "ai" };
        //roomOptions.customRoomProperties = new Hashtable() { { "map", 1 } };
        //roomOptions.maxPlayers = 20;
		PhotonNetwork.JoinOrCreateRoom("WorldTank", null, null);

	}

	public override void OnJoinedRoom()
	{
		Debug.Log("successed to join world!");

		// player의 탱크를 instantiate한다.
		var player = PhotonNetwork.Instantiate("Prefabs/PlayerTank", Vector3.zero, Quaternion.identity, 0);
    }
    
	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		Debug.Log(codeAndMsg);
	}
}
