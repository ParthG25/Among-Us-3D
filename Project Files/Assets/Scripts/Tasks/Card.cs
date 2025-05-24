using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public RectTransform rectTransform;
    public bool inPlace, passed;
    private float startTimerMilli, endTimerMilli, startTimerSec, endTimerSec;
    public GameObject greenLightOn, redLightOn;
    public GameObject message;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inPlace)
        {
            startTimerSec = DateTime.Now.Second;
            startTimerMilli = DateTime.Now.Millisecond;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    { 
        if (inPlace)
        {
            endTimerSec = DateTime.Now.Second;
            endTimerMilli = DateTime.Now.Millisecond;
        }

        float timeDiff = (endTimerSec - startTimerSec) * 1000 + (endTimerMilli - startTimerMilli);
        timeDiff /= 1000;

        if(inPlace && rectTransform.anchoredPosition.x >= 550)
        {
            if (timeDiff > 0.75 && timeDiff < 1.25)
            {
                StartCoroutine(Swiped("pass"));
                message.GetComponent<TMP_Text>().text = "Accepted!";
                message.GetComponent<TMP_Text>().color = Color.green;
                passed = true;

                GetComponent<Image>().raycastTarget = false;
            }
            else
            {
                StartCoroutine(Swiped("fail"));
                
                if (timeDiff < 0.75)
                    message.GetComponent<TMP_Text>().text = "Too Fast!";
                if (timeDiff > 1.25)
                    message.GetComponent<TMP_Text>().text = "Too Slow!";
                
                message.GetComponent<TMP_Text>().color = Color.red;
                rectTransform.anchoredPosition = new Vector2(-300f, 40f);
            }
        }
    }
    
    IEnumerator Swiped(string result)
    {
        if (result == "pass")
            greenLightOn.SetActive(true);
        else
            redLightOn.SetActive(true);
        message.SetActive(true);
        
        yield return new WaitForSeconds(0.75f);
        
        message.SetActive(false);
        greenLightOn.SetActive(false);
        redLightOn.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        
        float x, y;
        x = rectTransform.anchoredPosition.x;
        y = rectTransform.anchoredPosition.y;

        if (x > -280 && x < -200 && y > -50 && y < 50 && !inPlace)
        {
            rectTransform.anchoredPosition = new Vector2(-300f, 40f);
            inPlace = true;
        }

        if(inPlace)
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 40);
    }
}