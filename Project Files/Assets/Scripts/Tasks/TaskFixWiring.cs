using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class TaskFixWiring : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] private GameObject gamePanel;

    public Camera taskCamera;
    public List<Color> wireColors = new List<Color>();
    public List<Wire> leftWires = new List<Wire>();
    public List<Wire> rightWires = new List<Wire>();

    private bool openedOnce;
    private List<Color> availableColors;
    private List<int> availableLeftWireIndex;
    private List<int> availableRightWireIndex;

    public Wire currentDraggedWire;
    public Wire currentHoveredWire;
    
    //variable to check if the player is in range for the task or not
    private bool inRange;

    private void Awake()
    {
        taskCamera.enabled = false;
    }

    private void AssignColor()
    {
        availableColors = new List<Color>(wireColors);
        availableLeftWireIndex = new List<int>();
        availableRightWireIndex = new List<int>();

        for (int i = 0; i < leftWires.Count; i++)
        {
            availableLeftWireIndex.Add(i);
        }
        for (int i = 0; i < leftWires.Count; i++)
        {
            availableRightWireIndex.Add(i);
        }

        while (availableColors.Count > 0 && availableLeftWireIndex.Count > 0 && availableRightWireIndex.Count > 0)
        {
            Color pickedColor = availableColors[Random.Range(0, availableColors.Count)];
            int pickedLeftWireIndex = Random.Range(0, availableLeftWireIndex.Count);
            int pickedRightWireIndex = Random.Range(0, availableRightWireIndex.Count);
            
            leftWires[availableLeftWireIndex[pickedLeftWireIndex]]
                .SetColor(pickedColor);
            rightWires[availableRightWireIndex[pickedRightWireIndex]]
                .SetColor(pickedColor);
            
            availableColors.Remove(pickedColor); 
            availableLeftWireIndex.RemoveAt(pickedLeftWireIndex);
            availableRightWireIndex.RemoveAt(pickedRightWireIndex);
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
        
        if(TaskManager.Instance.activeTasks[serialNumber])
        {
            int successfulWires = 0;
            for (int i = 0; i < rightWires.Count; i++)
            {
                if (rightWires[i].isSuccess)
                    successfulWires++;
            }

            if (successfulWires >= rightWires.Count)
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
        taskCamera.enabled = false;
        gamePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    //function to open game panel
    private void ButtonOrderPanelOpen()
    {
        taskCamera.enabled = true;
        gamePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        if (!openedOnce)
        {
            AssignColor();
            openedOnce = true;
        }
    }
}