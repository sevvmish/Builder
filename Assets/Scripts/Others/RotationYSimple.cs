using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationYSimple : MonoBehaviour
{
    [SerializeField] private float timerForFullCircle = 10;

    private void OnEnable()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.Append(transform.DORotate(transform.eulerAngles + new Vector3(0, 360, 0), timerForFullCircle, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
    }
}
