using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanisterSlot : MonoBehaviour, IDropHandler
{
    public bool canisterReceived;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<Image>().raycastTarget = false;
            eventData.pointerDrag.GetComponent<CanvasGroup>().alpha = 1f;
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;
            canisterReceived = true;
        }
    }
}