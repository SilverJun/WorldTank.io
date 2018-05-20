using UnityEngine;

public class Bullet : Photon.PunBehaviour
{
    [SerializeField] private float _bulletSpeed = 10.0f;
    [SerializeField] private int _damage = 40;
	private GameObject _owner;
    private Rigidbody2D _rigid;
    private PhotonView _photonView;

    void Start ()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.velocity = transform.right.normalized * _bulletSpeed;
        _photonView = GetComponent<PhotonView>();
        Debug.Log("Bullet Owner");
        Debug.Log(NetworkManager.Tank);
        _owner = NetworkManager.Tank;

        if (_photonView.isMine)
            GetComponent<CapsuleCollider2D>().enabled = true;
    }
    
	public GameObject GetOwner() { return _owner; }

    public int GetDamage() { return _damage; }

	void OnTriggerEnter2D(Collider2D other)
    {
        if (!_photonView.isMine)
            return;

        if (_owner.gameObject == other.gameObject)
			return;

        Debug.Log("Collision!----");
        Debug.Log("Owner");
        Debug.Log(_owner.gameObject);
        Debug.Log("Other");
        Debug.Log(other.gameObject);
        Debug.Log("--------------");

        if (other.gameObject.CompareTag("FieldObject"))
		{
			PhotonNetwork.Instantiate("Prefabs/Explosion", transform.position, Quaternion.identity, 0);
		    PhotonNetwork.Destroy(gameObject);
		}
		else if (other.gameObject.CompareTag("Tank"))
		{
            Debug.Log("Tank Hit!");
		    PhotonNetwork.Instantiate("Prefabs/Explosion", transform.position, Quaternion.identity, 0);
            PhotonNetwork.Destroy(gameObject);
		}
		else if (other.gameObject.CompareTag("Wall"))
		{
			PhotonNetwork.Instantiate("Prefabs/Explosion", transform.position, Quaternion.identity, 0);
		    PhotonNetwork.Destroy(gameObject);
		}
	}
}
