using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 10.0f;
    [SerializeField] private int _damage = 40;
	private GameObject _owner;
    private Rigidbody2D _rigid;
    private static GameObject _explosion = null;

    void Awake()
    {
        //_explosion = Resources.Load<GameObject>("Prefabs/Explosion");
    }
    
    void Start ()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.velocity = transform.right.normalized * _bulletSpeed;
    }

	public void SetOwner(GameObject owner)
	{
		_owner = owner;
	}

	public GameObject GetOwner() { return _owner; }

    public int GetDamage() { return _damage; }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (_owner == other.gameObject)
			return;

		if (other.gameObject.CompareTag("FieldObject"))
		{
			PhotonNetwork.Instantiate("Prefabs/Explosion", transform.position, Quaternion.identity, 0);
			Destroy(gameObject);
		}
		else if (other.gameObject.CompareTag("Tank"))
		{
			Instantiate(_explosion, transform.position, Quaternion.identity);
		}
		else if (other.gameObject.CompareTag("Wall"))
		{
			PhotonNetwork.Instantiate("Prefabs/Explosion", transform.position, Quaternion.identity, 0);
			Destroy(gameObject);
		}
	}
}
