using System.Collections;
using DG.Tweening;
using Photon;
using UnityEngine;
using UnityEngine.UI;

public class HPItem : Photon.MonoBehaviour, IPunObservable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private int _HPIncrease = 40;
    [SerializeField] private Image _image;

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

        if (other.CompareTag("PlayerTank"))
        {
            other.GetComponent<Tank>().Hp += _HPIncrease;

            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void GenAnim(float time)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_image.DOFillAmount(1.0f, time));
        seq.AppendCallback(() =>
        {
            EnableItem();
        });

        seq.Play();
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
