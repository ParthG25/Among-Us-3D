using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FilterLeaf : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public TaskCleanFilter taskCleanFilter;
    private bool leafReached;

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
        float x = rectTransform.anchoredPosition.x;

        if (x < -150f)
        {
            taskCleanFilter.count++;
            GetComponent<Image>().raycastTarget = false;
            leafReached = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        if (rectTransform.anchoredPosition.x < -960 || rectTransform.anchoredPosition.x > 960 || rectTransform.anchoredPosition.y < -540 || rectTransform.anchoredPosition.y > 540)
            rectTransform.anchoredPosition = startPosition;
    }

    private void Update()
    {
        if(leafReached)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, new Vector2(-540f, 0f), 150f * Time.deltaTime);
            if (rectTransform.anchoredPosition == new Vector2(-540f, 0f))
                gameObject.SetActive(false);
        }
    }
}