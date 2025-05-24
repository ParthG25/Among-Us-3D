using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TaskRecordTemperature : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] GameObject gamePanel;
    public TemperatureButtonUp temperatureButtonUp;
    public TemperatureButtonDown temperatureButtonDown;
    
    [SerializeField] private int startTemperature, endTemperature;
    private float targetTemperature, wrongTemperature;
    public TMP_Text targetTemperatureText, wrongTemperatureText;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    private bool finished;

    private void Awake()
    {
        targetTemperature = Random.Range(startTemperature, endTemperature+1);
        targetTemperatureText.text = targetTemperature.ToString("F0");

        wrongTemperature = Random.Range(-200, 201);
        wrongTemperatureText.text = wrongTemperature.ToString("F0");
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
        
        if(TaskManager.Instance.activeTasks[serialNumber] && !finished)
        {
            if (temperatureButtonUp.buttonHeld)
            {
                wrongTemperature += Time.deltaTime * 20;
                wrongTemperatureText.text = wrongTemperature.ToString("F0");
            }
            if (temperatureButtonDown.buttonHeld)
            {
                wrongTemperature -= Time.deltaTime * 20;
                wrongTemperatureText.text = wrongTemperature.ToString("F0");
            }

            if (int.Parse(wrongTemperature.ToString("F0")) >= targetTemperature && targetTemperature > 0)
            {
                temperatureButtonUp.deactivateButton = true;
                temperatureButtonDown.deactivateButton = true;
                
                temperatureButtonUp.buttonHeld = false;
                temperatureButtonDown.buttonHeld = false;

                finished = true;
                StartCoroutine(TemperatureFixed());
            }
            
            if (int.Parse(wrongTemperature.ToString("F0")) <= targetTemperature && targetTemperature < 0)
            {
                temperatureButtonUp.deactivateButton = true;
                temperatureButtonDown.deactivateButton = true;
                
                temperatureButtonUp.buttonHeld = false;
                temperatureButtonDown.buttonHeld = false;

                finished = true;
                StartCoroutine(TemperatureFixed());
            }
        }
    }
    
    IEnumerator TemperatureFixed()
    {
        wrongTemperatureText.text = targetTemperatureText.text;
        
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
