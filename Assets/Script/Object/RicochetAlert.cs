using DG.Tweening;
using Photon;
using TMPro;

public class RicochetAlert : PunBehaviour
{
	void Start ()
	{
	    Sequence seq = DOTween.Sequence();
	    var textMesh = GetComponent<TextMeshPro>();
	    seq.Append(textMesh.DOFade(0.0f, 0.5f));
        seq.Join(transform.DOLocalMoveY(0.2f, 0.5f));
	    seq.AppendCallback(() =>
	    {
	        if (PhotonNetwork.isMasterClient || photonView.isMine)
	            PhotonNetwork.Destroy(gameObject);
        });
	    seq.Play();
	}
}
