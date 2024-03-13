using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerMoveOnly : MonoBehaviour, IPointerMoveHandler
{
    private Vector2 deltaPosition;
    public Vector2 DeltaPosition
    {
        get
        {
            if (deltaPosition != Vector2.zero)
            {
                Vector2 result = deltaPosition;
                deltaPosition = Vector2.zero;
                return result;
            }
            else
            {
                return deltaPosition;
            }
        }

        private set { deltaPosition = value; }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        deltaPosition = eventData.delta;
    }
}
