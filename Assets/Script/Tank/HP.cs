using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : Photon.PunBehaviour
{
    [SerializeField] private Tank _object;
    [SerializeField] private GameObject _hpGauge;
	private PhotonView _photonView;

    private int _maxHP;
    private float _mapValue;

    private void Start()
    {
        _hpGauge.GetComponent<SpriteRenderer>().color = Color.green;
		_photonView = GetComponent<PhotonView>();
        if (_photonView.isMine)
        {
            _object = NetworkManager.Tank.GetComponent<Tank>();
            _maxHP = _object.Hp;
        }
    }

    void Update ()
    {
        if (_hpGauge.transform.localScale.x < 0.3f)
            _hpGauge.GetComponent<SpriteRenderer>().color = Color.red;
        else if (_hpGauge.transform.localScale.x < 0.7f)
            _hpGauge.GetComponent<SpriteRenderer>().color = Color.yellow;
        else
            _hpGauge.GetComponent<SpriteRenderer>().color = Color.green;

        
        if (!_photonView.isMine)
            return;

        transform.position = _object.transform.position - new Vector3(0.0f, 0.7f, 0.0f);

        _mapValue = _object.Hp / (float)_maxHP;
        _hpGauge.transform.localScale = new Vector3(_mapValue, 1.0f, 1.0f);
    }
}
