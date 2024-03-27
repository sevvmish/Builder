using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBlink : MonoBehaviour
{
    [SerializeField] private float timer = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.SetUpdate(UpdateType.Fixed);

        sequence.Append(transform.DOScale(Vector3.one * 0.2f, timer).SetEase(Ease.Linear));
        sequence.Append(transform.DOScale(Vector3.one, timer).SetEase(Ease.Linear));
    }

    
}
