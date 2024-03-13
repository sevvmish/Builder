using UnityEngine;
using UnityEngine.EventSystems;

public class PointerBase : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
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

    private bool isPointerEnter;
    public bool IsPointerEnter
    {
        get
        {
            return isPointerEnter;
        }

        private set { isPointerEnter = value; }
    }

    private bool isPointerUp;
    public bool IsPointerUp
    {
        get
        {
            if (isPointerUp)
            {
                isPointerUp = false;
                return true;
            }
            else
            {
                return isPointerUp;
            }
        }

        private set { isPointerUp = value; }
    }



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

    private string name;

    private void Start()
    {
        name = gameObject.name;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        deltaPosition = eventData.delta;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerUp = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {        
        isPointerEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerEnter = false;
    }
}
