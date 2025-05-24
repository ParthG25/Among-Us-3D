using UnityEngine;
using UnityEngine.EventSystems;

public class Keys : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Vector2 position;
    public bool initialized;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        position = rectTransform.anchoredPosition;
        canvasGroup = GetComponent<CanvasGroup>();
        initialized = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
