﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Photon.PunBehaviour 
{
	public static GameObject Tank;
	public static GameObject HP;
	public static UI PlayerName;

	// Use this for initialization
	private void Awake()
	{
		PhotonNetwork.ConnectUsingSettings("0.1");
        Screen.SetResolution(1280, 720, false);
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

		PhotonNetwork.playerName = "Client"+PhotonNetwork.countOfPlayers;
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("successed to join world!");

		// player의 탱크를 instantiate한다.
		Tank = PhotonNetwork.Instantiate("Prefabs/PlayerTank", Vector3.zero, Quaternion.identity, 0);
	    HP = PhotonNetwork.Instantiate("Prefabs/HP", Vector3.zero, Quaternion.identity, 0);
		PlayerName = UIManager.OpenUIPhoton<PlayerName>("Prefabs/PlayerName");
		PlayerName.name = "PlayerClientName";
		PlayerName.GetComponent<UnityEngine.UI.Text>().text = PhotonNetwork.playerName;
	    Tank.name = "PlayerClientTank";
	    HP.name = "PlayerClientHP";
        Debug.Log(Tank);
	    Debug.Log(HP);
		Debug.Log(PlayerName);
    }
    

	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		Debug.Log(codeAndMsg);
	}

    public override void OnLeftRoom()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
    }
}
