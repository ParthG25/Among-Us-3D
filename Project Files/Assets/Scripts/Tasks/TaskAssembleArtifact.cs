using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TaskAssembleArtifact : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;

    [SerializeField] GameObject gamePanel;
  
    private int count = 0;
    private bool finished;

    public Image[] parts;
    public Vector2[] startPositions;
    public Vector2[] endPositions;
    public Vector2[] finalPositions;

    private bool[] artifactSet = new bool[4];

    public ArtifactMira[] artifactMira;

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

        if (TaskManager.Instance.activeTasks[serialNumber] && !finished)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                float x = parts[i].rectTransform.anchoredPosition.x, y = parts[i].rectTransform.anchoredPosition.y;

                if (!artifactSet[i])
                {
                    if (x > startPositions[i].x && x < endPositions[i].x && y < startPositions[i].y && y > endPositions[i].y)
                    {
                        parts[i].raycastTarget = false;
                        count++;
                        artifactSet[i] = true;
                        artifactMira[i].isPlaced = true;
                    }
                }
            }

            if (count == 4)
            {
                StartCoroutine(EndTask());
                finished = true;
            }
        }
    }

    IEnumerator EndTask()
    {
        float timer = 0;

        while (timer < 1f)
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
