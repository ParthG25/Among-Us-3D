using System.Collections;
using Photon.Pun;
using UnityEngine;

public class DoorMiraHQ : MonoBehaviour
{
    //stores left and right doors respectively
    [SerializeField] private GameObject doorLeft, doorRight;

    //stores true if this door is of type which need to be opened by the user with swithes
    [SerializeField] private bool isLocked;

    //target values for the transform of the left and the right doors
    [SerializeField] private Vector3 doorLeftOpenTarget;
    [SerializeField] private Vector3 doorRightOpenTarget;
    [SerializeField] private Vector3 doorLeftCloseTarget;
    [SerializeField] private Vector3 doorRightCloseTarget;

    //stores true if a door is in the state of being opened
    private bool openable; 

    //stores the number of people in range of this door
    private int count;

    private void Update()
    {
        //setting the state of the 'use' button
        if(openable && !isLocked)
        {
            if (!DoorManagerMiraHQ.Instance.open[int.Parse(gameObject.name)] && !InterfaceManager.Instance.useActive.activeSelf)
                InterfaceManager.Instance.useActive.SetActive(true);
            if (DoorManagerMiraHQ.Instance.open[int.Parse(gameObject.name)] && InterfaceManager.Instance.useActive.activeSelf)
                InterfaceManager.Instance.useActive.SetActive(false);
        }

        if (openable && isLocked && !DoorManagerMiraHQ.Instance.open[int.Parse(gameObject.name)])
        {
            DoorManagerMiraHQ.Instance.OpenDoor(int.Parse(gameObject.name));

            FindObjectOfType<AudioManager>().Play("Gas");
            StartCoroutine(CloseAutomaticDoors());
        }  
            
        //condition to open the door
        if (openable && Input.GetKeyUp("e") && !DoorManagerMiraHQ.Instance.open[int.Parse(gameObject.name)])
        {
                FindObjectOfType<AudioManager>().Play("Gas");
            
                DoorManagerMiraHQ.Instance.OpenDoor(int.Parse(gameObject.name));
                StartCoroutine(CloseAutomaticDoors());
        }
        
        //opening the game screen if the door has been opened
        if (DoorManagerMiraHQ.Instance.open[int.Parse(gameObject.name)])
        {   
            doorLeft.transform.localPosition = Vector3.MoveTowards(doorLeft.transform.localPosition, doorLeftOpenTarget,
                2f * Time.deltaTime);
            doorRight.transform.localPosition = Vector3.MoveTowards(doorRight.transform.localPosition, doorRightOpenTarget,
                2f * Time.deltaTime);
        }
        if (!DoorManagerMiraHQ.Instance.open[int.Parse(gameObject.name)] && count == 0)
        {
            doorLeft.transform.localPosition = Vector3.MoveTowards(doorLeft.transform.localPosition, doorLeftCloseTarget,
                2f * Time.deltaTime);
            doorRight.transform.localPosition = Vector3.MoveTowards(doorRight.transform.localPosition, doorRightCloseTarget,
                2f * Time.deltaTime);
        }
    }

    //IEnumerator to close the automatic doors
    IEnumerator CloseAutomaticDoors()
    {
        float timer = 3f;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            
            yield return null;
        }
        
        DoorManagerMiraHQ.Instance.CloseDoor(int.Parse(gameObject.name));
    }
    
    private void OnTriggerEnter(Collider other)
    {
        count++;
        
        if(other.GetComponent<PhotonView>().IsMine && !DoorManagerMiraHQ.Instance.open[int.Parse(gameObject.name)])
            openable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        count--;
        
        if(other.GetComponent<PhotonView>().IsMine)
        {
            if( InterfaceManager.Instance.useActive.activeSelf)
                InterfaceManager.Instance.useActive.SetActive(false);
            
            openable = false;

            foreach (Transform child in DoorManagerMiraHQ.Instance.interfaces)
            {
                if(child.name == "DoorOpenMiraHQ" && child.gameObject.activeSelf)
                {
                    InterfaceManager.Instance.OpenInterface("GameScreen");
                    Cursor.lockState = CursorLockMode.Locked;
                    Players.isDoingTask = false;
                }
            }
        }
    }
}
