using Photon;
using UnityEngine;

public class HPItem : PunBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _HPIncrease = 40.0f;

    public float HPIncrease
    {
        get { return _HPIncrease; }
    }

    void Start ()
	{
	    _collider.enabled = false;
	}

    public void EnableItem()
    {
        _collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.isMine)
            return;

        if (other.CompareTag("Tank"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
