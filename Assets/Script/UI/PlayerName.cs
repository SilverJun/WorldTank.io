using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : UI {
	private Tank _tank;
    private PhotonView _photonView;
	private Camera _mainCamera;
	private RectTransform _rectTransform;

	// Use this for initialization
	void Start () {
		transform.SetParent(UIManager.Instance.transform, false);
		_photonView = GetComponent<PhotonView>();
		_mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _rectTransform = GetComponent<RectTransform>();
		_rectTransform.position = Vector3.zero;
        GetComponent<Text>().text = _photonView.owner.NickName;

        if (_photonView.isMine)
			_tank = NetworkManager.Tank.GetComponent<Tank>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!_photonView.isMine)
			return;
        
		_rectTransform.position = _tank.transform.position + new Vector3(0.0f, 0.7f, 0.0f);
	}
}
