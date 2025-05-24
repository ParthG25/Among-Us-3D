using Photon.Pun;
using UnityEngine;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager Instance;

    public string userRole;                           //To decide if we are creating or joining room
    
    //Declaring a 'menus' array for the menus
    [SerializeField] Menu[] menus;

    private void Awake()
    {
        if (Instance)                
        {
            Destroy(gameObject);    
            return;
        }
        Instance = this;                      
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            //Opening the required menu from the 'menus' array, and closing the rest
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    //Called when we click on Create Game
    public void HostRoom()
    {
        PhotonNetwork.ConnectUsingSettings();            //connecting to the server
        
        PhotonNetwork.NickName = PlayerValues.Instance.playerName;
        if (PhotonNetwork.NickName == "")
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000"); //giving random username if none is created
        
        userRole = "HostRoom";
        
        OpenMenu("Loading");
    }

    //Called when we click on Find Game
    public void FindRoom()
    {
        PhotonNetwork.ConnectUsingSettings();            //connecting to the server
        
        PhotonNetwork.NickName = PlayerValues.Instance.playerName;
        if (PhotonNetwork.NickName == "")
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000"); //giving random username if none is created
        
        userRole = "RoomList";
        
        OpenMenu("Loading");
    }

    public void ClickedStart()
    {
        if(PhotonNetwork.PlayerList.Length >= 4)
            OpenMenu("StartConfirmation");
        else
            OpenMenu("NotEnoughPlayers");
    }

    //used to set the game volume
    public void SetVolume()
    {
        AudioListener.volume = PlayerValues.Instance.masterVolume;
    }

    //Called when we click on 'yes' to quit the game
    public void Close()
    {
        Application.Quit();
    }
}