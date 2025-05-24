using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Players : MonoBehaviourPunCallbacks
{
    //Declaring a photon view for the player
    private PhotonView PV;

    //Texts to be displayed to the crewmate and the imposter respectively
    public TMP_Text playerNameForCrewmate, playerNameForImposter;
    
    public GameObject player, playerGhost, playerDead;                                //Game objects storing these three parent folders
    public GameObject[] playerSkin, playerGhostSkin, playerDeadSkin;                  //Game objects storing all the skins of these game objects respectively
    public GameObject[] headSkin;                                                     //game objects storing all the head skins
    public GameObject[] faceSkin;                                                     //game objects storing all the face skins
    public GameObject firstPersonCamera;                                              //Game Object storing the firs person camera
    
    //player transform to rotate the player
    public Transform playerBody;
    
    private float xRotation;              
    
    //Aim constraint to look up or down
    public MultiAimConstraint headLookUp, headLookDown;
    private float pointSpeed = 5f;                        //Point speed for the constraints

    public CharacterController controller;
    public float gravity = -9.81f;

    //for checking distance from the ground to stop the player
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    //Player's animator component
    private Animator animator;

    //Speed of walkings
    public float speed = 10f;

    private Vector3 velocity;
    private bool isGrounded;            //boolean to check if the player is grounded or not

    public ParticleSystem killBlood;
    
    public static bool isDoingTask;     //boolean variables to check if player is dead and if player is doing a task
    public static bool isDoingScan;
    
    private bool isDead;                        //stores true if we are dead
    private bool justKilled;                    //stores true if we are just killed, we make it false when we complete the death animation
    private bool isEjected;
    private bool changeCamera;
    private bool isMapOpen;

    public static float killCooldown;
    public int killerBodySkin;
    
    private bool isImposter;             //stores true if we are an imposter
    private bool isMoving;

    private RaycastHit hit;             //Ray-cast from the player to detect bodies for killing and reporting
    [SerializeField] private LayerMask reportMask, ventMask;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        if(PV.IsMine)
        {
            //Calls the function to pick the imposters
            GameController.Instance.PickImposter();
            
            //Calling RPC function to send our player skins to everyone
            PV.RPC("RPC_SetBodySkin", RpcTarget.All, PlayerValues.Instance.bodySkin);
            PV.RPC("RPC_SetHeadSkin", RpcTarget.All, PlayerValues.Instance.headSkin);
            PV.RPC("RPC_SetFaceSkin", RpcTarget.All, PlayerValues.Instance.faceSkin);
            
            headSkin[PlayerValues.Instance.headSkin].SetActive(false);
            faceSkin[PlayerValues.Instance.faceSkin].SetActive(false);
            playerSkin[PlayerValues.Instance.bodySkin].SetActive(false);
            playerGhostSkin[PlayerValues.Instance.bodySkin].SetActive(false);

            //Calling RPC function to send our player skin to everyone                       
            PV.RPC("RPC_SetPlayerTag", RpcTarget.All, CreateAndJoinRooms.Instance.myNumberInRoom.ToString());
            
            //Calling RPC function to set dead body name same as index of body skin for easy reporting
            PV.RPC("RPC_SetDeadBodyName", RpcTarget.All, PlayerValues.Instance.bodySkin);
        }

        AudioListener.volume = PlayerValues.Instance.masterVolume;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        //Hiding the cursor
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();

        //Destroying the camera component of the player's instantiated by others
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //If the photon view is not ours, we don't need to control it, hence we return out of the function. Else we move the player
        if (!PV.IsMine)
        {
            playerNameForCrewmate.transform.rotation = GameController.Instance.playerRotation;
            playerNameForImposter.transform.rotation = GameController.Instance.playerRotation;
            return;
        }
        
        GameController.Instance.playerRotation = transform.rotation;
        
        if (CreateAndJoinRooms.Instance.isEjected)
        {
            isEjected = true;
            
            PV.RPC("RPC_Die", RpcTarget.All, PlayerValues.Instance.bodySkin);

            CreateAndJoinRooms.Instance.isEjected = false;
        }

        if (GameController.Instance.changeCamera && !changeCamera)
        {
            GameController.Instance.ejectionCamera.GetComponent<Camera>().enabled = true;
            firstPersonCamera.GetComponent<Camera>().enabled = false;

            changeCamera = true;
        }

        if (!GameController.Instance.changeCamera && changeCamera)
        {
            firstPersonCamera.GetComponent<Camera>().enabled = true;
            GameController.Instance.ejectionCamera.GetComponent<Camera>().enabled = false;

            changeCamera = false;
        }

        if(!InterfaceManager.Instance.inAnimation)
        {
            if (Input.GetKeyUp("space"))
            {
                if (!isMapOpen)
                {
                    InterfaceManager.Instance.OpenInterface("MapPolus");
                    Cursor.lockState = CursorLockMode.None;
                    isDoingTask = true;
                }
                else
                {
                    InterfaceManager.Instance.OpenInterface("GameScreen");
                    Cursor.lockState = CursorLockMode.Locked;
                    isDoingTask = false;
                }

                isMapOpen = !isMapOpen;
            }
            
            if (Input.GetKeyUp("escape"))
            {
                if (!InterfaceManager.Instance.isGameSettingsOpen)
                {
                    InterfaceManager.Instance.OpenInterface("Settings");
                    Cursor.lockState = CursorLockMode.None;
                    isDoingTask = true;
                }
                else
                {
                    InterfaceManager.Instance.OpenInterface("GameScreen");
                    PlayerValues.Instance.SaveValues();
                    Cursor.lockState = CursorLockMode.Locked;
                    isDoingTask = false;

                    AudioListener.volume = PlayerValues.Instance.masterVolume;
                }

                InterfaceManager.Instance.isGameSettingsOpen = !InterfaceManager.Instance.isGameSettingsOpen;
            }
            
            MovePlayer();
            if(!isDead)
            {
                if(isImposter)
                {
                    if(!VentManager.Instance.inVent)
                        KillPlayer();
                    else
                        ChangeVent();

                    if(VentManager.Instance.canVent)
                        VentIn();
                    else if (VentManager.Instance.inVent && VentManager.Instance.activeVent != -1)
                        if(VentManager.Instance.onVentCount[VentManager.Instance.activeVent] == 0)
                            VentOut();
                }
                ReportPlayer();
            }
            
            //rotating the emergency meeting text according to us
            EmergencyText.Instance.emergencyText.transform.rotation = transform.rotation;
            EmergencyText.Instance.emergencyTimer.transform.rotation = transform.rotation;

            if (EmergencyText.Instance.callEmergencyMeeting)
            {
                PV.RPC("RPC_CallEmergencyMeeting", RpcTarget.All);
                
                animator.SetBool("isMoving", false);
                
                EmergencyText.Instance.callEmergencyMeeting = false;
            }

            if (TaskSubmitScan.startedScan)
            {
                TaskSubmitScan.startedScan = false;
                PV.RPC("RPC_SetTarget", RpcTarget.All, 3);
            }
        }
        
        //Checks if we are an imposter
        if(GameController.Instance.imposters.Length != 0)
        {
            for (int i = 0; i < GameController.Instance.imposters.Length - 1; i++)
            {
                int imposterNumber = -1;
                if (GameController.Instance.imposters[i] == ' ')
                    imposterNumber = int.Parse(GameController.Instance.imposters.Substring(i + 1,
                        (GameController.Instance.imposters.Substring(i + 1)).IndexOf(' ')));
                if (imposterNumber == CreateAndJoinRooms.Instance.myNumberInRoom)
                    isImposter = true;
            }
            
            if (isImposter)
            {
                CreateAndJoinRooms.Instance.isImposter = true;
                InterfaceManager.Instance.killCooldownTimer.gameObject.SetActive(true);
                
                foreach (GameObject obj in SabotageManager.Instance.sabotages)
                {
                    obj.SetActive(true);
                }
                SabotageManager.Instance.ventInstructions.SetActive(true);

                RenderSettings.fogStartDistance = 40;
                RenderSettings.fogEndDistance = 65;
                firstPersonCamera.GetComponent<Camera>().farClipPlane = 67;

                InterfaceManager.Instance.taskList.text = "Sabotage and kill everyone!";
                InterfaceManager.Instance.taskList.color = Color.red;
                InterfaceManager.Instance.taskList.alignment = TextAlignmentOptions.Center;
                InterfaceManager.Instance.taskList.fontSize = 45f;
            }
            else
            {
                RenderSettings.fogStartDistance = 15;
                RenderSettings.fogEndDistance = 40;
                firstPersonCamera.GetComponent<Camera>().farClipPlane = 42;
            }

            TaskManager.Instance.SetTaskValues();

            PV.RPC("RPC_SetImposter", RpcTarget.All, isImposter);
            
            //Calling an RPC function to send our name to everyone
            PV.RPC("RPC_SetName", RpcTarget.All, PhotonNetwork.NickName);

            playerNameForCrewmate.gameObject.SetActive(false);
            playerNameForImposter.gameObject.SetActive(false);

            GameController.Instance.imposters = "";

            //conditions to display the name according to the roles
            if(isImposter)
            {
                InterfaceManager.Instance.RevealRole("imposter");
                firstPersonCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("ForImposter");
            }
            else
            {
                InterfaceManager.Instance.RevealRole("crewmate");
                firstPersonCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("ForCrewmate");
            }
        }

        if (InterfaceManager.Instance.resetPosition)
        {
            if (SceneManager.GetActiveScene().name == "Polus")
            {
                transform.position = SpawnPoints.Instance
                    .polusEmergencyPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].position;
                transform.rotation = SpawnPoints.Instance
                    .polusEmergencyPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].rotation;
            }

            if (SceneManager.GetActiveScene().name == "TheSkeld")
            {
                transform.position = SpawnPoints.Instance
                    .theSkeldSpawnPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].position;
                transform.rotation = SpawnPoints.Instance
                    .theSkeldSpawnPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].rotation;
            }

            if (SceneManager.GetActiveScene().name == "MiraHQ")
            {
                transform.position = SpawnPoints.Instance
                    .miraHQEmergencyPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].position;
                transform.rotation = SpawnPoints.Instance
                    .miraHQEmergencyPoints[CreateAndJoinRooms.Instance.myNumberInRoom - 1].rotation;
            }
        }
    }
    
    //function to check if we want to kill anyone
    private void KillPlayer()
    {
        if (Physics.Raycast(firstPersonCamera.transform.position - new Vector3(0f, 0.5f, 0f), firstPersonCamera.transform.forward, out hit, 2f))
        {
            if(killCooldown <= 0f)
            {
                if (int.TryParse(hit.transform.tag, out int targetPlayer))
                {
                    if (GameController.Instance.impostersList.IndexOf(targetPlayer) == -1)
                    {
                        InterfaceManager.Instance.killActive.SetActive(true);

                        if (Input.GetKey("f") && isImposter)
                        {
                            //play dead sound
                            FindObjectOfType<AudioManager>().Play("Kill");
                            
                            PV.RPC("RPC_KilledCrewmate", RpcTarget.All);
                            
                            hit.transform.gameObject.GetPhotonView().RPC("RPC_Die", RpcTarget.All,
                                PlayerValues.Instance.bodySkin);
                            controller.Move(transform.forward * 5);

                            InterfaceManager.Instance.killActive.SetActive(false);
                            
                            InterfaceManager.Instance.KillCooldown();
                        }
                    }
                    else
                    {
                        InterfaceManager.Instance.killActive.SetActive(false);
                    }
                }
            }
        }
        else
        {
            InterfaceManager.Instance.killActive.SetActive(false);
        }
    }

    //function to check if we want to vent in
    private void VentIn()
    {
        if(Input.GetKeyUp("e"))
        {
            FindObjectOfType<AudioManager>().Play("VentIn");
            FindObjectOfType<AudioManager>().Stop("VentOut");

            gameObject.layer = 6;

            StartCoroutine(ChangeGround());
           
            firstPersonCamera.transform.localPosition = new Vector3(firstPersonCamera.transform.localPosition.x,
                6.5f, firstPersonCamera.transform.localPosition.z);

            InterfaceManager.Instance.killActive.SetActive(false);
            VentManager.Instance.inVent = true;

            VentManager.Instance.mapVentNetworks
                [int.Parse(VentManager.Instance.ventNetwork
                [VentManager.Instance.ventNetwork.Length - 1].ToString()) - 1].SetActive(true);
        }
    }

    //IEnumerator to make sure we are inside the vent
    IEnumerator ChangeGround()
    {
        while (transform.position.y > -5f)
        {
            transform.position = new Vector3(transform.position.x, -5.5f, transform.position.z);
            
            yield return null;
        }
    }
    
    //function to vent out
    private void VentOut()
    {
        if(Input.GetKeyUp("e"))
        {
            FindObjectOfType<AudioManager>().Play("VentOut");
            FindObjectOfType<AudioManager>().Stop("VentIn");

            PV.RPC("RPC_VentAnimation", RpcTarget.All);

            controller.Move(transform.up * 5);
            firstPersonCamera.transform.localPosition = new Vector3(firstPersonCamera.transform.localPosition.x,
                2.45f, firstPersonCamera.transform.localPosition.z);
            gameObject.layer = 0;

            VentManager.Instance.inVent = false;
            
            VentManager.Instance.mapVentNetworks
            [int.Parse(VentManager.Instance.ventNetwork
                [VentManager.Instance.ventNetwork.Length - 1].ToString()) - 1].SetActive(false);
        }
    }

    //function to change vents
    private void ChangeVent()
    {
        if (Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit, 450f, ventMask))
        {
            string currentVentNetwork = VentManager.Instance.ventNetwork;
            string targetVentNetwork = hit.transform.name;

            if(currentVentNetwork == targetVentNetwork)
            {
                if (Input.GetMouseButton(0)) 
                {
                    FindObjectOfType<AudioManager>().Play("VentTravel");
                    VentManager.Instance.activeVent = int.Parse(hit.transform.parent.name);
                }
            }
        }
        
        transform.position = new Vector3(VentManager.Instance.vents[VentManager.Instance.activeVent].position.x, transform.position.y,
            VentManager.Instance.vents[VentManager.Instance.activeVent].position.z);
    }

    //function to report players    
    private void ReportPlayer()
    {
        if (Physics.Raycast(firstPersonCamera.transform.position - new Vector3(0f, 2f, 0f), gameObject.transform.forward, out hit, 3f, reportMask))
        {
            if(hit.transform.CompareTag("DeadBody"))
            {
                InterfaceManager.Instance.reportActive.SetActive(true);
                
                if (Input.GetKey("q"))
                {
                    CreateAndJoinRooms.Instance.hasReported = true;
                    
                    PV.RPC("RPC_BodyReported", RpcTarget.All, hit.transform.name);
                    
                    animator.SetBool("isMoving", false);

                    InterfaceManager.Instance.reportActive.SetActive(false);
                }
            }
            else
            {
                InterfaceManager.Instance.reportActive.SetActive(false);
            }
        }
        else
        {
            InterfaceManager.Instance.reportActive.SetActive(false);
        }
    }
    
    //Function to perform movements on a player
    private void MovePlayer()
    {
        MapManager.Instance.playerIcon.rotation = Quaternion.Euler(0f, 0f, -transform.eulerAngles.y);
        Vector3 posi = new Vector3(transform.position.x / 300 * 1561 - 780.5f, transform.position.z / 207.6f * 1080 - 540, 0f);
        MapManager.Instance.playerIcon.localPosition = posi;
        
        MapManager.Instance.fullMiniMap.rotation = Quaternion.Euler(0f, 0f, transform.eulerAngles.y);
        posi = new Vector3(transform.position.x / 300 * 1561 - 780.5f, transform.position.z / 207.6f * 1080 - 540, 0f);
        MapManager.Instance.miniMap.localPosition = -posi;
        
        if (isDead && justKilled)
        {
            CreateAndJoinRooms.Instance.isDead = true;
            
            if(CreateAndJoinRooms.Instance.isImposter)
                InterfaceManager.Instance.killCooldownTimer.gameObject.SetActive(false);
            
            if(!isEjected)
                InterfaceManager.Instance.GotKilled(killerBodySkin, PlayerValues.Instance.bodySkin);
            
            firstPersonCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("Ghost");
            
            if(isImposter)
                firstPersonCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("ForImposterDead");
            else
                firstPersonCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("ForCrewmateDead");
            
            PV.RPC("RPC_SetPlayerTag", RpcTarget.All, "Untagged");

            justKilled = false;
            isEjected = false;
        }
        
        //calling an RPC function if we move our head up/down
        PV.RPC("RPC_MoveHead", RpcTarget.All, xRotation);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }

        float x = 0f;
        float z = 0f;
        
        if(!VentManager.Instance.inVent)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        //condition to check if the walk animation needs to be played or the idle one 
        if (x != 0 || z != 0)
        {
            if(!isMoving && !CreateAndJoinRooms.Instance.isDead)
                FindObjectOfType<AudioManager>().Play("Walk");

            animator.SetBool("isMoving", true);
            isMoving = true;
        }
        else
        {
            FindObjectOfType<AudioManager>().Stop("Walk");

            animator.SetBool("isMoving", false);
            isMoving = false;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        //moving the player
        if(!isDoingScan) 
            controller.Move(move * speed * Time.deltaTime);
       
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //taking mouse input to rotate the player
        float mouseX = Input.GetAxis("Mouse X") * PlayerValues.Instance.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * PlayerValues.Instance.mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        //rotating the player
        if (!isDoingTask || isDoingScan)
        {
            firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    //RPC called by the player who killed someone
    [PunRPC]
    private void RPC_KilledCrewmate()
    {
        if(PhotonNetwork.IsMasterClient)
            GameController.Instance.RemovePlayer("Crewmate");
    }
        
    [PunRPC]
    private void RPC_SetTarget(int index)
    {
        TaskSubmitScan.target = index;
    }
    
    //RPC to trigger the vent animation
    [PunRPC]
    private void RPC_VentAnimation()
    {
        animator.SetTrigger("Vent");
    }

    //RPC function to assign the player tag
    [PunRPC]
    private void RPC_SetPlayerTag(string tag)
    {
        gameObject.tag = tag;
    }
    
    //RPC function to set dead body name as the body skin index
    [PunRPC]
    private void RPC_SetDeadBodyName(int index)
    {
        playerDead.name = index.ToString();
    }
    
    //RPC function to assign the isImposter bool
    [PunRPC]
    private void RPC_SetImposter(bool imposter)
    {
        isImposter = imposter;
    }

    //RPC function to assign the respective body skin
    [PunRPC]
    private void RPC_SetBodySkin(int index)
    {
        for (int i = 0; i < playerSkin.Length; i++)
        {
            if(i == index)
                playerSkin[i].SetActive(true);
            else
                playerSkin[i].SetActive(false);
        }
        
        for (int i = 0; i < playerDeadSkin.Length; i++)
        {
            if(i == index)
                playerDeadSkin[i].SetActive(true);
            else
                playerDeadSkin[i].SetActive(false);
        }
        
        for (int i = 0; i < playerGhostSkin.Length; i++)
        {
            if(i == index)
                playerGhostSkin[i].SetActive(true);
            else
                playerGhostSkin[i].SetActive(false);
        }
    }
    
    //RPC function to assign the respective head skin
    [PunRPC]
    private void RPC_SetHeadSkin(int index)
    {
        for (int i = 0; i < headSkin.Length; i++)
        {
            if(i == index)
                headSkin[i].SetActive(true);
            else
                headSkin[i].SetActive(false);
        }
    }
    
    //RPC function to assign the respective face skin
    [PunRPC]
    private void RPC_SetFaceSkin(int index)
    {
        for (int i = 0; i < faceSkin.Length; i++)
        {
            if(i == index)
                faceSkin[i].SetActive(true);
            else
                faceSkin[i].SetActive(false);
        }
    }

    //RPC function to set the player name
    [PunRPC]
    private void RPC_SetName(string name)
    {
        playerNameForCrewmate.text = name;
        playerNameForImposter.text = name;
        if(isImposter)
            playerNameForImposter.color = Color.red;
    }
    
    //RPC function called when a body is reported
    [PunRPC]
    private void RPC_BodyReported(string name)
    {
        InterfaceManager.Instance.BodyReported(name);
        
        PV.RPC("RPC_SetVotingInfo", RpcTarget.All, PhotonNetwork.NickName, 
            CreateAndJoinRooms.Instance.myNumberInRoom.ToString(), CreateAndJoinRooms.Instance.isDead.ToString(), 
            PlayerValues.Instance.bodySkin.ToString(), CreateAndJoinRooms.Instance.hasReported.ToString(), 
            CreateAndJoinRooms.Instance.hasCalledEmergency.ToString());
    }
    
    //RPC function called when an emergency meeting is called
    [PunRPC]
    private void RPC_CallEmergencyMeeting()
    {
        InterfaceManager.Instance.EmergencyMeetingCalled();
        
        PV.RPC("RPC_SetVotingInfo", RpcTarget.All, PhotonNetwork.NickName, 
            CreateAndJoinRooms.Instance.myNumberInRoom.ToString(), CreateAndJoinRooms.Instance.isDead.ToString(), 
            PlayerValues.Instance.bodySkin.ToString(), CreateAndJoinRooms.Instance.hasReported.ToString(), 
            CreateAndJoinRooms.Instance.hasCalledEmergency.ToString());
    }

    //RPC to set all the player info whenever a meeting is called
    [PunRPC]
    private void RPC_SetVotingInfo(string name, string numberInRoom, string dead, string skin, string reported, string emergency)
    {
        InterfaceManager.Instance.infoName.Add(name);
        InterfaceManager.Instance.infoNumberInRoom.Add(numberInRoom);
        InterfaceManager.Instance.infoIsDead.Add(dead);
        InterfaceManager.Instance.infoSkin.Add(skin);
        InterfaceManager.Instance.infoHasReported.Add(reported);
        InterfaceManager.Instance.infoHasCalledEmergency.Add(emergency);
    }
    
    //RPC function for killing ourself
    [PunRPC]
    private void RPC_Die(int killerSkin)
    {
        killBlood.Play();
        
        killerBodySkin = killerSkin;
        
        justKilled = true;
        isDead = true;
        player.SetActive(false);
        playerDead.SetActive(true);
        playerGhost.SetActive(true);
        animator.SetBool("isDead", true);

        speed = 15f; //setting the walk speed to 15 for ghosts

        playerDead.transform.parent = null;                                //making dead body independent of us
        
        gameObject.layer = 8;                                              //setting our player to "Ghost" layer

        for (int i = 0; i < headSkin.Length; i++)                          //setting our head skins to "Ghost" layer
            headSkin[i].layer = 8;

        for (int i = 0; i < faceSkin.Length; i++)                          //setting our face skins to "Ghost" layer
            faceSkin[i].layer = 8;

        playerNameForCrewmate.gameObject.layer = 11;                       //making our name only visible to dead crew-mates
        playerNameForImposter.gameObject.layer = 12;                       //making our name only visible to dead imposters
    }
    
    //RPC function for moving head up/down
    [PunRPC]
    private void RPC_MoveHead(float xRotation)
    {
        if (xRotation < -20 && !isDead)
        {
            headLookDown.weight = Mathf.MoveTowards(headLookDown.weight, 0, pointSpeed * Time.deltaTime);
            headLookUp.weight = Mathf.MoveTowards(headLookUp.weight, 1, pointSpeed * Time.deltaTime);
        }
        else if (xRotation > 10 && !isDead)
        {
            headLookUp.weight = Mathf.MoveTowards(headLookUp.weight, 0, pointSpeed * Time.deltaTime);
            headLookDown.weight = Mathf.MoveTowards(headLookDown.weight, 1, pointSpeed * Time.deltaTime);
        }
        else
        {
            headLookDown.weight = Mathf.MoveTowards(headLookDown.weight, 0, pointSpeed * Time.deltaTime);
            headLookUp.weight = Mathf.MoveTowards(headLookUp.weight, 0, pointSpeed * Time.deltaTime);
        }
    }
}