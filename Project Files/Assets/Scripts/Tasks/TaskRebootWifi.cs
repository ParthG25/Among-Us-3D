using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskRebootWifi : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    [SerializeField] GameObject gamePanel;
    public Slider wifi;
    public TMP_Text message;
    public Button button;
    public Vector2 targetPosition;
    private bool rebootReady;
    private float timer;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    private void Awake()
    {
        targetPosition = new Vector2(384, 292);
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
            button.GetComponent<RectTransform>().anchoredPosition =
                Vector2.MoveTowards(button.GetComponent<RectTransform>().anchoredPosition, targetPosition, 2500f * Time.deltaTime);

            if (button.GetComponent<RectTransform>().anchoredPosition == targetPosition)
                targetPosition = new Vector2(384, 292);
        }
    }

    public void ButtonClicked()
    {
        if (timer == 0.0f)
        {
            if (!rebootReady)
            {
                timer = 60f;
                StartCoroutine(RebootWifi());
            }
            else
            {
                StartCoroutine(EndTask());
                button.interactable = false;
            }
            
            targetPosition = new Vector2(384, -298);
        }
    }

    IEnumerator RebootWifi()
    {
        while (timer >= 0f)
        {
            wifi.value += Time.deltaTime / 5;
            if (wifi.value >= 1)
                wifi.value = 0;
            
            timer -= Time.deltaTime;

            message.text = "Return in " + timer.ToString("F0") + " s";

            yield return null;
        }

        rebootReady = true;
        message.text = "Reboot WiFi";
        message.color = Color.green;
        wifi.value = 0;
        timer = 0f;
    }

    IEnumerator EndTask()
    {
        wifi.value = 1;
        float timer = 0;
        
        while (timer <= 2f)
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
