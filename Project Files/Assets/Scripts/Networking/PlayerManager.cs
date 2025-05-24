using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    //Declaring a photon view for the player
    private PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    
    // Update is called once per frame
    private void Start()
    {
        //Calling the CreatePlayer function if the photon view is mine
        if (PV.IsMine)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        //Instantiating the player on polus spawn points
        if(SceneManager.GetActiveScene().name == "Polus")
            PhotonNetwork.Instantiate(Path.Combine("Player", "Player"), SpawnPoints.Instance.polusSpawnPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].position, Quaternion.Euler(0f, 180f,0f));

        //Instantiating the player on TheSkeld spawn points
        if(SceneManager.GetActiveScene().name == "TheSkeld")
            PhotonNetwork.Instantiate(Path.Combine("Player", "Player"), SpawnPoints.Instance.theSkeldSpawnPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].position, Quaternion.Euler(SpawnPoints.Instance.theSkeldSpawnPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].eulerAngles));

        //Instantiating the player on MiraHQ spawn points
        if(SceneManager.GetActiveScene().name == "MiraHQ")
            PhotonNetwork.Instantiate(Path.Combine("Player", "Player"), SpawnPoints.Instance.miraHQSpawnPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].position, Quaternion.Euler(SpawnPoints.Instance.miraHQSpawnPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].eulerAngles));
    }
}