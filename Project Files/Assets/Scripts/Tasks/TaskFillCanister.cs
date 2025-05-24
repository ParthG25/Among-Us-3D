using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TaskFillCanister : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] GameObject gamePanel;
    public CanisterSlot canisterSlot;
    public Canister canister;
    public Slider canisterFill;
    private bool taskComplete;

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
            if (Input.GetMouseButtonUp(0) && canister.initialized && !canisterSlot.canisterReceived)
            {
                canister.canvasGroup.alpha = 1f;
                canister.canvasGroup.blocksRaycasts = true;
                canister.rectTransform.anchoredPosition = canister.position;
            }
            
            if (canisterSlot.canisterReceived && !taskComplete)
            {
                StartCoroutine(EndTask());
                taskComplete = true;
            }
        }
    }

    IEnumerator EndTask()
    {
        while (canisterFill.value < 1f)
        {
            canisterFill.value += Time.deltaTime / 10;
            
            yield return null;
        }

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
        gamePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
