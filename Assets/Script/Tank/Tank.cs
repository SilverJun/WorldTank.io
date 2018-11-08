using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tank : Photon.MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _barrel;
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _fireEffect;
    [SerializeField] private Transform _firePos;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private ParticleSystem _smoke;
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
    [SerializeField] private float _missRatio = 20.0f;
    [SerializeField] private float _minimumAngle = 20.0f;
    [SerializeField] private float _maximumAngle = 80.0f;

    private bool _isShoot;
    private bool _isDie;

    public int Hp
    {
        get { return _hp; }
        set
        {
            _hp = value;

            if (_hp > _maxHP)
                _hp = _maxHP;
            else if (_hp <= 0)
                _isDie = true;
            else if (_hp <= 40)
                _smoke.Play();
            else
                _smoke.Stop();
        }
    }

    private void Awake()
    {
        Hp = _maxHP;
    }

    void Start()
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

    void FixedUpdate()
    {
        if (!_photonView.isMine)
        {
            //transform.position = _curPos;
            //transform.rotation = _curQuat;
            return;
        }

        if (_isDie)
        {
            UIManager.OpenUI<RespawnUI>("Prefabs/RespawnUI");
            PhotonNetwork.Destroy(gameObject);
        }

        _camera.transform.position =
            new Vector3(transform.position.x, transform.position.y, _camera.transform.position.z);

        LookBarrelMouse();
        MoveTank();

        // Left Mouse Click
        if (!_isShoot && Input.GetMouseButtonDown(0))
        {
            // 총알이랑 바렐이랑 보고있는 방향이 달라서 차이값만큼 보정
            _fireEffect.SetActive(true);
            _audio.Play();

            PhotonNetwork.Instantiate("Prefabs/Bullet", _firePos.position,
                Quaternion.Euler(0.0f, 0.0f, _barrel.transform.eulerAngles.z - 90.0f), 0);
            _isShoot = true;
            StartCoroutine(_fireEffectDisable());
            StartCoroutine(_reloadBullet()); // 재장전
        }
    }

    void LookBarrelMouse()
    {
        _worldBarrelPos = Camera.main.WorldToScreenPoint(_barrel.transform.position);
        var dir = Input.mousePosition - _worldBarrelPos;

        _barrel.transform.rotation = Quaternion.Lerp(_barrel.transform.rotation,
            Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90.0f),
            Time.deltaTime * _barrelRotateSpeed);
        //_barrel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90.0f));
    }

    void MoveTank()
    {
        if (Input.GetKey(KeyCode.W)) // 전진 이동.
        {
            _rigid.AddForce(-transform.up * _tankSpeed);
        }

        if (Input.GetKey(KeyCode.S)) // 후진 이동.
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
        /// 도탄시스템 알고리즘
        /// 
        /// 1. 어떤 콜라이더에 충돌했는지 확인.
        /// 2. 탄의 진행방향에 따른 탱크의 입사각을 구한다.
        /// 3. 입사각에 따라 알맞은 확률을 적용한다.
        /// 

        var bulletFront = bullet.gameObject.transform.right;
        var tankUp = -gameObject.transform.up;
        var tankRight = gameObject.transform.right;
        var theta = 0.0f;

        if (bullet.otherCollider == _up)
        {
            Debug.Log("Up!");
            /// 탄이 탱크 위에 맞을 경우, 충돌판정 기준
            /// red = 오른쪽, green = 아래.
            /// 따라서 수평인 탱크의 right와 탄의 right의 내적을 이용해 탄의 입사각을 구한다.
            theta = Mathf.Acos(Vector3.Dot(tankRight, bulletFront) / (tankRight.magnitude * bulletFront.magnitude));
        }
        else if (bullet.otherCollider == _down)
        {
            Debug.Log("Down!");
            theta = Mathf.Acos(Vector3.Dot(-tankRight, bulletFront) / (-tankRight.magnitude * bulletFront.magnitude));
        }
        else if (bullet.otherCollider == _left)
        {
            Debug.Log("Left!");
            theta = Mathf.Acos(Vector3.Dot(tankUp, bulletFront) / (tankUp.magnitude * bulletFront.magnitude));
        }
        else if (bullet.otherCollider == _right)
        {
            Debug.Log("Right!");
            theta = Mathf.Acos(Vector3.Dot(-tankUp, bulletFront) / (-tankUp.magnitude * bulletFront.magnitude));
        }

        theta = Mathf.Rad2Deg * theta;
        theta = Mathf.Abs(Mathf.Min(theta, 180.0f - theta));

        Debug.Log(theta);

        /// 현재각이 최대 각보다 클 경우 무조건 적중
        if (theta > _maximumAngle)
            return false;
        /// 현재각이 최소 각보다 작을 경우 무조건 도탄
        if (theta <= _minimumAngle)
            return true;
        if (Random.Range(0.0f, 100.0f) <= _missRatio) /// 랜덤확률로 도탄.
            return true;

        return false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var bullet = other.gameObject.GetComponent<Bullet>();

        Debug.Log("OnCollisionEnter2D");
        if (!other.gameObject.CompareTag("Bullet"))
        {
            return;
        }

        if (_photonView.viewID == bullet.GetOwner())
        {
            Debug.Log("_photonView.viewID == bullet.GetOwner()");
            return;
        }

        if (bullet.IsAlreadyChecked)
        {
            Debug.Log("bullet.IsAlreadyChecked");
            return;
        }

        if (!PhotonNetwork.isMasterClient)
            return;

        if (CheckRicochet(other))
        {
            /// 탄 중복충돌 방지.
            bullet.DisableBullet();
            /// 도탄 알림.
            Debug.Log("도탄되었습니다.");
            PhotonNetwork.Instantiate("Prefabs/RicochetAlert", transform.position, Quaternion.identity, 0);
            return;
        }

        Debug.Log("Hit by other bullet!");

        /// 탄 중복충돌 방지.
        bullet.DisableBullet();

        var damage = bullet.GetDamage();
        _photonView.RPC("DamageHP", PhotonTargets.All, damage, _photonView.viewID);


        /// 이 탄의 주인이 이 클라이언트 탱크면 이 클라이언트 탱크의 킬수를 업데이트 함
        if (Hp <= 0 && NetworkManager.Tank.GetPhotonView().viewID == bullet.GetOwner())
        {
            /// KillUP!
            NetworkManager.Kill++;
        }

        Debug.Log(Hp);
    }

    [PunRPC]
    void DamageHP(int damage, int viewID)
    {
        if (_photonView.viewID == viewID)
            Hp -= damage;
    }

}
