using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UI {
	[SerializeField] private InputField _inputField;
	[SerializeField] private Button _button;


	// Use this for initialization
	void Start () {
		_button.onClick.AddListener(() => {
			if (_inputField.text.Length != 0)
			{
				PhotonNetwork.player.NickName = _inputField.text;
				NetworkManager.StartPlayer();
				UIManager.CloseUI(this);
			}
		});
	}
}
