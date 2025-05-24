using UnityEngine;
using UnityEngine.EventSystems;

public class TemperatureButtonUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool deactivateButton;
    public bool buttonHeld;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(!deactivateButton)
            buttonHeld = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonHeld = false;
    }
}