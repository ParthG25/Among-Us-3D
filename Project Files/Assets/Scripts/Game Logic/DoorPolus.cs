using System.Collections;
using Photon.Pun;
using UnityEngine;

public class DoorPolus : MonoBehaviour
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

    //temporary token used to keep track of the screen that needs to be opened
    private int doorToken;

    private void Start()
    {
        //closing the doors at the start of the map
        if (!isLocked)
            DoorManagerPolus.Instance.open[int.Parse(gameObject.name)] = false;
        else
            DoorManagerPolus.Instance.open[int.Parse(gameObject.name)] = true;
    }

    private void Update()
    {
        //setting the state of the 'use' button
        if(openable)
        {
            if (!DoorManagerPolus.Instance.open[int.Parse(gameObject.name)] && !InterfaceManager.Instance.useActive.activeSelf)
                InterfaceManager.Instance.useActive.SetActive(true);
            if (DoorManagerPolus.Instance.open[int.Parse(gameObject.name)] && InterfaceManager.Instance.useActive.activeSelf)
                InterfaceManager.Instance.useActive.SetActive(false);
        }
            
        //condition to open the door
        if (openable && Input.GetKeyUp("e") && !DoorManagerPolus.Instance.open[int.Parse(gameObject.name)])
        {
            if (isLocked)
            {
                if (doorToken == 0)
                {
                    DoorManagerPolus.Instance.OpenSwitchBoard();

                    InterfaceManager.Instance.OpenInterface("DoorOpenPolus");
                    Cursor.lockState = CursorLockMode.None;
                    Players.isDoingTask = true;

                    DoorManagerPolus.Instance.accessedDoor = int.Parse(gameObject.name);
                    doorToken = 1;
                }
                else
                {
                    InterfaceManager.Instance.OpenInterface("GameScreen");
                    Cursor.lockState = CursorLockMode.Locked;
                    Players.isDoingTask = false;

                    doorToken = 0;
                }
            }
            else
            {
                FindObjectOfType<AudioManager>().Play("Gas");
            
                DoorManagerPolus.Instance.OpenDoor(int.Parse(gameObject.name));
                StartCoroutine(CloseAutomaticDoors());
            }
        }
        
        //opening the game screen if the door has been opened
        if (DoorManagerPolus.Instance.open[int.Parse(gameObject.name)])
        { 
            if(doorToken == 1)            
            {
                InterfaceManager.Instance.OpenInterface("GameScreen");
                Cursor.lockState = CursorLockMode.Locked;
                Players.isDoingTask = false;

                doorToken = 0;
            }
            
            doorLeft.transform.localPosition = Vector3.MoveTowards(doorLeft.transform.localPosition, doorLeftOpenTarget,
                2f * Time.deltaTime);
            doorRight.transform.localPosition = Vector3.MoveTowards(doorRight.transform.localPosition, doorRightOpenTarget,
                2f * Time.deltaTime);
        }
        if (!DoorManagerPolus.Instance.open[int.Parse(gameObject.name)] && count == 0)
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
        float timer = 5f;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            
            yield return null;
        }
        
        DoorManagerPolus.Instance.CloseDoor(int.Parse(gameObject.name));
    }
    
    private void OnTriggerEnter(Collider other)
    {
        count++;
        
        if(other.GetComponent<PhotonView>().IsMine && !DoorManagerPolus.Instance.open[int.Parse(gameObject.name)])
        {
            openable = true;

            doorToken = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        count--;
        
        if(other.GetComponent<PhotonView>().IsMine)
        {
            if( InterfaceManager.Instance.useActive.activeSelf)
                InterfaceManager.Instance.useActive.SetActive(false);
            
            openable = false;

            foreach (Transform child in DoorManagerPolus.Instance.interfaces)
            {
                if(child.name == "DoorOpenPolus" && child.gameObject.activeSelf)
                {
                    InterfaceManager.Instance.OpenInterface("GameScreen");
                    Cursor.lockState = CursorLockMode.Locked;
                    Players.isDoingTask = false;
                }
            }
        }
    }
}
