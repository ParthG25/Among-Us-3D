using UnityEngine;
using UnityEngine.EventSystems;

public class Artifact : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Vector2 position;
    public ArtifactSocket artifactSocket;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        position = rectTransform.anchoredPosition;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!artifactSocket.artifactReceived)
        {
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!artifactSocket.artifactReceived)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            rectTransform.anchoredPosition = position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}