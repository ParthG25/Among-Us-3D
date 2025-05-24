using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactSocket : MonoBehaviour, IDropHandler
{
    public bool artifactReceived;
    
    public void OnDrop(PointerEventData eventData)
    {
        if (gameObject.name.IndexOf(eventData.pointerDrag.name) != -1)
        {
            eventData.pointerDrag.GetComponent<Image>().raycastTarget = false;
            eventData.pointerDrag.GetComponent<CanvasGroup>().alpha = 1f;
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;
            artifactReceived = true;
        }
    }
}