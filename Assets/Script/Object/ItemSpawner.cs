using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : PunBehaviour
{
    [SerializeField] private GameObject _hpItem;
    
    private Image _cooltimeImage;

    private void Start()
    {
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(5.0f);

        /// 자신이 마스터 클라이언트가 아니면 코루틴을 종료시킨다. (아이템 겹쳐서 생성되는거 방지.)
        if (!PhotonNetwork.isMasterClient)
            yield break;

        _hpItem = PhotonNetwork.Instantiate("Prefabs/HPItem", transform.position, Quaternion.identity, 0);

        _hpItem.GetComponent<HPItem>().GenAnim(30.0f);

        yield return new WaitWhile(() => _hpItem != null);
        StartCoroutine(Setup());
    }

    
}
