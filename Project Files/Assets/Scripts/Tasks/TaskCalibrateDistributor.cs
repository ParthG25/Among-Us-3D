using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TaskCalibrateDistributor : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;

    [SerializeField] GameObject gamePanel;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    //gameObject arrays that store the down switches, up switches and the on lights
    public GameObject[] onLights, offLights;
    public GameObject[] dials;
    public Button[] buttons;

    private int activeDial = 0;
    private bool[] dialsFixed = new bool[3];

    private bool started;

    private void Awake()
    {
        for (int i = 0; i < dials.Length; i++)
        {
            dials[i].transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(90f, 270f));
        }
    }

    private void Update()
    {
        //closing the panel if you enter the state of animation
        if (InterfaceManager.Instance.inAnimation)
        {
            if (gamePanel.activeSelf)
            {
                ButtonOrderPanelClose();
                Players.isDoingTask = false;
            }
        }

        //cheking the conditions when the user wants to open the panel
        if (inRange && Input.GetKeyUp("e"))
        {
            Players.isDoingTask = !Players.isDoingTask;

            //to open or close the task panel if player is doing or not doing task respectively
            if (Players.isDoingTask)
            {
                ButtonOrderPanelOpen();
                if(!started)
                    StartCoroutine(RotateDial());
                started = true;
            }
            else
                ButtonOrderPanelClose();
        }
    }

    IEnumerator RotateDial()
    {
        while (true)
        {
            dials[activeDial].transform.Rotate(new Vector3(0f, 0f, 250 * Time.deltaTime));

            bool flag = true;
            for (int i = 0; i < dialsFixed.Length; i++)
            {
                if (!dialsFixed[i])
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
                break;

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

    //called when the button is clicked
    public void ButtonClick(int index)
    {
        if(dials[index].transform.rotation.eulerAngles.z > 345f || dials[index].transform.rotation.eulerAngles.z < 15f)
        {
            buttons[index].interactable = false;

            offLights[index].SetActive(false);

            dialsFixed[index] = true;
            dials[index].transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            if (index < 2)
                activeDial = index + 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine && TaskManager.Instance.activeTasks[serialNumber])
        {
            InterfaceManager.Instance.useActive.SetActive(true);

            inRange = true;

            //Adding outline to task asset
            model.GetComponent<Renderer>().material.SetFloat("_Outline", 0.02f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine)
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