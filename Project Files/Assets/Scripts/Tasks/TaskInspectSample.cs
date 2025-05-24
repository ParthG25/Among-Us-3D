using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TaskInspectSample : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] GameObject gamePanel;
    public Slider[] samples;
    public TMP_Text message;
    public Button startButton;
    public Button[] greenButtons;
    public GameObject[] sampleLiquids;
    public int selectedSample;
    private float timer;

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

    public void ButtonClicked()
    {
        StartCoroutine(ReadySamples());
        startButton.interactable = false;
    }

    IEnumerator ReadySamples()
    {
        timer = 60f;
        
        while (timer >= 0f)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i].value += Time.deltaTime / 5;
                if(samples[i].value >= 1)
                    samples[i].value = 0;
            }

            timer -= Time.deltaTime;

            message.text = "Return in " + timer.ToString("F0") + " s";

            yield return null;
        }

        message.text = "Samples Ready!";

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i].value = 1;
        }

        selectedSample = Random.Range(0, 5);
        for (int i = 0; i < greenButtons.Length; i++)
        {
            greenButtons[i].interactable = true;
        }
        
        sampleLiquids[selectedSample].GetComponent<Image>().color = Color.green;
    }

    public void SelectedButton(int index)
    {
        if(index == selectedSample)
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
