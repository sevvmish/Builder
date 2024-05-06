using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationZSimple : MonoBehaviour
{
    [SerializeField] private float timerForFullCircle = 6;

    private void OnEnable()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.Append(transform.DORotate(transform.localEulerAngles + new Vector3(0, 0, 360), timerForFullCircle, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
    }
}
