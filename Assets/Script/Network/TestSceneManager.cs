using UnityEngine;
using System.Collections;

public class TestSceneManager : NetworkManager
{
	private void Start()
	{
        PhotonNetwork.ConnectUsingSettings("0.1");
		PhotonNetwork.offlineMode = true;
        PhotonNetwork.JoinOrCreateRoom("WorldTank", null, null);
	}
}
