using Photon.Pun;
using UnityEngine;

public class Reactor : MonoBehaviour
{
    //stores the model which needs to be highlighted
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject gamePanel;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    //stores true if the sabotage as been fixed
    public bool sabotageFixed;

    private void Update()
    {
        if (InterfaceManager.Instance.inAnimation)
        {
            if(gamePanel.activeSelf)
            {
                ButtonOrderPanelClose();
                Players.isDoingTask = false;
            }
        }

        if (sabotageFixed)
        {
            InterfaceManager.Instance.useActive.SetActive(false);
            
            inRange = false;
            Players.isDoingTask = false;
            ButtonOrderPanelClose();

            //removing outline from task asset
            model.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
            
            sabotageFixed = false;
        }
        
        if (inRange && Input.GetKeyUp("e"))
        {
            Players.isDoingTask = !Players.isDoingTask;
            
            //to open or close the task panel if player is doing or not doing task respectively
            if(Players.isDoingTask)
                ButtonOrderPanelOpen();
            else
                ButtonOrderPanelClose();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine && SabotageManager.Instance.reactorSabotaged && !CreateAndJoinRooms.Instance.isDead)
        {
            InterfaceManager.Instance.useActive.SetActive(true);
            
            inRange = true;

            //Adding outline to task asset
            model.GetComponent<Renderer>().material.SetFloat("_Outline", 0.02f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine)
        {
            if(SabotageManager.Instance.reactorHeld)
            {
                SabotageManager.Instance.reactorHeld = false;
                SabotageManager.Instance.Decrement(SabotageManager.Instance.activeReactorSide);
                SabotageManager.Instance.activeReactorSide = "";
            }

            InterfaceManager.Instance.useActive.SetActive(false);
            
            inRange = false;
            Players.isDoingTask = false;
            ButtonOrderPanelClose();

            //removing outline from task asset
            model.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
        }
    }
    
    //function to close task panel
    private void ButtonOrderPanelClose()
    {
        gamePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    //function to open game panel
    private void ButtonOrderPanelOpen()
    {       
        gamePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}