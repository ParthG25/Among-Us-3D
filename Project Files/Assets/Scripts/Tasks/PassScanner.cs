using UnityEngine;
using UnityEngine.EventSystems;

public class PassScanner : MonoBehaviour, IDropHandler
{ 
    public bool passReceived;
    
    public void OnDrop(PointerEventData eventData)    
    {
        if (eventData.pointerDrag != null)
        {
            passReceived = true;
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;
        }
    }
}