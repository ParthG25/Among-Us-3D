using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public static CreateAndJoinRooms Instance;
    
    [SerializeField] private string fullCode;                       //to store the full name of the room a user clicks on 
    [SerializeField] private TMP_InputField createLobby;            //to take input of the code to create room
    [SerializeField] private TMP_InputField joinLobby;              //to take input of the code to join the room
    [SerializeField] private TMP_InputField createdRoomName;        //name of the room taken as input
    [SerializeField] private TMP_Text displayedRoomName;            //room of the name displayed in room list and inside that room
    [SerializeField] private Transform roomListContent;             //stores Room List Content game object which is initially empty
    [SerializeField] private Transform playerListContent;           //stores Player List Content game object which is initially empty
    [SerializeField] private GameObject roomListItemPrefab;         //stores the Room List Item Prefab
    [SerializeField] private GameObject playerListItemPrefab;       //stores the Player List Item Prefab
    [SerializeField] private GameObject startGameButton;            //holds start game button
    [SerializeField] private GameObject roomSettingsButton;         //holds room settings button
    [SerializeField] private GameObject roomManager;                //hold the prefab of the Room Manager which is instantiated at the start of the game
    
    private List<RoomInfo> fullRoomList = new List<RoomInfo>();
    private List<RoomListItem> roomListItems = new List<RoomListItem>();

    public int myNumberInRoom;                              //Stores the number at which you joined the room
    
    public bool isDead;
    public bool isImposter;
    public bool hasReported;
    public bool hasCalledEmergency;
    public bool isEjected;
    
    private void Awake()
    {
        if (Instance)                //checks if another RoomManager exists
        {
            Destroy(gameObject);    //destroys itself as only one RoomManager can be there
            return;
        }      
        Instance = this;

        Instantiate(roomManager, new Vector3(0, 0, 0), Quaternion.identity);    //Instantiating the Room Manager at the starting of the game
    }

    //Called when we connect to the master server
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();                         //Lobbies are where we find or create rooms. This is done inside the master server
        PhotonNetwork.AutomaticallySyncScene = true;       //to sync scenes for all players in a room
    }

    //Called when we join the lobby
    public override void OnJoinedLobby()
    {
        if(MenuManager.Instance.userRole != "")                     //if we leave a room, this function is called again, hence we want to go to the host-find screen and not the particular role screen
            MenuManager.Instance.OpenMenu(MenuManager.Instance.userRole);
    }
    
    //Creates room with the given full code (name + code). Called when we click on 'Create Room' button in room creation menu
    public void CreateRoom()
    {
        MenuManager.Instance.OpenMenu("Loading");
        if (createdRoomName.text.Length > 16)            //allowing only a maximum of 16 characters for the room name
            createdRoomName.text = createdRoomName.text.Substring(0, 16);
        PhotonNetwork.CreateRoom(createdRoomName.text + "|" + createLobby.text, new RoomOptions() {IsVisible = true, IsOpen = true, MaxPlayers = 10});    //creating a room with code as "{name} + "|" + {room code}" with a max of 10 players allowed
    }
    
    //called when a player successfully creates a room
    public override void OnCreatedRoom()
    { 
        MenuManager.Instance.OpenMenu("Room");
        fullCode = "";                                    //setting this to "", because when a player first joins a room, this variable gets a value. And if they create a room after that, they will still see the room they joined before as their room name
        displayedRoomName.text = createdRoomName.text;    //setting displayed text same as inputted text or the person who created the room

        myNumberInRoom = 1;
    }
    
    //called when room is not created for some reason (like when a room with the entered code already exists)
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.OpenMenu("Error");            //Opening the error menu if we can't create a room
    }

    //called when the room host clicks on 'Start Game'
    public void StartGame()
    {
        MenuManager.Instance.OpenMenu("Loading");
        PhotonNetwork.LoadLevel(MapSettings.Instance.mapName.text);
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    //called when a user clicks on a room in the rooms list
    public void GoToJoinRoom(RoomInfo info)
    {
        fullCode = info.Name;                        //full code of the room user clicked on
        MenuManager.Instance.OpenMenu("JoinRoom");
    }
    
    //we call this when a user clicks on 'join room' button after entering the room code
    public void JoinRoom()
    {
        //Checking if the room code entered is correct
        if (fullCode.Substring(fullCode.IndexOf("|") + 1) == joinLobby.text)
        {
            MenuManager.Instance.OpenMenu("Loading");
            PhotonNetwork.JoinRoom(fullCode);
        }
    }

    //called when we join a room
    public override void OnJoinedRoom()
    { 
        MenuManager.Instance.OpenMenu("Room");
        
        //when we create a room, this function is also called. Hence to avoid an error for room creator, we put this condition and set the room name as part of room code before "|" for people who join the room
        if(fullCode != "")                            
            displayedRoomName.text = fullCode.Substring(0, fullCode.IndexOf("|"));

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length ; i++)
        {
            //Instantiation player list item prefab as players join a room
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);        //sets button active only for room host
        roomSettingsButton.SetActive(PhotonNetwork.IsMasterClient);     //sets button active only for room host

        if (!PhotonNetwork.IsMasterClient)
            myNumberInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
    }
    
    //Called when we fail to join the room, called in situations like if the room is full
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.OpenMenu("RoomFull");
    }

    //called when the room host leaves and a new host is selected by Photon Network
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);        //sets button active only for room host
        roomSettingsButton.SetActive(PhotonNetwork.IsMasterClient);     //sets button active only for room host
    }

    //called when we want to leave room by clicking on the 'Leave Room' button
    public void Disconnect()
    {
        MenuManager.Instance.userRole = "";            //setting userRole to an empty string so that we don't go back to host screen, and go to host-find Screen
        PhotonNetwork.Disconnect();                    //disconnects the player from the server
        MenuManager.Instance.OpenMenu("Host-FindScreen");
    }

    //called when a player enters a room successfully 
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //instantiating player list item for everyone else who joins the room
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    //called when a remote players leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //we first assign 1 to myNumberInRoom, then we increment it till we get our number in the room
        myNumberInRoom = 1;
        Player[] allPlayers = PhotonNetwork.PlayerList;

        foreach (Player player in allPlayers)
        {
            if (player != PhotonNetwork.LocalPlayer)
                myNumberInRoom++;
            else
                break;
        }

        if (PhotonNetwork.IsMasterClient)
            MapSettings.Instance.ImposterCount("Recount");
    }

    //called when the room list is updated in the Photon Lobby
    public override void OnRoomListUpdate(List<RoomInfo> roomList) 
    {
        foreach(RoomInfo updatedRoom in roomList)
        {
            RoomInfo existingRoom = fullRoomList.Find(x => x.Name.Equals(updatedRoom.Name)); // Check to see if we have that room already
            if(existingRoom == null) //We do not have it
            {
                fullRoomList.Add(updatedRoom); //Add the room to the full room list
            }
            else if(updatedRoom.RemovedFromList) //We do have it, so check if it has been removed
            {
                fullRoomList.Remove(existingRoom); //Remove it from our full room list
            }
        }
        RenderRoomList();
    }

    private void RenderRoomList()
    {
        RemoveRoomList();
        foreach(RoomInfo roomInfo in fullRoomList)
        {
            RoomListItem roomListItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
            roomListItem.SetUp(roomInfo);
            roomListItems.Add(roomListItem);
        }
    }

    private void RemoveRoomList()
    {
        foreach(RoomListItem roomListItem in roomListItems)
        {
            Destroy(roomListItem.gameObject);
        }
        roomListItems.Clear();
    }
}