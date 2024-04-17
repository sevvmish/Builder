using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrambleUI : MonoBehaviour
{
    private void OnDisable()
    {
        StopAllCoroutines();
    }


    private void OnEnable()
    {
        StartCoroutine(play());
    }
    private IEnumerator play()
    {
        while (true)
        {
            transform.DOPunchPosition(new Vector3(10, 10, 1), 0.5f, 30).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(2);
            transform.DOPunchScale(new Vector3(0.3f, 0.2f, 1), 0.5f, 30).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(2);
        }
    }

}
