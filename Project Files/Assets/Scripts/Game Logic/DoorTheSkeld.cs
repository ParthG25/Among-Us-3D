using System.Collections;
using Photon.Pun;
using UnityEngine;

public class DoorTheSkeld : MonoBehaviour
{
    //stores left and right doors respectively
    [SerializeField] private GameObject doorLeft, doorRight;

    //target values for the transform of the left and the right doors
    [SerializeField] private Vector3 doorLeftOpenTarget;
    [SerializeField] private Vector3 doorRightOpenTarget;
    [SerializeField] private Vector3 doorLeftCloseTarget;
    [SerializeField] private Vector3 doorRightCloseTarget;

    //stores the number of people in range of this door
    private int count;

    //temporary token used to keep track of the screen that needs to be opened
    private int doorToken;

    private void Start()
    {
        //opening the doors at the start of the map
        DoorManagerTheSkeld.Instance.open[int.Parse(gameObject.name)] = true;
    }

    private void Update()
    {
        //opening the game screen if the door has been opened
        if (DoorManagerTheSkeld.Instance.open[int.Parse(gameObject.name)])
        {
            doorLeft.transform.localPosition = Vector3.MoveTowards(doorLeft.transform.localPosition, doorLeftOpenTarget,
                2f * Time.deltaTime);
            doorRight.transform.localPosition = Vector3.MoveTowards(doorRight.transform.localPosition, doorRightOpenTarget,
                2f * Time.deltaTime);
        }
        if (!DoorManagerTheSkeld.Instance.open[int.Parse(gameObject.name)] && count == 0)
        {
            doorLeft.transform.localPosition = Vector3.MoveTowards(doorLeft.transform.localPosition, doorLeftCloseTarget,
                2f * Time.deltaTime);
            doorRight.transform.localPosition = Vector3.MoveTowards(doorRight.transform.localPosition, doorRightCloseTarget,
                2f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        count++;
    }

    private void OnTriggerExit(Collider other)
    {
        count--;
    }
}
