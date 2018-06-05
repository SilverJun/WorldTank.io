using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUI : UI
{
    [SerializeField] private Button _button;


    // Use this for initialization
    void Start()
    {
        _button.onClick.AddListener(() => {
			NetworkManager.StartPlayer();
			UIManager.CloseUI(this);
        });
    }
}
