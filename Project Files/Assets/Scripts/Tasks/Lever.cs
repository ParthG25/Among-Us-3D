using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lever : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public bool leverHeld;

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
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -25f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(rectTransform.anchoredPosition.y <= -25 && rectTransform.anchoredPosition.y >= -380)
        {
            rectTransform.anchoredPosition += new Vector2(0, eventData.delta.y / canvas.scaleFactor);
            if(rectTransform.anchoredPosition.y > -25)
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -25f);
            if(rectTransform.anchoredPosition.y < -380)
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -380f);
            if (rectTransform.anchoredPosition.y == -380f)
                StartCoroutine(StartTimer());
        }
    }

    IEnumerator StartTimer()
    {
        float timer = 0f;
        while (timer < 2f && rectTransform.anchoredPosition.y == -380f)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        if (timer >= 2f)
            leverHeld = true;
    }
}