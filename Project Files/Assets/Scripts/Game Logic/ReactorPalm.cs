using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReactorPalm : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private PhotonView PV;
    [SerializeField] private string side;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(!SabotageManager.Instance.reactorHeld)
            GetComponent<Image>().color = Color.red;
    }

    //called when the the user holds the click on the referenced image
    public void OnPointerDown(PointerEventData eventData)
    {
        SabotageManager.Instance.reactorHeld = true;
        GetComponent<Image>().color = Color.cyan;
        SabotageManager.Instance.Increment(side);
        SabotageManager.Instance.activeReactorSide = side;
    }

    //called when the the user lifts the click on the referenced image
    public void OnPointerUp(PointerEventData eventData)
    {
        SabotageManager.Instance.reactorHeld = false;
        GetComponent<Image>().color = Color.red;
        SabotageManager.Instance.Decrement(side);
        SabotageManager.Instance.activeReactorSide = "";
    }
}