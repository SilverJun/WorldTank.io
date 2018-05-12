using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 10.0f;
    private Rigidbody2D _rigid;

    // Use this for initialization
    void Start ()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.velocity = transform.right.normalized * _bulletSpeed;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.CompareTag("FieldObject"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Tank"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
