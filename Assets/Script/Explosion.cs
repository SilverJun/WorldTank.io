using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void DestroySelf()
    {
		if (PhotonNetwork.isMasterClient)  
		    PhotonNetwork.Destroy(gameObject);
    }
}
