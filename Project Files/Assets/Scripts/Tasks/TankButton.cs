using UnityEngine;
using UnityEngine.EventSystems;

public class TankButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonHeld;
    public GameObject greenLightOn, redLightOn;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonHeld = true;
        greenLightOn.SetActive(true);
        redLightOn.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonHeld = false;
        greenLightOn.SetActive(false);
        redLightOn.SetActive(true);
    }
}