using UnityEngine;
using UnityEngine.EventSystems;

public class WaterCanMask : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public RectTransform waterCan;
    private Vector2 startPosi;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        waterCan.anchoredPosition = new Vector2(Random.Range(-950f, 950f), Random.Range(-540f, 540f));
        startPosi = waterCan.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        if (rectTransform.anchoredPosition.x < -960 || rectTransform.anchoredPosition.x > 960 || rectTransform.anchoredPosition.y < -540 || rectTransform.anchoredPosition.y > 540)
            rectTransform.anchoredPosition = startPosition;

        waterCan.position = startPosi;
    }
}