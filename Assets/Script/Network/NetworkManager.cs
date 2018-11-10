using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class NetworkManager : PunBehaviour
{
    public static GameObject Tank;
    public static GameObject HP;
    public static UI PlayerName;
    private static int _kill = 0;
    private static ExitGames.Client.Photon.Hashtable dashboardTable;
    private static PhotonView _photonView;

    public static List<PhotonPlayer> users;

    public static int Kill
    {
        get
        {
            return _kill;
        }
        set
        {
            _kill = value;
            Debug.LogFormat("Now PlayerKill : {0}", _kill);
            PhotonNetwork.player.SetScore(_kill);

			_photonView.RPC("UpdateScore", PhotonTargets.All);
        }
    }

    // Use this for initialization
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
        Screen.SetResolution(1280, 720, false);
        Application.targetFrameRate = 60;
        _photonView = GetComponent<PhotonView>();

        dashboardTable = new ExitGames.Client.Photon.Hashtable();

        dashboardTable.Add("First", "");
        dashboardTable.Add("Second", "");
        dashboardTable.Add("Third", "");

        dashboardTable.Add("FirstKill", 0);
        dashboardTable.Add("SecondKill", 0);
        dashboardTable.Add("ThirdKill", 0);
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
        UIManager.OpenUI<DashBoardUI>("Prefabs/DashBoardUI");
        UIManager.OpenUI<LoginUI>("Prefabs/LoginUI");
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log(codeAndMsg);
    }

    //private void OnApplicationQuit()
    //{
    //    PhotonNetwork.Destroy(Tank);
    //    PhotonNetwork.Destroy(HP);
    //    UIManager.CloseUI(PlayerName);

    //    Debug.Log("ChangeMaster");
    //    if (PhotonNetwork.isMasterClient)
    //    {
    //        var player = PhotonNetwork.playerList.Single((x) => !x.IsMasterClient);
    //        Debug.LogFormat("Changed : {0}", player.NickName);
    //        PhotonNetwork.SetMasterClient(player);
    //    }
    //}

    public static void StartPlayer()
    {
        var pos = Map.GetRandomSpawnPosition().position;

        // player의 탱크를 instantiate한다.
        Tank = PhotonNetwork.Instantiate("Prefabs/PlayerTank", pos, Quaternion.identity, 0);
        HP = PhotonNetwork.Instantiate("Prefabs/HP", pos, Quaternion.identity, 0);
        PlayerName = UIManager.OpenUIPhoton<PlayerName>("Prefabs/PlayerName");
        PlayerName.name = "PlayerClientName";
        Tank.name = "PlayerClientTank";
        Tank.tag = "PlayerTank";
        Tank.layer = 8;
        HP.name = "PlayerClientHP";

        _photonView.RPC("UpdateScore", PhotonTargets.All);

        Debug.LogFormat("IsMasterClient? {0}", PhotonNetwork.isMasterClient);
    }


    [PunRPC]
    void UpdateScore()
    {
        /// 전체 유저들의 스코어를 체크하고 정렬, 123등을 만든다.
        users = PhotonNetwork.playerList.ToList().OrderByDescending((x) => { return x.GetScore(); }).ToList();

        dashboardTable["First"] = users[0].NickName;
        dashboardTable["Second"] = users.Count > 1 ? users[1].NickName : "";
        dashboardTable["Third"] = users.Count > 2 ? users[2].NickName : "";

        dashboardTable["FirstKill"] = users[0].GetScore();
        dashboardTable["SecondKill"] = users.Count > 1 ? users[1].GetScore() : 0;
        dashboardTable["ThirdKill"] = users.Count > 2 ? users[2].GetScore() : 0;

        PhotonNetwork.room.SetCustomProperties(dashboardTable);
    }

    public override void OnPhotonMaxCccuReached()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        Debug.LogFormat("MasterClient is Changed.\nIsMasterClient? {0}", PhotonNetwork.isMasterClient);
    }

}
