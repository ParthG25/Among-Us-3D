using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
     [SerializeField] private TMP_Text text;                              //stores the name of the player
     
     private Player player;
     
     public void SetUp(Player _player)
     {
          player = _player;
          text.text = _player.NickName;                         //changing player name to its nickname
     }

     public override void OnPlayerLeftRoom(Player otherPlayer)
     {
          if (player == otherPlayer)
          {
               Destroy(gameObject);                           
          }
     }
     
     public override void OnLeftRoom()
     {
          Destroy(gameObject);  
     }
}
