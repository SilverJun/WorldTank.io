using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tank : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _hpObject;
    private HP _hpScript;
    [SerializeField] private GameObject _barrel;
    [SerializeField] private GameObject _body;

    private Rigidbody2D _rigid;
    private GameObject _bullet;

    private Vector3 _worldBarrelPos;

    // data
    [SerializeField] private const int _maxHP = 100;
    [SerializeField] private int _hp = 100;
    [SerializeField] private float _barrelRotateSpeed = 1.0f;
    [SerializeField] private float _bodyRotateSpeed = 1.0f;
    [SerializeField] private float _tankSpeed = 5.0f;
    [SerializeField] private float _shootDelay = 1.0f;
    private bool _isShoot = false;
    private bool _isDie = false;

    void Awake()
    {
        _bullet = Resources.Load<GameObject>("Prefabs/Bullet");
    }
    
    void Start ()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _hpScript = _hpObject.GetComponent<HP>();
        _hpScript.SetHP(_maxHP);
    }
	
	void FixedUpdate ()
	{
	    if (_isDie)
	        return;

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
	        Instantiate(_bullet, transform.position, Quaternion.Euler(0.0f, 0.0f, _barrel.transform.eulerAngles.z - 90.0f));
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            var damage = other.gameObject.GetComponent<Bullet>().GetDamage();
            _hp -= damage;
            _hpScript.UpdateHP(_hp);
        }
    }
}
