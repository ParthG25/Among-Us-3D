using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskRepairDrill : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] GameObject gamePanel;
    [SerializeField] private Button[] buttons;
    private int[] count = new int[4];
    public TMP_Text status;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    private void Update()
    {
        if (InterfaceManager.Instance.inAnimation)
        {
            if(gamePanel.activeSelf)
            {
                if(gamePanel.activeSelf)
                {
                    ButtonOrderPanelClose();
                    Players.isDoingTask = false;
                }
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

    public void ButtonClick(int index)
    {
        count[index]++;
        buttons[index].GetComponent<RectTransform>().localScale -= new Vector3(0.1f, 0.1f, 0f);
        
        if(count[index] == 4)
            buttons[index].gameObject.SetActive(false);

        bool flag = true;
        
        for (int i = 0; i < 4; i++)
        {
            if (count[i] < 4)
            {
                flag = false;
                break;
            }
        }

        if (flag)
            StartCoroutine(DrillFixed());
    }

    IEnumerator DrillFixed()
    {
        status.text = "Status: Good";
        status.color = Color.green;
        
        float timer = 0.0f;
        
        while (timer <= 2.0f)
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
