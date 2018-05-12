using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] private Transform _object;
    [SerializeField] private GameObject _hpGauge;

    private int _maxHP;

    private void Start()
    {
        _hpGauge.GetComponent<SpriteRenderer>().color = Color.green;
        _maxHP = 0;
    }

    void Update ()
	{
	    transform.position = _object.transform.position - new Vector3(0.0f, 0.7f, 0.0f);
    }

    public void SetHP(int maxHP)
    {
        _maxHP = maxHP;
    }

    public void UpdateHP(int hp)
    {
        //MapValue(double a0, double a1, double b0, double b1, double a)
        //b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
        float mapValue = (float)hp / _maxHP;
        _hpGauge.transform.localScale = new Vector3(mapValue, 0.0f, 0.0f);

        if (mapValue < 0.3f)
            _hpGauge.GetComponent<SpriteRenderer>().color = Color.red;
        else if (mapValue < 0.7f)
            _hpGauge.GetComponent<SpriteRenderer>().color = Color.yellow;
        else
            _hpGauge.GetComponent<SpriteRenderer>().color = Color.green;
    }
}
