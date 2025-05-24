using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TaskReplaceWaterJug : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] GameObject gamePanel;
    public JugButton jugButton;
    [SerializeField] private Slider leftJug, rightJug;

    //variable to check if the player is in range for the task or not
    private bool inRange;

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
        
        if (inRange && Input.GetKeyUp("e"))
        {
            Players.isDoingTask = !Players.isDoingTask;
            
            //to open or close the task panel if player is doing or not doing task respectively
            if(Players.isDoingTask)
                ButtonOrderPanelOpen();
            else
                ButtonOrderPanelClose();
        }
        
        if(TaskManager.Instance.activeTasks[serialNumber])
        {
            if (jugButton.buttonHeld)
            {
                leftJug.value -= Time.deltaTime / 5;
                rightJug.value += Time.deltaTime / 5;
            }
            
            if (leftJug.value <= 0)
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine && TaskManager.Instance.activeTasks[serialNumber])
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
        leftJug.value = 1;
        rightJug.value = 0;
        jugButton.buttonHeld = false;
        gamePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
