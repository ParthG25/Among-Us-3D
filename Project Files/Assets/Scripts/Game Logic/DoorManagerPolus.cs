using Photon.Pun;
using UnityEngine;

public class DoorManagerPolus : MonoBehaviourPunCallbacks
{
    //Creating an instance of this door manager
    public static DoorManagerPolus Instance;

    //Creating a photon view for the door manager
    private PhotonView PV;

    public bool[] open = new bool[16];
    public bool[] switchStatus = new bool[8];

    //GameObject Array of On and Off Switches
    public GameObject[] offSwitches, onSwitches;

    public Transform interfaces;

    //Array storing the audio sources on the doors
    [SerializeField] private AudioSource[] doorSound;

    //Stores the index of the door being currently accssed by the local player
    public int accessedDoor;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    
        //Destroying other instances if this gameObject
        if (Instance)                
        {
            Destroy(gameObject);    
            return;
        }
        Instance = this;
    }

    //function called on a local player to open door of index 'index'
    public void OpenDoor(int index)
    {
        PV.RPC("RPC_OpenDoor", RpcTarget.All, index);
    }

    //RPC to open door of index 'index'
    [PunRPC]
    private void RPC_OpenDoor(int index)
    {
        //playing the sound on the opened door
        doorSound[index].Play();
             
        open[index] = true;
        
        //triggering the smoke particle effect of the automatic doors
        if (index == 2 || index == 3)
        {
            for (int i = 0; i < ParticleSystemManager.Instance.smokeLaboratory.Length; i++)
            {
                ParticleSystemManager.Instance.smokeLaboratory[i].Play();
            }
        }
    
        if (index == 4 || index == 5)
        {
            for (int i = 0; i < ParticleSystemManager.Instance.smokeAdmin.Length; i++)
            {
                ParticleSystemManager.Instance.smokeAdmin[i].Play();
            }
        }
    }
    
    //function called on a local player to close door of index 'index'
    public void CloseDoor(int index)
    {
        PV.RPC("RPC_CloseDoor", RpcTarget.All, index);
    }

    //RPC to close door of index 'index'
    [PunRPC]
    private void RPC_CloseDoor(int index)
    {
        open[index] = false;
    }

    //function to keep track of the switch panel events
    public void OpenSwitchBoard()
    {
        for (int i = 0; i < switchStatus.Length; i++)
        {
            switchStatus[i] = false;
        }
        for (int i = 0; i < 4;)
        {
            int num = Random.Range(0, 8);
            if (switchStatus[num] == false)
            {
                switchStatus[num] = true;
                i++;
            }
        }

        for (int i = 0; i < switchStatus.Length; i++)
        {
            if(switchStatus[i])
            {
                onSwitches[i].SetActive(true);
                offSwitches[i].SetActive(false);
            }
            else
            {
                onSwitches[i].SetActive(false);
                offSwitches[i].SetActive(true);
            }
        }
    }

    //function to turn on the switch of ondex 'index'
    public void SwitchOn(int index)
    {
        FindObjectOfType<AudioManager>().Play("Switch");
            
        switchStatus[index] = false;
        onSwitches[index].SetActive(false);
        offSwitches[index].SetActive(true);

        for (int i = 0; i < switchStatus.Length; i++)
        {
            if (switchStatus[i])
                break;
            if(i == 7)
            {
                InterfaceManager.Instance.OpenInterface("GameScreen");
                Cursor.lockState = CursorLockMode.Locked;
                Players.isDoingTask = false;
            
                OpenDoor(accessedDoor);
            }
        }
    }
}