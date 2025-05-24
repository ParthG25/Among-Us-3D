using Photon.Pun;
using TMPro;
using UnityEngine;

public class MapSettings : MonoBehaviour
{
    public static MapSettings Instance;
    
    public TMP_Text mapName;
    public TMP_Text imposterCount;
    public TMP_Text confirmEject;
    public TMP_Text killCooldown;
    public TMP_Text emergengyCooldown;
    public TMP_Text discussionTime;
    public TMP_Text votingTime;

    private void Awake()
    {
        //making sure that only one instance of this gameObject is there
        if (Instance)                
        {
            Destroy(gameObject);    
            return;
        }
        Instance = this;                   
    }

    private void Start()
    {
        mapName.text = "Polus";                //Maps are Polus / TheSkeld / MiraHQ
        imposterCount.text = "1";              //Imposters range from 1 to 9
        confirmEject.text = "Yes";             //between "Yes" or "No"
        killCooldown.text = "20 s";            //between "10 s" and "60 s", increment is doe by "5 s"
        emergengyCooldown.text = "20 s";       //between "10 s" and "60 s", increment is doe by "5 s"          
        discussionTime.text = "30 s";          //between "10 s" and "180 s", increment is doe by "10 s"   
        votingTime.text = "30 s";              //between "10 s" and "120 s", increment is doe by "10 s"   
    }
    
    //to change map
    public void MapName(string selectedMap)           //selectedMap stores the name of the clicked map
    {
        mapName.text = selectedMap;
    }

    //to change imposter count
    public void ImposterCount(string operation)       //operation checks for increase or decrease
    {
        int count = int.Parse(imposterCount.text);
        
        if (operation == "Increase" && count == 1 && PhotonNetwork.PlayerList.Length >= 7)
        {
            imposterCount.text = (count + 1).ToString();
        }
        if (operation == "Increase" && count == 2 && PhotonNetwork.PlayerList.Length >= 9)
        {
            imposterCount.text = (count + 1).ToString();
        }
        
        if (operation == "Decrease" && count > 1)
        {
            imposterCount.text = (count - 1).ToString();
        }

        if (operation == "Recount")
        {
            if (count == 3 && PhotonNetwork.PlayerList.Length < 9)
            {
                imposterCount.text = (count - 1).ToString();
                count--;
            }    
            if (count == 2 && PhotonNetwork.PlayerList.Length < 7)
            {
                imposterCount.text = (count - 1).ToString();
                count--;
            }    
        }
    }
    
    //to change confirm eject
    public void ConfirmEject(string operation)        //operation checks for increase or decrease
    {
        if (confirmEject.text == "Yes")
            confirmEject.text = "No";
        else
            confirmEject.text = "Yes";
    }
    
    //to change kill cooldown
    public void KillCooldown(string operation)        //operation checks for increase or decrease
    {
        int count = int.Parse(killCooldown.text.Substring(0, killCooldown.text.IndexOf(" ")));
        if (operation == "Increase" && count < 60)
        {
            killCooldown.text = (count + 5) + " s";
        }
        if (operation == "Decrease" && count > 10)
        {
            killCooldown.text = (count - 5) + " s";
        }
    }
    
    //to change emergency cooldown
    public void EmergengyCooldown(string operation)   //operation checks for increase or decrease
    {
        int count = int.Parse(emergengyCooldown.text.Substring(0, emergengyCooldown.text.IndexOf(" ")));
        if (operation == "Increase" && count < 60)
        {
            emergengyCooldown.text = (count + 5) + " s";
        }
        if (operation == "Decrease" && count > 10)
        {
            emergengyCooldown.text = (count - 5) + " s";
        }
    }
    
    //to change discussion time
    public void DiscussionTime(string operation)      //operation checks for increase or decrease
    {
        int count = int.Parse(discussionTime.text.Substring(0, discussionTime.text.IndexOf(" ")));
        if (operation == "Increase" && count < 180)
        {
            discussionTime.text = (count + 10) + " s";
        }
        if (operation == "Decrease" && count > 10)
        {
            discussionTime.text = (count - 10) + " s";
        }
    }
    
    //to change voting time
    public void VotingTime(string operation)          //operation checks for increase or decrease
    {
        int count = int.Parse(votingTime.text.Substring(0, votingTime.text.IndexOf(" ")));
        if (operation == "Increase" && count < 120)
        {
            votingTime.text = (count + 10) + " s";
        }
        if (operation == "Decrease" && count > 10)
        {
            votingTime.text = (count - 10) + " s";
        }
    }
}
