using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TaskFixNodes : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] GameObject gamePanel;
    private bool[] offState = new bool[9];
    [SerializeField] private GameObject[] offButtons, onButtons;
    [SerializeField] private GameObject[] offSlots, onSlots;
    [SerializeField] private GameObject[] offLights, onLights;
    [SerializeField] private TMP_Text[] names;
    
    //variable to check if the player is in range for the task or not
    private bool inRange;

    private void Start()
    {
        for (int i = 0; i < offState.Length; i++)
        {
            int decider;
            decider = Random.Range(0, 2);

            if (decider == 0)
                offState[i] = false;
            else
                offState[i] = true;

            names[i].text = "NODE_" + Random.Range(0, 100);
        }
        
        offState[Random.Range(0, 9)] = true;
        
        for (int i = 0; i < offState.Length; i++)
        {
            offSlots[i].SetActive(offState[i]);
            onSlots[i].SetActive(!offState[i]);
            offLights[i].SetActive(offState[i]);
            onLights[i].SetActive(!offState[i]);
            offButtons[i].SetActive(offState[i]);
            onButtons[i].SetActive(!offState[i]);
        }
    }

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
    }

    public void ButtonClicked(int index)
    {
        offSlots[index].SetActive(false);
        onSlots[index].SetActive(true);
        offLights[index].SetActive(false);
        onLights[index].SetActive(true);
        offButtons[index].SetActive(false);
        onButtons[index].SetActive(true);

        bool flag = true;
        for (int i = 0; i < onButtons.Length; i++)
        {
            if (!onButtons[i].gameObject.activeSelf)
            {
                flag = false;
                break;
            }
        }

        if (flag)
            StartCoroutine(EndTask());
    }

    IEnumerator EndTask()
    {
        float timer = 0;
        
        while (timer <= 1f)
        {
            timer += Time.deltaTime;
            
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
