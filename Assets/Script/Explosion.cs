using Photon;
using UnityEngine;

public class Explosion : PunBehaviour
{
    public void DestroySelf()
    {
        if (PhotonNetwork.isMasterClient || photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
