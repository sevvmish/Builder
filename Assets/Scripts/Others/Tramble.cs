using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tramble : MonoBehaviour
{
    [SerializeField] private float deltaX = 0.1f;
    [SerializeField] private bool isRandomX;

    [SerializeField] private float deltaY = 0.1f;
    [SerializeField] private bool isRandomY;

    [SerializeField] private float deltaZ = 0.1f;
    [SerializeField] private bool isRandomZ;

    [SerializeField] private float timer = 0.3f;
    [SerializeField] private int vibrato = 30;

    private float _timer;    
    private float _startCooldown;

    private float x = 0, y = 0, z = 0;

    // Start is called before the first frame update
    void Start()
    {
        _startCooldown = UnityEngine.Random.Range(0, 0.75f);


        /*
        
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, 0.75f));

        while (true)
        {
            if (isRandomX)
            {
                x = UnityEngine.Random.Range(-deltaX, deltaX);
            }
            else
            {
                x = deltaX;
            }

            if (isRandomY)
            {
                y = UnityEngine.Random.Range(-deltaY, deltaY);
            }
            else
            {
                y = deltaY;
            }

            if (isRandomZ)
            {
                z = UnityEngine.Random.Range(-deltaZ, deltaZ);
            }
            else
            {
                z = deltaZ;
            }

            transform.DOPunchPosition(new Vector3(x, y, z), timer, vibrato).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(1f);
        }        */
    }

    private void Update()
    {
        if (_startCooldown > 0)
        {
            _startCooldown -= Time.deltaTime;
            return;
        }

        if (_timer > 1)
        {
            _timer = 0;

            if (isRandomX)
            {
                x = UnityEngine.Random.Range(-deltaX, deltaX);
            }
            else
            {
                x = deltaX;
            }

            if (isRandomY)
            {
                y = UnityEngine.Random.Range(-deltaY, deltaY);
            }
            else
            {
                y = deltaY;
            }

            if (isRandomZ)
            {
                z = UnityEngine.Random.Range(-deltaZ, deltaZ);
            }
            else
            {
                z = deltaZ;
            }

            transform.DOPunchPosition(new Vector3(x, y, z), timer, vibrato).SetEase(Ease.OutQuad);
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }


}
