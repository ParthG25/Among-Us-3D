using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TaskMoniterTree : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] GameObject gamePanel;
    public Slider slider1, slider2, slider3, slider4;
    public Image[] targets;
    public int[] targetPosi = new int[4];
    private int count; 
    private bool taskCompleted;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    private void Start()
    {
        for (int i = 0; i < targetPosi.Length; i++)
        {
            targetPosi[i] = Random.Range(0, 391);
            
            RectTransform rectTransform = targets[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, targetPosi[i]);
        }
        
        slider1.onValueChanged.AddListener((newValue) =>
        {
            newValue = (float)Math.Round(newValue * 100f) / 100f;
            float target = (float)Math.Round(((targetPosi[0] + 270.0) / 660.0) * 100f) / 100f;

            if (newValue == target)
            {
                slider1.interactable = false;
                count++;
            }
        });
        slider2.onValueChanged.AddListener((newValue) =>
        {
            newValue = (float)Math.Round(newValue * 100f) / 100f;
            float target = (float)Math.Round(((targetPosi[1] + 270.0) / 660.0) * 100f) / 100f;

            if (newValue == target)
            {
                slider2.interactable = false;
                count++;
            }
        });
        slider3.onValueChanged.AddListener((newValue) =>
        {
            newValue = (float)Math.Round(newValue * 100f) / 100f;
            float target = (float)Math.Round(((targetPosi[2] + 270.0) / 660.0) * 100f) / 100f;

            if (newValue == target)
            {
                slider3.interactable = false;
                count++;
            }
        });
        slider4.onValueChanged.AddListener((newValue) =>
        {
            newValue = (float)Math.Round(newValue * 100f) / 100f;
            float target = (float)Math.Round(((targetPosi[3] + 270.0) / 660.0) * 100f) / 100f;

            if (newValue == target)
            {
                slider4.interactable = false;
                count++;
            }
        });
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
        
        if(TaskManager.Instance.activeTasks[serialNumber] && !taskCompleted)
        {
            if(count == 4)
            {
                taskCompleted = true;

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
        gamePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
