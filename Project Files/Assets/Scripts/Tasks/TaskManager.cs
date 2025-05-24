using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TaskManager : MonoBehaviourPunCallbacks
{
    public static TaskManager Instance;

    //Assigning a photon view for the specific task
    private PhotonView PV;

    public bool[] activeTasks = new bool[0];
    public int totalMyTasks, totalGameTasks, completedTasks;
    public string[] allTasks;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (Instance)                
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(gameObject);            //Destroying the task instances instantiated by other players
        }
    }

    public void ResetList()
    {
        InterfaceManager.Instance.taskList.text = "";
        for (int i = 0; i < allTasks.Length; i++)
        {
            if(activeTasks[i])
                InterfaceManager.Instance.taskList.text += allTasks[i] + "\n";
        }
    }

    public void SetTaskValues()
    {
        totalMyTasks = PhotonNetwork.PlayerList.Length + (int.Parse(MapSettings.Instance.imposterCount.text) * 2);
        totalGameTasks = totalMyTasks * (PhotonNetwork.PlayerList.Length - int.Parse(MapSettings.Instance.imposterCount.text));
        
        List<int> tasks = new List<int>();
        if(!CreateAndJoinRooms.Instance.isImposter)
        {
            for (int i = 0; i < totalMyTasks;)
            {
                int index;
                if(SceneManager.GetActiveScene().name == "Polus")
                    index = Random.Range(0, 29);
                else if (SceneManager.GetActiveScene().name == "TheSkeld")
                    index = Random.Range(0, 23);
                else
                    index = Random.Range(0, 21);

                if (tasks.IndexOf(index) == -1)
                {
                    tasks.Add(index);
                    activeTasks[index] = true;
                    InterfaceManager.Instance.taskIcons[index].SetActive(true);
                    InterfaceManager.Instance.taskList.text += allTasks[index] + "\n";
                    i++;
                }
            }
        }
    }
}