using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : Photon.PunBehaviour
{
    [SerializeField] private Transform _object;
    [SerializeField] private GameObject _hpGauge;
	private PhotonView _photonView;

    private int _maxHP;

    private void Start()
    {
        _hpGauge.GetComponent<SpriteRenderer>().color = Color.green;
		_photonView = GetComponent<PhotonView>();
    }

    void Update ()
    {
        if (!_photonView.isMine)
            return;

        transform.position = _object.transform.position - new Vector3(0.0f, 0.7f, 0.0f);
    }

	public void SetFollowObject(Transform obj)
	{
		_object = obj;
	}

    public void SetHP(int maxHP)
    {
        _maxHP = maxHP;
		Debug.LogFormat("maxhp: {0}", _maxHP);
    }

    public void UpdateHP(int hp)
    {
        //MapValue(double a0, double a1, double b0, double b1, double a)
		//b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
		float mapValue = hp / (float)_maxHP;
        _hpGauge.transform.localScale = new Vector3(mapValue, 1.0f, 1.0f);

        if (mapValue < 0.3f)
            _hpGauge.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
        else if (mapValue < 0.7f)
            _hpGauge.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0);
        else
            _hpGauge.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
    }
}
