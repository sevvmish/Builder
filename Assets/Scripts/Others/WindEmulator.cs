using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEmulator : MonoBehaviour
{
    [SerializeField] private float deltaX = 4;
    [SerializeField] private float deltaOnX = 0.3f;
    [SerializeField] private float deltaY = 0;
    [SerializeField] private float deltaOnY = 0;
    [SerializeField] private float deltaZ = 2;
    [SerializeField] private float deltaOnZ = 0.2f;
    [SerializeField] private float _time = 5;
    [SerializeField] private Ease _ease = Ease.InOutSine;

    private bool isStart;
    private Vector3 from, to;

    // Start is called before the first frame update
    void Start()
    {
        deltaX += UnityEngine.Random.Range(-deltaOnX, deltaOnX);
        deltaY += UnityEngine.Random.Range(-deltaOnY, deltaOnY);
        deltaZ += UnityEngine.Random.Range(-deltaOnZ, deltaOnZ);

        int rnd = UnityEngine.Random.Range(0, 2);

        switch(rnd)
        {
            case 0:
                from = transform.localEulerAngles + new Vector3(deltaX, deltaY, deltaZ);
                to = transform.localEulerAngles + new Vector3(-deltaX, -deltaY, -deltaZ);
                break;

            case 1:
                to = transform.localEulerAngles + new Vector3(deltaX, deltaY, deltaZ);
                from = transform.localEulerAngles + new Vector3(-deltaX, -deltaY, -deltaZ);
                break;
        }

                
        transform.localEulerAngles = from;
        Invoke("play", UnityEngine.Random.Range(0, 0.9f));
    }

    private void play()
    {        
        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(UpdateType.Fixed);

        sequence.SetLoops(-1, LoopType.Restart);

        sequence.Append(transform.DOLocalRotate(to, _time).SetEase(_ease));
        sequence.Append(transform.DOLocalRotate(from, _time).SetEase(_ease));
    }
        
}
