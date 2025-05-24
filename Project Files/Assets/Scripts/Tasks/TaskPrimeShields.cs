using Photon.Pun;
using UnityEngine;

public class TaskPrimeShields : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;

    [SerializeField] GameObject gamePanel;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    //gameObject arrays that store the down switches, up switches and the on lights
    public GameObject[] onSwitches, offSwitches;

    private void Update()
    {
        //closing the panel if you enter the state of animation
        if (InterfaceManager.Instance.inAnimation)
        {
            if (gamePanel.activeSelf)
            {
                ButtonOrderPanelClose();
                Players.isDoingTask = false;
            }
        }

        //cheking the conditions when the user wants to open the panel
        if (inRange && Input.GetKeyUp("e"))
        {
            Players.isDoingTask = !Players.isDoingTask;

            //to open or close the task panel if player is doing or not doing task respectively
            if (Players.isDoingTask)
            {
                ButtonOrderPanelOpen();
                OnEnable();
            }
            else
                ButtonOrderPanelClose();
        }
    }

    //function that sets the values of the switches
    public void OnEnable()
    {
        bool[] temp = new bool[7];
        for (int i = 0; i < temp.Length; i++)
        {
            int index = Random.Range(0, 2);
            if (index == 0)
                temp[i] = true;
            else
                temp[i] = false;
        }

        temp[Random.Range(0, 7)] = false;

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i])
            {
                onSwitches[i].SetActive(true);
                offSwitches[i].SetActive(false);
            }
            else
            {
                onSwitches[i].SetActive(false);
                offSwitches[i].SetActive(true);
            }
        }
    }

    //called when a down button is clicked
    public void OnButtonClick(int index)
    {
        onSwitches[index].SetActive(false);
        offSwitches[index].SetActive(true);
    }

    //called when an up button is clicked
    public void OffButtonClick(int index)
    {
        onSwitches[index].SetActive(true);
        offSwitches[index].SetActive(false);

        bool flag = true;

        for (int i = 0; i < onSwitches.Length; i++)
        {
            if (!onSwitches[i].activeSelf)
                flag = false;
        }

        if (flag)
        {
            InterfaceManager.Instance.useActive.SetActive(false);

            inRange = false;
            Players.isDoingTask = false;
            ButtonOrderPanelClose();

            //removing outline from task asset
            model.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);

            GameController.Instance.TaskCompleted();
            TaskManager.Instance.activeTasks[serialNumber] = false;
            TaskManager.Instance.ResetList();
            InterfaceManager.Instance.taskIcons[serialNumber].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine && TaskManager.Instance.activeTasks[serialNumber])
        {
            InterfaceManager.Instance.useActive.SetActive(true);

            inRange = true;

            //Adding outline to task asset
            model.GetComponent<Renderer>().material.SetFloat("_Outline", 0.02f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine)
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