using UnityEngine;
using UnityEngine.EventSystems;

public class JugButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonHeld;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonHeld = false;
    }
}
