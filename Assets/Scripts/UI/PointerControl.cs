using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerControl : MonoBehaviour, IPointerDownHandler
{
    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gm.PointerClickedCount = 0.2f;
    }

}
