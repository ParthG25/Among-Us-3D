using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class KeyHole : MonoBehaviour, IDropHandler
{
    [SerializeField] private Vector3[] positions;
    private RectTransform rectTransform;
    public bool keyReceived;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = positions[Random.Range(0, 8)];
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            keyReceived = true;
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;
        }
    }
}