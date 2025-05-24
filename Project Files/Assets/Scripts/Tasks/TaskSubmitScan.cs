using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TaskSubmitScan : MonoBehaviourPunCallbacks
{
    [SerializeField] private string taskName;
    [SerializeField] private int serialNumber;
    
    public static int target;
    public static bool startedScan;
    private float timer;

    private PhotonView PV;
    
    //variable to check if the player is in range for the task or not
    private bool inRange;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        target = 0;
    }

    private void Update()
    {
        if (InterfaceManager.Instance.inAnimation && Players.isDoingScan)
        {
            Players.isDoingTask = false;
            Players.isDoingScan = false;
        }
        
        if (inRange && Input.GetKeyUp("e") && TaskManager.Instance.activeTasks[serialNumber] && !Players.isDoingTask)
        {
            Players.isDoingTask = true;
            Players.isDoingScan = true;
            startedScan = true;
            StartCoroutine(StartScan());
        }
        
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, target, transform.position.z), 
            0.8f * Time.deltaTime);

        if (transform.position.y >= 3)
            target = 0;
    }

    IEnumerator StartScan()
    {
        timer = 8f;
        
        while (timer >= 0f &&  Players.isDoingScan)
        {
            timer -= Time.deltaTime;
            
            yield return null;
        }

        if(timer <= 0f)
        {
            InterfaceManager.Instance.useActive.SetActive(false);

            inRange = false;
            Players.isDoingTask = false;
            Players.isDoingScan = false;

            //removing outline from task asset
            gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);

            GameController.Instance.TaskCompleted();
            TaskManager.Instance.activeTasks[serialNumber] = false;
            TaskManager.Instance.ResetList();
            InterfaceManager.Instance.taskIcons[serialNumber].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine)
        {
            if(TaskManager.Instance.activeTasks[serialNumber])
            {
                InterfaceManager.Instance.useActive.SetActive(true);

                inRange = true;

                //Adding outline to task asset
                gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0.02f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine)
        {
            Players.isDoingScan = false;

            InterfaceManager.Instance.useActive.SetActive(false);
            
            inRange = false;
            Players.isDoingTask = false;

            //removing outline from task asset
            gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
        }
    }
}
