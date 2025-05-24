using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    private float rotationSpeed, moveSpeed;
    public float startX;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rotationSpeed = Random.Range(30, 80);
        moveSpeed = Random.Range(1000, 1200);
        
        startX = rectTransform.anchoredPosition.x;
        rectTransform.anchoredPosition = new Vector3(startX, Random.Range(-410, 410), 0);
    }

    private void Update()
    {
        rectTransform.Rotate(0.0f, 0.0f, rotationSpeed * Time.deltaTime);
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x - (moveSpeed * Time.deltaTime), rectTransform.anchoredPosition.y);
         
        if(rectTransform.anchoredPosition.x < - startX)
            rectTransform.anchoredPosition = new Vector3(startX, rectTransform.anchoredPosition.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Destroy(gameObject);
        TaskClearAsteroids.count++;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}