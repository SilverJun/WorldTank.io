using UnityEngine;

public class Bullet : Photon.PunBehaviour
{
    [SerializeField] private float _bulletSpeed = 10.0f;
    [SerializeField] private int _damage = 10;
    private int _viewID = -1;
    private Rigidbody2D _rigid;
    private PhotonView _photonView;

    /// <summary>
    /// 이미 탄이 한번 맞아서 체크되었는지 알고있는 변수
    /// </summary>
    public bool IsAlreadyChecked
    {
        get;
        set;
    }

    void Start ()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.velocity = transform.right.normalized * _bulletSpeed;
        _photonView = GetComponent<PhotonView>();
        IsAlreadyChecked = false;

        if (_photonView.isMine)
        {
            _viewID = _photonView.ownerId;
            _photonView.RPC("SetOwner", PhotonTargets.All, _viewID);
        }
    }

    [PunRPC]
    public void SetOwner(int viewID)
    {
        _viewID = viewID;
    }

	public int GetOwner() { return _viewID; }

    public int GetDamage() { return _damage; }

    public void DisableBullet()
    {
        IsAlreadyChecked = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_photonView.isMine)
            return;

        PhotonNetwork.Instantiate("Prefabs/Explosion", transform.position, Quaternion.identity, 0);
        PhotonNetwork.Destroy(gameObject);
    }
}
