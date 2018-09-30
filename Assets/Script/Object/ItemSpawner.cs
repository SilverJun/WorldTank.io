using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
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

        _hpItem = PhotonNetwork.Instantiate("Prefabs/HPItem", transform.position, Quaternion.identity, 0);
        _cooltimeImage = _hpItem.GetComponent<Image>();

        yield return StartCoroutine(GenItem(30.0f));
    }

    IEnumerator GenItem(float time)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_cooltimeImage.DOFillAmount(1.0f, time));
        seq.AppendCallback(()=>
        {
            _hpItem.GetComponent<HPItem>().EnableItem();
        });

        yield return new WaitWhile(() => _hpItem != null);
        StartCoroutine(Setup());
    }
}
