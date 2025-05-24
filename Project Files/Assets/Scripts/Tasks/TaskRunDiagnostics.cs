using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TaskRunDiagnostics : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;

    [SerializeField] GameObject gamePanel;
    public bool taskCompleted;

    public Button button;
    public Slider ship;
    public TMP_Text progress;
    public GameObject normals;
    public GameObject[] anomalies;

    //variable to check if the player is in range for the task or not
    private bool inRange;

    private void Update()
    {
        if (InterfaceManager.Instance.inAnimation)
        {
            if (gamePanel.activeSelf)
            {
                ButtonOrderPanelClose();
                Players.isDoingTask = false;
            }
        }

        if (inRange && Input.GetKeyUp("e"))
        {
            Players.isDoingTask = !Players.isDoingTask;

            //to open or close the task panel if player is doing or not doing task respectively
            if (Players.isDoingTask)
                ButtonOrderPanelOpen();
            else
                ButtonOrderPanelClose();
        }
    }

    public void Button()
    {
        button.interactable = false;

        progress.text = "0% completed.";
        StartCoroutine(FinishTask());
    }

    public void AnomalyClicked()
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

    IEnumerator FinishTask()
    {
        float timer = 0;

        while (timer < 40f)
        {
            timer += Time.deltaTime;

            progress.text = ((int)(timer / 40f * 100)) + "% completed";
            ship.value -= 0.25f * Time.deltaTime;

            if (ship.value <= 0f)
                ship.value = 1f;

            yield return null;
        }

        progress.text = "Detect Anomaly!";

        ship.value = 0;
        normals.SetActive(true);
        anomalies[Random.Range(0, 5)].SetActive(true);
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
