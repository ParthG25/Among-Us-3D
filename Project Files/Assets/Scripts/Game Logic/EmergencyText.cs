using Photon.Pun;
using TMPro;
using UnityEngine;

public class EmergencyText : MonoBehaviour
{
    //this stores the model which needs to be highlighted
    [SerializeField] private GameObject model;

    //Assigning a photon view for the specific task
    private PhotonView PV;

    //creating an instance of this class
    public static EmergencyText Instance;

    //they store the time remaining and timer text respectively
    public TMP_Text emergencyTimer, emergencyText;

    private bool inRange;

    //stores true temporarily if you call the emergency meeting
    public bool callEmergencyMeeting;
        
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        //making sure that there is only one instance of this gameObject
        if (Instance)                
        {
            Destroy(gameObject);   
            return;
        }
        Instance = this; 
    }

    private void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(gameObject);            //Destroying the task instances instantiated by other players
        }
    }

    private void Update()
    {
        //checkign all the necessary conditions for the meeting to be called
        if (inRange && InterfaceManager.Instance.emergencyCooldown <= 0.0f 
                    && !InterfaceManager.Instance.inAnimation  
                    && !SabotageManager.Instance.reactorSabotaged
                    && !SabotageManager.Instance.lightsSabotaged)
        {
            InterfaceManager.Instance.useActive.SetActive(true);
            
            //Adding outline to task asset
            model.GetComponent<Renderer>().material.SetFloat("_Outline", 0.02f);
            
            if(Input.GetKey("e"))
            {
                CreateAndJoinRooms.Instance.hasCalledEmergency = true;

                callEmergencyMeeting = true;
                inRange = false;
                
                //removing outline from task asset
                model.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
            }
        }
    }

    //set the value of the time text
    public void SetTimer(float time)
    {
        emergencyTimer.text = time.ToString("F0") + " s";
        if (emergencyTimer.text == "0 s")
            emergencyTimer.text = "";
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine)
            inRange = true;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<PhotonView>().IsMine)
        {
            inRange = false;
            InterfaceManager.Instance.useActive.SetActive(false);
            
            //removing outline from task asset
            model.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
        }
    }
}
