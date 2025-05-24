using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviourPunCallbacks
{
    //creating an instance of this gameObject
    public static GameController Instance;
    
    //assigning a photon view to this game object
    private PhotonView PV;

    //stores the player rotation
    public Quaternion playerRotation;

    //stores the ejection Camera of that map    
    public GameObject ejectionCamera;
    public GameObject polusEjection, theSkeldEjection, miraHQEjection;

    public GameObject skeldEjectionBackground, miraHQEjectionBackground;

    //stores the player models for ejection
    public GameObject[] polusEjectionBodies, theSkeldEjectionBodies, miraHQEjectionBodies;

    public List<int> impostersList;            //to store the int numbers of imposters
    public string imposters;                   //convert the above list to string wih spaces for string logic
    
    //stores true if we need to change the camera
    public bool changeCamera;

    public int crewmateCount, imposterCount;

    // Start is called before the first frame update
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        
        //makig sure that only one instance of this gameObject is there
        if (Instance)                
        {
            Destroy(gameObject);    
            return;
        }
        Instance = this;       

        //Setting up values of map settings for all players
        if (PV.IsMine && PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_SetMapSettings", RpcTarget.All, MapSettings.Instance.mapName.text, MapSettings.Instance.imposterCount.text, MapSettings.Instance.confirmEject.text, MapSettings.Instance.killCooldown.text, MapSettings.Instance.emergengyCooldown.text, MapSettings.Instance.discussionTime.text, MapSettings.Instance.votingTime.text);

            crewmateCount = PhotonNetwork.CurrentRoom.PlayerCount - int.Parse(MapSettings.Instance.imposterCount.text);
            imposterCount = int.Parse(MapSettings.Instance.imposterCount.text);
        }
    }

    //RPC function to setup map settings for everyone
    [PunRPC]
    private void RPC_SetMapSettings(string mapName, string imposterCount, string confirmEject, string killCooldown, string emergengyCooldown, string discussionTime, string votingTime)
    {
        MapSettings.Instance.mapName.text = mapName;              
        MapSettings.Instance.imposterCount.text = imposterCount;        
        MapSettings.Instance.confirmEject.text = confirmEject;           
        MapSettings.Instance.killCooldown.text = killCooldown;           
        MapSettings.Instance.emergengyCooldown.text = emergengyCooldown;             
        MapSettings.Instance.discussionTime.text = discussionTime;           
        MapSettings.Instance.votingTime.text = votingTime;  
    }

    //function for the room host to randomly pick an imposter
    public void PickImposter()
    {
        //only the msater client should select the imposter
        if(PhotonNetwork.IsMasterClient)
        {
            imposters = " ";
            
            for (int i = 0; i < int.Parse(MapSettings.Instance.imposterCount.text);)
            {
                int imposterNumber = Random.Range(1, PhotonNetwork.CurrentRoom.PlayerCount + 1);
                if (impostersList.IndexOf(imposterNumber) == -1)
                {
                    imposters = imposters + imposterNumber + " ";
                    impostersList.Add(imposterNumber);
                    i++;
                }
            }
            
            //Providing the imposter list to everyone
            PV.RPC("RPC_SyncImposter", RpcTarget.All, imposters);
        }
    }

    //called by the master client to eject a player
    public void EjectedPlayer(int votedPlayer, string votedPlayerName, int votedSkin)
    {
        PV.RPC("RPC_EjectPlayer", RpcTarget.All, votedPlayer, votedPlayerName, votedSkin);
    }

    //RPC to remove a player and show the ejection process
    [PunRPC]
    private void RPC_EjectPlayer(int ejectedPlayer, string ejectedPlayerName, int ejectedSkin)
    {
        changeCamera = true;
     
        InterfaceManager.Instance.ejectedPlayer.text = ejectedPlayerName;
        
        if (CreateAndJoinRooms.Instance.myNumberInRoom == ejectedPlayer)
            CreateAndJoinRooms.Instance.isEjected = true;
        
        if(ejectedPlayer != -1)
            EjectAnimation(ejectedSkin);
    }

    //function to keep a count of the number of imposters and crewmates
    public void RemovePlayer(string role)
    {
        if (role == "Imposter")
            imposterCount--;
        else
            crewmateCount--;
        
        if(imposterCount == 0)
            PV.RPC("RPC_CrewmatesWin", RpcTarget.All);
        if(crewmateCount <= imposterCount)
            PV.RPC("RPC_ImpostersWin", RpcTarget.All);
    }

    //RPC called when the crewmates win
    [PunRPC]
    private void RPC_CrewmatesWin()
    {
        InterfaceManager.Instance.GameOver("CrewmatesWin");
    }
    
    //RPC called when the imposters win 
    [PunRPC]
    private void RPC_ImpostersWin()
    {
        InterfaceManager.Instance.GameOver("ImpostersWin");
    }

    private void Update()
    {
        if (changeCamera)
        {
            if (SceneManager.GetActiveScene().name == "Polus")
            {
                polusEjection.transform.position = polusEjection.transform.position - new Vector3(0f, 2.5f * Time.deltaTime, 0f);
                polusEjection.transform.Rotate(0f, 100f * Time.deltaTime, 0f);
            }

            if (SceneManager.GetActiveScene().name == "TheSkeld")
            {
                theSkeldEjection.transform.position = theSkeldEjection.transform.position + new Vector3(10f * Time.deltaTime, 0f, 0f);
                theSkeldEjection.transform.Rotate(0f, 100f * Time.deltaTime, 0f);
                skeldEjectionBackground.transform.position = skeldEjectionBackground.transform.position - new Vector3(20f * Time.deltaTime, 0f, 0f);
            }

            if (SceneManager.GetActiveScene().name == "MiraHQ")
            {
                miraHQEjection.transform.position = miraHQEjection.transform.position - new Vector3(0f, 10f * Time.deltaTime, 0f);
                miraHQEjection.transform.Rotate(0f, 100f * Time.deltaTime, 0f);
                miraHQEjectionBackground.transform.position = miraHQEjectionBackground.transform.position + new Vector3(0f, 20f * Time.deltaTime, 0f);
            }
        }
    }

    //function to play the ejection animation
    private void EjectAnimation(int ejectedPlayerSkin)
    {
        if(SceneManager.GetActiveScene().name == "Polus")
        {
            polusEjectionBodies[ejectedPlayerSkin].SetActive(true);
            StartCoroutine(PlayLavaDipSound());
        }

        if (SceneManager.GetActiveScene().name == "TheSkeld")
            theSkeldEjectionBodies[ejectedPlayerSkin].SetActive(true);

        if (SceneManager.GetActiveScene().name == "MiraHQ")
            miraHQEjectionBodies[ejectedPlayerSkin].SetActive(true);
    }

    //playing the lava dip sound after some time of the ejection
    IEnumerator PlayLavaDipSound()
    {
        float timer = 7.5f;

        while (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            
            yield return null;
        }

        FindObjectOfType<AudioManager>().Play("LavaDip");
    }
    
    //called by a local player when a task is completed
    public void TaskCompleted()
    {
        FindObjectOfType<AudioManager>().Play("TaskCompleted");
        PV.RPC("RPC_IncrementTaskBar", RpcTarget.All);
    }
    
    //RPC called when we need to nicrement the task bar
    [PunRPC]
    private void RPC_IncrementTaskBar()
    {
        TaskManager.Instance.completedTasks++;
        InterfaceManager.Instance.tasksCompleted.value = (float)TaskManager.Instance.completedTasks / TaskManager.Instance.totalGameTasks;
        if (TaskManager.Instance.completedTasks == TaskManager.Instance.totalGameTasks)
            InterfaceManager.Instance.GameOver("CrewmatesWin");
    }
    
    //providing imposter values to everyone
    [PunRPC]
    private void RPC_SyncImposter(string impostersCreated)
    {
        impostersList = new List<int>();
        imposters = impostersCreated;
        
        for (int i = 0; i < imposters.Length - 1; i++)
        {
            if (imposters[i] == ' ')
                impostersList.Add(int.Parse(GameController.Instance.imposters.Substring(i + 1,
                    (GameController.Instance.imposters.Substring(i + 1)).IndexOf(' '))));
        }
    }

    //called by the local player when they have voted
    public void Voted(int votedTo)
    {
        InterfaceManager.Instance.IVoted();
        
        PV.RPC("RPC_AddVote", RpcTarget.All, votedTo, PlayerValues.Instance.bodySkin, InterfaceManager.Instance.infoNumberInRoom.IndexOf(CreateAndJoinRooms.Instance.myNumberInRoom.ToString()));
    }

    //called when you need to display the voting info on the player you voted for
    [PunRPC]
    private void RPC_AddVote(int votedTo, int voterColor, int voterIndex)
    {
        InterfaceManager.Instance.AddVote(votedTo, voterColor, voterIndex);
    }
}