using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 10.0f;
    private Rigidbody2D _rigid;
    private static GameObject _explosion = null;

    void Awake()
    {
        _explosion = Resources.Load<GameObject>("Prefabs/Explosion");
    }
    
    void Start ()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.velocity = transform.right.normalized * _bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("FieldObject"))
        {
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        //else if (other.gameObject.CompareTag("Tank"))
        //{
        //    Instantiate(_explosion, transform.position, Quaternion.identity);
        //    Destroy(gameObject);
        //}
        else if (other.gameObject.CompareTag("Wall"))
        {
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
