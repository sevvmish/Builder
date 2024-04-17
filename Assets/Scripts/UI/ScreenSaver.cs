using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSaver : MonoBehaviour
{
    public static ScreenSaver Instance { get; private set; }

    [SerializeField] private RectTransform screen;

    private readonly float additionalWait = 0.3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        screen.anchoredPosition = Vector2.zero;
    }

    public void HideScreen()
    {
        screen.gameObject.SetActive(true);
        screen.anchoredPosition = new Vector2(-2000, 2000);
        screen.DOAnchorPos3D(Vector2.zero, Globals.SCREEN_SAVER_AWAIT).SetEase(Ease.InOutQuad);
    }

    public void FastShowScreen()
    {        
        StartCoroutine(deactivateAfter(0));
    }

    public void ShowScreen()
    {
        screen.gameObject.SetActive(true);
        screen.anchoredPosition = Vector2.zero;
        screen.DOAnchorPos3D(new Vector2(2000, -2000), Globals.SCREEN_SAVER_AWAIT).SetEase(Ease.InOutQuad);

        StartCoroutine(deactivateAfter(Globals.SCREEN_SAVER_AWAIT));
    }

    private IEnumerator deactivateAfter(float secs)
    {
        yield return new WaitForSeconds(secs);
        screen.gameObject.SetActive(false);
    }


}
