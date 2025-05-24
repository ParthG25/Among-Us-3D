using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance)                //checks if another RoomManager exists
        {
            Destroy(gameObject);    //destroys itself as only one RoomManager can be there
            return;
        }
        DontDestroyOnLoad(gameObject);        
        Instance = this;                      //If it is the only RoomManager, makes itself the instance and makes itself not destroy when the scene switches
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //checks if we are in Polus, if yes, then we instantiate our Player Manager, Polus Tasks and Emergency Button
        if (scene.name == "Polus")                
        { 
            PhotonNetwork.Instantiate(Path.Combine("Player", "PlayerManager"), Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(Path.Combine("Tasks", "PolusTasks"), Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(Path.Combine("EmergencyText", "EmergencyText"), new Vector3(140.3f, 2.0f, 70.96f), Quaternion.identity);
        }

        //checks if we are in TheSkeld, if yes, then we instantiate our Player Manager, TheSkeld Tasks and Emergency Button
        if (scene.name == "TheSkeld")                
        { 
            PhotonNetwork.Instantiate(Path.Combine("Player", "PlayerManager"), Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(Path.Combine("Tasks", "TheSkeldTasks"), Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(Path.Combine("EmergencyText", "EmergencyText"), new Vector3(171.4f, 2.22f, 157.6f), Quaternion.identity);
        }
        
        //checks if we are in MiraHQ, if yes, then we instantiate our Player Manager, MiraHQ Tasks and Emergency Button
        if (scene.name == "MiraHQ")                
        { 
            PhotonNetwork.Instantiate(Path.Combine("Player", "PlayerManager"), Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(Path.Combine("Tasks", "MiraHQTasks"), Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(Path.Combine("EmergencyText", "EmergencyText"), new Vector3(203.43f, 2.0f, 64.36f), Quaternion.identity);
        }
        
        //checks if we are in the Lobby after a game, if yes, then we disconnect from the master server
        if (scene.name == "Lobby")                
        {
            PhotonNetwork.Disconnect();
            
            if(PlayerValues.Instance.isUnlocked)
                MenuManager.Instance.OpenMenu("HomeScreen");
            
            Cursor.lockState = CursorLockMode.None;
        }
    }
}