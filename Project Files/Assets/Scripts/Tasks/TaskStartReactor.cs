using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TaskStartReactor : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;

    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject[] buttons;
    [SerializeField] GameObject[] lights;
    [SerializeField] GameObject[] indicators;
    
    int[] lightOrder = new int[9];
    private int level, buttonsClicked, colorOrderRunCount;
    private bool passed, won;

    private float lightSpeed = 0.3f;
    
    Color32 red = new Color32(255, 39, 0, 255);    
    Color32 green = new Color32(4, 204, 0, 255);
    Color32 invisible = new Color32(4, 204, 0, 0);
    Color32 white = new Color32(255, 255, 255, 255);
        
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine  && TaskManager.Instance.activeTasks[serialNumber])
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

    //function to set buttons in random order in hierarchy and game window 
    private void OnEnable()
    {
        level = 0;
        buttonsClicked = 0;
        colorOrderRunCount = 0;
        won = false;
        
        for (int i = 0; i < lightOrder.Length; i++)
        {
            lightOrder[i] = Random.Range(0, 9);
        }
        for (int i = 0; i < indicators.Length; i++)
        {
            indicators[i].GetComponent<Image>().color = white;
        }
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].GetComponent<Image>().color = invisible;
        }

        level = 1;
        StartCoroutine(ColorOrder());
    }

    //function to perform task by checking order of the buttons pressed
    public void ButtonClick(int button)
    {
        buttonsClicked++;
        if (button == lightOrder[buttonsClicked - 1])
            passed = true;
        else
        {
            won = false;
            passed = false;
            StartCoroutine(ColorBlink(red));
        }

        if (buttonsClicked == level && passed && buttonsClicked != 5)
        {
            level++;
            passed = false;
            StartCoroutine(ColorOrder());
        }
        if (buttonsClicked == level && passed && buttonsClicked == 5)
        {
            won = true;
            StartCoroutine(ColorBlink(green));
        }
    }

    IEnumerator ColorBlink(Color32 colorToBlink)
    {
        DisableInteractableButtons();
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<Image>().color = colorToBlink;
            }

            for (int i = 5; i < indicators.Length; i++)
            {
                indicators[i].GetComponent<Image>().color = colorToBlink;
            }
            
            yield return  new WaitForSeconds(0.2f);

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<Image>().color = white;
            } 
            for (int i = 5; i < indicators.Length; i++)
            {
                indicators[i].GetComponent<Image>().color = white;
            }
            
            yield return  new WaitForSeconds(0.2f);
        }
        
        if(won)
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
        EnableInteractableButtons();
        OnEnable();
    }

    IEnumerator ColorOrder()
    {
        buttonsClicked = 0;
        colorOrderRunCount++;
        DisableInteractableButtons();
        for (int i = 0; i < colorOrderRunCount; i++)
        {
            if (level >= colorOrderRunCount)
            {
                lights[lightOrder[i]].GetComponent<Image>().color = invisible;
                yield return new WaitForSeconds(lightSpeed);
                lights[lightOrder[i]].GetComponent<Image>().color = green;
                yield return new WaitForSeconds(lightSpeed);
                lights[lightOrder[i]].GetComponent<Image>().color = invisible;
                indicators[i].GetComponent<Image>().color = green;
            }
        }
        EnableInteractableButtons();
    }

    void DisableInteractableButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Button>().interactable = false;
        }
    }
    
    void EnableInteractableButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Button>().interactable = true;
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
        OnEnable();
        gamePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}