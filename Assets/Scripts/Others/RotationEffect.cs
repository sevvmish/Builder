using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationEffect : MonoBehaviour
{
    [SerializeField] private float speedTime = 3;
    [SerializeField] private bool axisX;
    [SerializeField] private bool axisY;
    [SerializeField] private bool axisZ;

    
    private void OnEnable()
    {        
        if (axisX)
        {
            transform.DORotate(new Vector3(
                    360,
                    0,
                    0), speedTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        }
        else if (axisY)
        {
            transform.DORotate(new Vector3(
                    0,
                    360,
                    0), speedTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        }
        else if (axisZ)
        {
            transform.DORotate(new Vector3(
                    0,
                    0,
                    360), speedTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        }        
    }

}
