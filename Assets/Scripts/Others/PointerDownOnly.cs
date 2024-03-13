using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDownOnly : MonoBehaviour, IPointerDownHandler
{
    private bool isPressed;
    public bool IsPressed
    {
        get
        {
            if (isPressed)
            {
                isPressed = false;
                return true;
            }
            else
            {
                return isPressed;
            }
        }

        private set { isPressed = value; }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
}
