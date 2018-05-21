﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tank : Photon.MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _barrel;
    [SerializeField] private GameObject _body;

    private Rigidbody2D _rigid;
    private GameObject _bullet;

    private Vector3 _worldBarrelPos;

	private PhotonView _photonView;

    // data
    [SerializeField] private float _barrelRotateSpeed = 1.0f;
    [SerializeField] private float _bodyRotateSpeed = 1.0f;
    [SerializeField] private float _tankSpeed = 5.0f;
    [SerializeField] private float _shootDelay = 1.0f;
    [SerializeField] private int _hp;
    [SerializeField] private int _maxHP = 100;
    private bool _isShoot = false;
    private bool _isDie = false;

    public int Hp
    {
        get
        {
            return _hp;
        }
    }

    private void Awake()
    {
        _hp = _maxHP;
    }

    void Start ()
    {
        _photonView = GetComponent<PhotonView>();
        _camera = GameObject.FindWithTag("MainCamera");
        _rigid = GetComponent<Rigidbody2D>();
		_barrel = transform.Find("Barrel").gameObject;
        _body = transform.Find("Body").gameObject;
    }

    void FixedUpdate ()
	{
		if (!_photonView.isMine)
		{
			//transform.position = _curPos;
			//transform.rotation = _curQuat;
			return;
		}

	    if (_isDie)
	        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);

	    if (_hp <= 0)
	    {
	        _isDie = true;
	        return;
	    }

        _camera.transform.position = new Vector3(transform.position.x, transform.position.y, _camera.transform.position.z);

        LookBarrelMouse();
	    MoveTank();

	    // Left Mouse Click
	    if (!_isShoot && Input.GetMouseButtonDown(0))
	    {
            // 총알이랑 바렐이랑 보고있는 방향이 달라서 차이값만큼 보정
	        PhotonNetwork.Instantiate("Prefabs/Bullet", transform.position, Quaternion.Euler(0.0f, 0.0f, _barrel.transform.eulerAngles.z - 90.0f), 0);
	        _isShoot = true;
	        StartCoroutine(_reloadBullet()); // 재장전
	    }

	}

    void LookBarrelMouse()
    {
        _worldBarrelPos = Camera.main.WorldToScreenPoint(_barrel.transform.position);
        var dir = Input.mousePosition - _worldBarrelPos;
        _barrel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90.0f));
    }

    void MoveTank()
    {
        if (Input.GetKey(KeyCode.W))    // 전진 이동.
        {
            _rigid.AddForce(-transform.up * _tankSpeed);
        }
        if (Input.GetKey(KeyCode.S))    // 후진 이동.
        {
            _rigid.AddForce(transform.up * _tankSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0.0f, 0.0f, _bodyRotateSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0.0f, 0.0f, -_bodyRotateSpeed);
        }
    }

    IEnumerator _reloadBullet()
    {
        yield return new WaitForSeconds(_shootDelay);
        _isShoot = false;
    }

	void OnTriggerEnter2D(Collider2D other)
    {
		if (other.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Tank Trigger Entered!");
            if (_photonView.viewID == other.gameObject.GetComponent<Bullet>().GetOwner())
				return;

            Debug.Log("Hit by other bullet!");
            var damage = other.gameObject.GetComponent<Bullet>().GetDamage();
            _photonView.RPC("DamageHP", PhotonTargets.All, damage, _photonView.viewID);
            Debug.Log(_hp);
        }
    }

    [PunRPC]
    void DamageHP(int damage, int viewID)
    {
        if (_photonView.viewID == viewID)
            _hp -= damage;
    }

	//void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
   // {
   //     if (stream.isWriting)
   //     {
   //         //We own this player: send the others our data
   //         stream.SendNext(transform.position);
   //         stream.SendNext(transform.rotation);
   //     }
   //     else
   //     {
   //         //Network player, receive data
			//_curPos = (Vector3)stream.ReceiveNext();
			//_curQuat = (Quaternion)stream.ReceiveNext();
    //    }
    //}
}
