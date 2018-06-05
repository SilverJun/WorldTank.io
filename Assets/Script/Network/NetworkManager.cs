using System.Collections;
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
		UIManager.OpenUI<LoginUI>("Prefabs/LoginUI");
    }

	public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		Debug.Log(codeAndMsg);
	}

    public override void OnLeftRoom()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
    }

    public static void StartPlayer()
	{
		var pos = Map.GetRandomSpawnPosition().position;

		// player의 탱크를 instantiate한다.
        Tank = PhotonNetwork.Instantiate("Prefabs/PlayerTank", pos, Quaternion.identity, 0);
        HP = PhotonNetwork.Instantiate("Prefabs/HP", pos, Quaternion.identity, 0);
        PlayerName = UIManager.OpenUIPhoton<PlayerName>("Prefabs/PlayerName");
        PlayerName.name = "PlayerClientName";
        Tank.name = "PlayerClientTank";
        HP.name = "PlayerClientHP";
	}

}
