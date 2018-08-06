using UnityEngine;
using UnityEngine.UI;

public class DashBoardUI : UI {

	[SerializeField] private Text _first;
	[SerializeField] private Text _second;
	[SerializeField] private Text _third;
    [SerializeField] private Text _total;

	void Start()
	{
		_total.text = PhotonNetwork.room.PlayerCount.ToString();
    }

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		_total.text = PhotonNetwork.room.PlayerCount.ToString();
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		_total.text = PhotonNetwork.room.PlayerCount.ToString();
	}

	public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		var prop = PhotonNetwork.room.CustomProperties;
		_first.text = prop["First"] + " " + prop["FirstKill"] + "Kill";
		_second.text = prop["Second"] + " " + prop["SecondKill"] + "Kill";
		_third.text = prop["Third"] + " " + prop["ThirdKill"] + "Kill";
	}
}
