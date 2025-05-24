using UnityEngine;
using UnityEngine.EventSystems;

public class Sample : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public float startX, endX, startY, endY;
    public bool inPlace;

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
        float x = rectTransform.anchoredPosition.x, y = rectTransform.anchoredPosition.y;

        if (x > startX && x < endX && y < endY && y > startY)
            inPlace = true;
        else
            inPlace = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}