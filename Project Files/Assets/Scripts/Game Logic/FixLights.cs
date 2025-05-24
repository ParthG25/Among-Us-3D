using Photon.Pun;
using UnityEngine;

public class FixLights : MonoBehaviour
{
    //stores the model that need to be highlighted
    [SerializeField] private GameObject model;

    //store the panel for the fixing of the lights
    [SerializeField] private GameObject gamePanel;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    //gameObject arrays that store the down switches, up switches and the on lights
    public GameObject[] downSwitches, upSwitches, onLights;

    //becomes true if the sabotage has been fixed
    public bool sabotageFixed;

    private void Update()
    {
        //closing the panel if you enter the state of animation
        if (InterfaceManager.Instance.inAnimation)
        {
            if(gamePanel.activeSelf)
            {
                ButtonOrderPanelClose();
                Players.isDoingTask = false;
            }
        }

        //resetting things when the sabotaged has been fixed
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
        
        //cheking the conditions when the user wants to open the panel
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
    
    //function that sets the values of the switches
    public void OnEnable()
    {
        bool[] temp = new bool[5];
        for (int i = 0; i < temp.Length; i++)
        {
            int index = Random.Range(0, 2);
            if (index == 0)
                temp[i] = true;
            else
                temp[i] = false;
        }

        temp[Random.Range(0, 5)] = false;

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i])
            {
                downSwitches[i].SetActive(true);
                onLights[i].SetActive(true);
                upSwitches[i].SetActive(false);
            }
            else
            {
                downSwitches[i].SetActive(false);
                onLights[i].SetActive(false);
                upSwitches[i].SetActive(true);
            }
        }
    }

    //called when a down button is clicked
    public void OnDownButtonClick(int index)
    {
        FindObjectOfType<AudioManager>().Play("Switch");

        downSwitches[index].SetActive(false);
        onLights[index].SetActive(false);
        upSwitches[index].SetActive(true);
    }
    
    //called when an up button is clicked
    public void OnUpButtonClick(int index)
    {
        FindObjectOfType<AudioManager>().Play("Switch");

        downSwitches[index].SetActive(true);
        onLights[index].SetActive(true);
        upSwitches[index].SetActive(false);

        bool flag = true;
        
        for (int i = 0; i < downSwitches.Length; i++)
        {
            if (!downSwitches[i].activeSelf)
                flag = false;
        }

        if(flag)
            SabotageManager.Instance.FixedLights();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine && SabotageManager.Instance.lightsSabotaged && !CreateAndJoinRooms.Instance.isDead)
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