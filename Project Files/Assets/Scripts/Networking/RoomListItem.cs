using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;                    //Stores the name on the room

    public RoomInfo info;                                     //instance of RoomInfo which stores room name and room code in one string
    
    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name.Substring(0, _info.Name.IndexOf("|"));        //displaying only the room name
    }

    public void OnClick()
    {
       CreateAndJoinRooms.Instance.GoToJoinRoom(info);          //calling GoToJoinRoom with full name + "|" + code   
    }
}