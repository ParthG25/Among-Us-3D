using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TelescopeTargetItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public TaskAlignTelescope taskAlignTelescope;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float x = taskAlignTelescope.items[taskAlignTelescope.selectedItem].GetComponent<RectTransform>()
            .anchoredPosition.x;
        float y = taskAlignTelescope.items[taskAlignTelescope.selectedItem].GetComponent<RectTransform>()
            .anchoredPosition.y;
        if (x > -346.5 && x < 346.5 && y > -177.5 && y < 177.5)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<RectTransform>().anchoredPosition -= new Vector2(x, y);
            }
            taskAlignTelescope.taskCompleted = true;
            GetComponent<Image>().raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<RectTransform>().anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
}