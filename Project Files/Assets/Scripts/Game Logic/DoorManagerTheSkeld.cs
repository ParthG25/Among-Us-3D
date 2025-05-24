using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DoorManagerTheSkeld : MonoBehaviourPunCallbacks
{
    //Creating an instance of this door manager
    public static DoorManagerTheSkeld Instance;

    //Creating a photon view for the door manager
    private PhotonView PV;

    public bool[] open = new bool[13];
    public bool[] switchStatus = new bool[8];

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

        if(PhotonNetwork.IsMasterClient)
            StartCoroutine(OpenAutomaticDoors(index));
    }

    //IEnumerator to open the automatic doors
    IEnumerator OpenAutomaticDoors(int index)
    {
        float timer = 15f;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            yield return null;
        }

        DoorManagerTheSkeld.Instance.OpenDoor(index);
    }
}