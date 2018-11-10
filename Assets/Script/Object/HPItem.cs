using System.Collections;
using DG.Tweening;
using Photon;
using UnityEngine;
using UnityEngine.UI;

public class HPItem : PunBehaviour, IPunObservable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private int _HPIncrease = 40;
    [SerializeField] private Image _image;
    [SerializeField] private float _genTime = 30.0f;
    private float _animPerFrame = 0.0f;

    public float HPIncrease
    {
        get { return _HPIncrease; }
    }

    public void Start ()
	{
	    //_collider.enabled = false;
	    _animPerFrame = 1.0f / (_genTime * Application.targetFrameRate);    // 크기 0-1부터, 0초 ~ 30초 (1초에 60번)
        Debug.Log(_animPerFrame);
	    //StartCoroutine(EnableItem());
	}

    public void FixedUpdate()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (_image.fillAmount >= 1.0f)
            return;

        _image.fillAmount += _animPerFrame;
    }

    //public IEnumerator EnableItem()
    //{
    //    yield return new WaitUntil(()=> _image.fillAmount >= 1.0f);
    //    _collider.enabled = true;
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (!photonView.isMine)
        //    return;

        if (_image.fillAmount < 1.0f)
            return;

        if (other.CompareTag("PlayerTank") || other.CompareTag("EnemyTank"))
        {
            other.GetComponent<Tank>().Hp += _HPIncrease;

            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(_image.fillAmount);
            stream.SendNext(_collider.enabled);
        }
        else
        {
            _image.fillAmount = (float)stream.ReceiveNext();
            _collider.enabled = (bool)stream.ReceiveNext();
        }
    }
}
