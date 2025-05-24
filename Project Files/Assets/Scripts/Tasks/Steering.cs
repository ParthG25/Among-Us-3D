using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Steering : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public TaskStabilizeSteering taskStabilizeSteering;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(Random.Range(-950f, 950f), Random.Range(-540f, 540f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float x = rectTransform.anchoredPosition.x;
        float y = rectTransform.anchoredPosition.y;
      
        if (x > -40 && x < 40 && y > -40 && y < 40)
        { 
            rectTransform.anchoredPosition -= new Vector2(x, y);
            
            taskStabilizeSteering.taskCompleted = true;
            GetComponent<Image>().raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}