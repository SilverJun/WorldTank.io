﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using DG.Tweening;

public class Tank : Photon.MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _barrel;
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _fireEffect;
	[SerializeField] private AudioSource _audio;

    [SerializeField] private EdgeCollider2D _up;
    [SerializeField] private EdgeCollider2D _left;
    [SerializeField] private EdgeCollider2D _right;
    [SerializeField] private EdgeCollider2D _down;

    private Rigidbody2D _rigid;

    private Vector3 _worldBarrelPos;

	private PhotonView _photonView;

    // data
    [SerializeField] private float _barrelRotateSpeed = 10.0f;
    [SerializeField] private float _bodyRotateSpeed = 1.0f;
    [SerializeField] private float _tankSpeed = 5.0f;
    [SerializeField] private float _shootDelay = 1.0f;
    [SerializeField] private int _hp;
    [SerializeField] private int _maxHP = 100;
	[SerializeField] private float _missRatio = 10.0f;
    private bool _isShoot;
    private bool _isDie;

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
        _fireEffect.SetActive(false);
    }

    void OnDestroy()
    {
        PhotonNetwork.Instantiate("Prefabs/Explosion", transform.position, Quaternion.identity, 0);
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

		if (!_isDie && _hp <= 0)
	    {
			_isDie = true;
			UIManager.OpenUI<RespawnUI>("Prefabs/RespawnUI");
	        return;
	    }

        _camera.transform.position = new Vector3(transform.position.x, transform.position.y, _camera.transform.position.z);

        LookBarrelMouse();
	    MoveTank();

	    // Left Mouse Click
	    if (!_isShoot && Input.GetMouseButtonDown(0))
	    {
            // 총알이랑 바렐이랑 보고있는 방향이 달라서 차이값만큼 보정
	        _fireEffect.SetActive(true);
			_audio.Play();

            PhotonNetwork.Instantiate("Prefabs/Bullet", transform.position, Quaternion.Euler(0.0f, 0.0f, _barrel.transform.eulerAngles.z - 90.0f), 0);
	        _isShoot = true;
	        StartCoroutine(_fireEffectDisable());
            StartCoroutine(_reloadBullet()); // 재장전
	    }      
	}

    void LookBarrelMouse()
    {
        _worldBarrelPos = Camera.main.WorldToScreenPoint(_barrel.transform.position);
        var dir = Input.mousePosition - _worldBarrelPos;

		_barrel.transform.rotation = Quaternion.Lerp(_barrel.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90.0f), Time.deltaTime * _barrelRotateSpeed);
        //_barrel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90.0f));
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

    IEnumerator _fireEffectDisable()
    {
        yield return new WaitForSeconds(0.1f);
        _fireEffect.SetActive(false);
    }

    bool CheckRicochet(Collision2D bullet)
    {
        /// TODO : 도탄시스템 새로 구축.
        /// 
        /// 1. 탄환과 몸체의 각이 30도 이상될때 무조건 도탄된다.
        /// 2. 탄환과 몸체의 각이 수직 ~ 70도 무조건 타격.
        /// 3. 탄환과 몸체의 각 30~70도는 랜덤확률로 도탄.

        var bulletFront = bullet.gameObject.transform.right;
        var tankUp = gameObject.transform.up;
        var tankRight = gameObject.transform.right;

        if (bullet.otherCollider == _up)
        {
            Debug.Log("Up!");
        }
        else if (bullet.otherCollider == _down)
        {
            Debug.Log("Down!");
        }
        else if (bullet.otherCollider == _left)
        {
            Debug.Log("Left!");
        }
        else if (bullet.otherCollider == _right)
        {
            Debug.Log("Right!");
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("HPItem"))
        {
            _hp += (int)other.gameObject.GetComponent<HPItem>().HPIncrease;
            Debug.Log(_hp);
            if (_hp > 100)
                _hp = 100;

            return;
        }

        if (!other.gameObject.CompareTag("Bullet"))
            return;
        if (_photonView.viewID == other.gameObject.GetComponent<Bullet>().GetOwner())
            return;
        if (CheckRicochet(other))
            return;

        Debug.Log("Hit by other bullet!");
        var damage = other.gameObject.GetComponent<Bullet>().GetDamage();
        _photonView.RPC("DamageHP", PhotonTargets.All, damage, _photonView.viewID);

        /// 이 탄의 주인이 이 클라이언트 탱크면 이 클라이언트 탱크의 킬수를 업데이트 함
        if (_hp <= 0 && NetworkManager.Tank.GetPhotonView().viewID == other.gameObject.GetComponent<Bullet>().GetOwner())
        {
            /// KillUP!
            NetworkManager.Kill++;
        }

        Debug.Log(_hp);
    }

  //  void OnTriggerEnter2D(Collider2D other)
  //  {
  //      if (other.CompareTag("HPItem"))
  //      {
  //          _hp += (int)other.gameObject.GetComponent<HPItem>().HPIncrease;
  //          Debug.Log(_hp);
  //          if (_hp > 100)
  //              _hp = 100;

  //          return;
  //      }
        
  //      if (!other.CompareTag("Bullet"))
		//	return;
		//if (_photonView.viewID == other.gameObject.GetComponent<Bullet>().GetOwner())
  //          return;
  //      if (CheckRicochet(other))
  //          return;

  //      Debug.Log("Hit by other bullet!");
  //      var damage = other.gameObject.GetComponent<Bullet>().GetDamage();
		//_photonView.RPC("DamageHP", PhotonTargets.All, damage, _photonView.viewID);

  //      /// 이 탄의 주인이 이 클라이언트 탱크면 이 클라이언트 탱크의 킬수를 업데이트 함
		//if (_hp <= 0 && NetworkManager.Tank.GetPhotonView().viewID == other.gameObject.GetComponent<Bullet>().GetOwner())
		//{
		//	/// KillUP!
		//	NetworkManager.Kill++;
		//}

  //      Debug.Log(_hp);
  //  }

    [PunRPC]
    void DamageHP(int damage, int viewID)
    {
        if (_photonView.viewID == viewID)
            _hp -= damage;
    }

}
