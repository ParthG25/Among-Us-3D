using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviourPunCallbacks
{
    public static InterfaceManager Instance;

    //Declaring a 'interfaces' array for the menus
    [SerializeField] private Interface[] interfaces;
    
    //holds the imposter icons on the roles screen
    public GameObject[] imposters;
    //holds the crewmate icon on the roles screen
    public GameObject crewmate;
    //holds the crewmate or the imposter game screen
    public GameObject crewmateRole, imposterRole;
    //holds the imposter icons on the game over screen
    public GameObject[] impostersGameOver;
    //holds the crewmate icon on the game over screen
    public GameObject crewmateGameOver;
    //holds the defeat or the victory game screens
    public GameObject defeat, victory;
    public GameObject killInactive;
    public GameObject killActive, useActive, reportActive;
    public GameObject discussionTimePanel;
    public GameObject[] killerBodies, killedBodies, reportedBodies;
    public GameObject[] taskIcons;

    public float roleRevealTimer, gotKilledTimer, bodyReportedTimer, emergencyCalledTimer, discussionTimer, votingTimer;
    public float emergencyCooldown, afterVotingTime, votingResultTime;

    public TMP_Text discussionTimeLeft, votingTimeLeft, ejectedPlayer, killCooldownTimer;
    public TMP_Text taskList, importantMessage;
    public TMP_Text[] imposterNameGameOver;
    
    private string timer, temp;
    
    public bool inAnimation;                                         //hold true if player is in a state where he can't move like "role  reveal" and "discussions"
    public bool justReported;
    public bool hasVoted;
    public bool resetPosition;
    public bool isGameSettingsOpen;
    
    public List<string> infoName, infoNumberInRoom, infoIsDead, infoSkin, infoHasReported, infoHasCalledEmergency;

    public GameObject taskListBackground;
    public GameObject skippedVotes, skipVoteButton, skippedImage;
    public GameObject[] names, vote, playerName, voters, iVoted, reported, calledEmergency, namePlateHighlight;
    public GameObject[] playerPhotosPrefab, voterPhotosPrefab;

    //holds the non-interactable slider for the total tasks completed
    public Slider tasksCompleted;

    //holds the index of the player that has been ejected
    public int removedPlayerIndex;
    
    private void Awake()
    {
        if (Instance)                
        {
            Destroy(gameObject);    
            return;
        }
        Instance = this;
   
        inAnimation = true;                                         //setting it true as we start with roles reveal
    }

    private void Update()
    {
        if (!inAnimation && Input.GetKeyUp(KeyCode.Tab))
        {
            if(taskListBackground.GetComponent<RectTransform>().anchoredPosition.x == 0)
                taskListBackground.GetComponent<RectTransform>().anchoredPosition = new Vector2(-430, 0);
            else
                taskListBackground.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }

    public void OpenInterface(string interfaceName)
    {
        for (int i = 0; i < interfaces.Length; i++)
        {
            //Opening the required interface from the 'interfaces' array, and closing the rest
            if (interfaces[i].interfaceName == interfaceName)
            {
                interfaces[i].Open();
            }
            else if (interfaces[i].open)
            {
                CloseInterface(interfaces[i]);
            }
        }
    }

    public void CloseInterface(Interface interfaceName)
    {
        interfaceName.Close();
    }

    //function to reveal the roles
    public void RevealRole(string role)
    {
        FindObjectOfType<AudioManager>().Play("Roles");

        if (role == "imposter")
        {
            int count = 0;
            imposterRole.SetActive(true);

            Player[] allPlayers = PhotonNetwork.PlayerList;

            for (int i = 0; i < allPlayers.Length; i++)
            {
                if (GameController.Instance.impostersList.IndexOf(i + 1) != -1)
                    imposters[count++].SetActive(true);
            }
            
            killInactive.SetActive(true);
        }
        else
        {
            crewmateRole.SetActive(true);
            crewmate.SetActive(true);
        }
        
        StartCoroutine(RevealRoleInterfaceOpen());
    }

    IEnumerator RevealRoleInterfaceOpen()
    {
        while (roleRevealTimer < 5.0f)
        {
            roleRevealTimer += Time.deltaTime;
            
            yield return null;
        }
        
        OpenInterface("GameScreen");
        inAnimation = false;
        
        timer = MapSettings.Instance.emergengyCooldown.text;
        emergencyCooldown = float.Parse(timer.Substring(0, timer.IndexOf(' ')));
        
        EmergencyCooldown();
        KillCooldown();
    }
    
    //function to end the game
    public void GameOver(string result)
    {
        inAnimation = true;
        OpenInterface("GameOver");

        //stop looping sounds
        FindObjectOfType<AudioManager>().Stop("Walk");
        FindObjectOfType<AudioManager>().Stop("Siren");
        
        if (result == "CrewmatesWin" && !CreateAndJoinRooms.Instance.isImposter)
        {
            FindObjectOfType<AudioManager>().Play("CrewmateWin");

            victory.SetActive(true);
            crewmateGameOver.SetActive(true);
        }
        if (result == "CrewmatesWin" && CreateAndJoinRooms.Instance.isImposter)
        {
            FindObjectOfType<AudioManager>().Play("CrewmateWin");

            defeat.SetActive(true);

            Player[] allPlayers = PhotonNetwork.PlayerList;

            for (int i = 0; i < GameController.Instance.impostersList.Count; i++)
            {
                impostersGameOver[i].SetActive(true);
                imposterNameGameOver[i].text = allPlayers[GameController.Instance.impostersList[i] - 1].NickName;
            }
        }
        if (result == "ImpostersWin" && !CreateAndJoinRooms.Instance.isImposter)
        {
            FindObjectOfType<AudioManager>().Play("ImposterWin");

            defeat.SetActive(true);

            Player[] allPlayers = PhotonNetwork.PlayerList;

            for (int i = 0; i < GameController.Instance.impostersList.Count; i++)
            {
                impostersGameOver[i].SetActive(true);
                imposterNameGameOver[i].text = allPlayers[GameController.Instance.impostersList[i] - 1].NickName;
            }
        }
        if (result == "ImpostersWin" && CreateAndJoinRooms.Instance.isImposter)
        {
            FindObjectOfType<AudioManager>().Play("ImposterWin");

            victory.SetActive(true);

            Player[] allPlayers = PhotonNetwork.PlayerList;

            for (int i = 0; i < GameController.Instance.impostersList.Count; i++)
            {
                impostersGameOver[i].SetActive(true);
                imposterNameGameOver[i].text = allPlayers[GameController.Instance.impostersList[i] - 1].NickName;
            }
        }

        StartCoroutine(GameOverInterfaceOpen());
    }

    IEnumerator GameOverInterfaceOpen()
    {
        float temp = 0f;
        while (temp < 5.0f)
        {
            temp += Time.deltaTime;
            
            yield return null;
        }
        
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Lobby");

        Cursor.lockState = CursorLockMode.None;
    }

    //called when you get killed
    public void GotKilled(int killerSkin, int killedSkin)
    {
        FindObjectOfType<AudioManager>().Play("GotKilled");
        FindObjectOfType<AudioManager>().Stop("Walk");

        inAnimation = true;
        OpenInterface("GotKilled");
        
        killerBodies[killerSkin].SetActive(true);
        killedBodies[killedSkin].SetActive(true);
        
        StartCoroutine(GotKilledlInterfaceOpen());
    }
    
    IEnumerator GotKilledlInterfaceOpen()
    {
        while (gotKilledTimer < 2.0f)
        {
            gotKilledTimer += Time.deltaTime;
            
            yield return null;
        }
        
        OpenInterface("GameScreen");
        inAnimation = false;
    }
    
    //called when a body is reported
    public void BodyReported(string reportedSkin)
    {
        FindObjectOfType<AudioManager>().Play("Emergency");
        FindObjectOfType<AudioManager>().Stop("Walk");
        FindObjectOfType<AudioManager>().Stop("Siren");

        useActive.SetActive(false);
        bodyReportedTimer = 0f;
        
        inAnimation = true;
        OpenInterface("BodyReported");
        
        reportedBodies[int.Parse(reportedSkin)].SetActive(true);

        StartCoroutine(BodyReportedInterfaceOpen(reportedSkin));
        StartCoroutine(VotingScreenData());
    }
    
    IEnumerator BodyReportedInterfaceOpen(string reportedSkin)
    {
        while (bodyReportedTimer < 2.0f)
        {
            bodyReportedTimer += Time.deltaTime;
            
            yield return null;
        }

        OpenInterface("VotingPanel");
        discussionTimePanel.SetActive(true);
        
        reportedBodies[int.Parse(reportedSkin)].SetActive(false);

        timer = MapSettings.Instance.discussionTime.text;
        discussionTimer = float.Parse(timer.Substring(0, timer.IndexOf(' ')));
        
        timer = MapSettings.Instance.votingTime.text;
        votingTimer = float.Parse(timer.Substring(0, timer.IndexOf(' ')));;
        
        StartCoroutine(DiscussionTime());
    }
    
    public void EmergencyCooldown()
    {
        StartCoroutine(EmergencyCooldownCountdown());
    }
    
    IEnumerator EmergencyCooldownCountdown()
    {
        while (emergencyCooldown >= 0.0f)
        {
            emergencyCooldown -= Time.deltaTime;
            
            EmergencyText.Instance.SetTimer(emergencyCooldown);
            
            yield return null;
        }
    }
    
    public void KillCooldown()
    {
        if(CreateAndJoinRooms.Instance.isImposter)
            StartCoroutine(KillCooldownCountdown());
    }
    
    IEnumerator KillCooldownCountdown()
    {
        string temp = MapSettings.Instance.killCooldown.text;
        Players.killCooldown = float.Parse(temp.Substring(0, timer.IndexOf(' ')));
        
        while (Players.killCooldown >= 0.0f)
        {
            killCooldownTimer.text = Players.killCooldown.ToString("F0") + " s";
            if (killCooldownTimer.text == "0 s")
                killCooldownTimer.text = "";
            
            Players.killCooldown -= Time.deltaTime;

            yield return null;
        }
    }

    //called when an emergency meeting is called
    public void EmergencyMeetingCalled()
    {
        FindObjectOfType<AudioManager>().Play("Emergency");
        FindObjectOfType<AudioManager>().Stop("Walk");

        useActive.SetActive(false);
        emergencyCalledTimer = 0f;
        
        inAnimation = true;
        OpenInterface("EmergencyMeetingCalled");

        StartCoroutine(EmergencyMeetingCalledInterfaceOpen());
        StartCoroutine(VotingScreenData());
    }
    
    IEnumerator EmergencyMeetingCalledInterfaceOpen()
    {
        while (emergencyCalledTimer <= 3.0f)
        {
            emergencyCalledTimer += Time.deltaTime;

            yield return null;
        }
        OpenInterface("VotingPanel");
        discussionTimePanel.SetActive(true);
        
        timer = MapSettings.Instance.discussionTime.text;
        discussionTimer = float.Parse(timer.Substring(0, timer.IndexOf(' ')));
        
        timer = MapSettings.Instance.votingTime.text;
        votingTimer = float.Parse(timer.Substring(0, timer.IndexOf(' ')));
        
        StartCoroutine(DiscussionTime());
    }

    IEnumerator DiscussionTime()
    {
        resetPosition = true;
        justReported = true;
        
        while (discussionTimer >= 0f)
        {
            discussionTimeLeft.text = "Discussion Time: " + discussionTimer.ToString("F0") + " s";
            
            discussionTimer -= Time.deltaTime;
            
            yield return null;
        }

        discussionTimeLeft.text = "";
        
        discussionTimePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        
        StartCoroutine(VotingTime());
    }

    IEnumerator VotingTime()
    {
        resetPosition = false;
        Cursor.lockState = CursorLockMode.None;

        if(!CreateAndJoinRooms.Instance.isDead)
            skipVoteButton.SetActive(true);

        int aliveCount = 0, votedCount;

        for (int i = 0; i < infoIsDead.Count; i++)
        {
            if (infoIsDead[i] == "False")
            {
                aliveCount++;
                if (!CreateAndJoinRooms.Instance.isDead)
                    vote[i].SetActive(true);
            }
        }

        while (votingTimer >= 0f)
        {
            votedCount = 0;
            
            votingTimeLeft.text = "Voting Time: " + votingTimer.ToString("F0") + " s";
            
            votingTimer -= Time.deltaTime;

            for (int i = 0; i < iVoted.Length; i++)
            {
                if (iVoted[i].activeSelf)
                    votedCount++;
            }
            
            if (aliveCount == votedCount)
                votingTimer = -1f;
            
            yield return null;
        }
        
        if (!CreateAndJoinRooms.Instance.isDead && !hasVoted)
        {
            GameController.Instance.Voted(-2);
        }

        for (int i = 0; i < iVoted.Length; i++)
        {
            iVoted[i].SetActive(false);
        }
        
        votingTimeLeft.text = "";
        
        for (int i = 0; i < vote.Length; i++)
        {
            vote[i].SetActive(false);
        }

        for (int i = 0; i < voters.Length; i++)
        {
            voters[i].SetActive(true);
        }

        skipVoteButton.SetActive(false);
        skippedVotes.SetActive(true);
        skippedImage.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
            DecidedResultOfVoting();
        
        StartCoroutine(AfterVotingTime());
    }

    //called on the master client to decide the results of the voting
    private void DecidedResultOfVoting()
    {
        int votedPlayer = -1;
        int aliveCount = 0;
        string ejectedPlayerName;

        for (int i = 0; i < infoIsDead.Count; i++)
        {
            if (infoIsDead[i] == "False")
                aliveCount++;
        }
        
        for (int i = 0; i < voters.Length; i++)
        {
            if (voters[i].transform.childCount > aliveCount / 2)
            {
                votedPlayer = i;
                break;
            }
        }
        
        if(votedPlayer != -1)
        {
            if(MapSettings.Instance.confirmEject.text == "Yes")
            {
                if (GameController.Instance.impostersList.IndexOf(int.Parse(infoNumberInRoom[votedPlayer])) != -1)
                    ejectedPlayerName = infoName[votedPlayer] + " Was An Imposter.";
                else
                    ejectedPlayerName = infoName[votedPlayer] + " Was Not An Imposter.";
            }
            else
                ejectedPlayerName = infoName[votedPlayer] + " Was Ejected.";
            
            removedPlayerIndex = int.Parse(infoNumberInRoom[votedPlayer]);
            
            GameController.Instance.EjectedPlayer(int.Parse(infoNumberInRoom[votedPlayer]),
                ejectedPlayerName, int.Parse(infoSkin[votedPlayer]));
        }
        else
        {
            removedPlayerIndex = -1;
            ejectedPlayerName = "No One Was Ejected.";
            GameController.Instance.EjectedPlayer(-1, ejectedPlayerName, -1);
        }
    }

    IEnumerator AfterVotingTime()
    {
        afterVotingTime = 5f;

        while (afterVotingTime > 0f)
        {
            afterVotingTime -= Time.deltaTime;
            
            yield return null;
        }

        OpenInterface("VotingResult");

        StartCoroutine(VotingResult());
    }

    IEnumerator VotingResult()
    {
        votingResultTime = 5f;

        while (votingResultTime > 0f)
        {
            votingResultTime -= Time.deltaTime;
            
            yield return null;
        }

        justReported = false;
        
        GameController.Instance.changeCamera = false;
        
        Cursor.lockState = CursorLockMode.Locked;

        CreateAndJoinRooms.Instance.hasReported = false;
        CreateAndJoinRooms.Instance.hasCalledEmergency = false;

        OpenInterface("GameScreen");
        inAnimation = false;
        
        if(SabotageManager.Instance.reactorSabotaged)
            FindObjectOfType<AudioManager>().Play("Siren");

        Players.isDoingTask = false;
        
        if(removedPlayerIndex != -1 && PhotonNetwork.IsMasterClient)
        {
            if (GameController.Instance.impostersList.IndexOf(removedPlayerIndex) != -1)
                GameController.Instance.RemovePlayer("Imposter");
            else
                GameController.Instance.RemovePlayer("Crewmate");
        }
        
        KillCooldown();
        
        if(!CreateAndJoinRooms.Instance.isDead)
            SabotageManager.Instance.ResetTimer();

        if (SceneManager.GetActiveScene().name == "Polus")
        {
            GameController.Instance.polusEjection.transform.position = new Vector3(238.9f, 13f, 85.9f);
            GameController.Instance.polusEjection.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            for (int i = 0; i < 20; i++)
            {
                GameController.Instance.polusEjectionBodies[i].SetActive(false);
            }
        }

        
        if (SceneManager.GetActiveScene().name == "TheSkeld")
        {
            GameController.Instance.theSkeldEjection.transform.position = new Vector3(100f, 0f, 237f);
            GameController.Instance.theSkeldEjection.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            GameController.Instance.skeldEjectionBackground.transform.position = new Vector3(35f, -57f, 276f);

            for (int i = 0; i < 20; i++)
            {
                GameController.Instance.theSkeldEjectionBodies[i].SetActive(false);
            }
        }

        if (SceneManager.GetActiveScene().name == "MiraHQ")
        {
            GameController.Instance.miraHQEjection.transform.position = new Vector3(167f, 75f, 237f);
            GameController.Instance.miraHQEjection.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            GameController.Instance.miraHQEjectionBackground.transform.position = new Vector3(35f, -262f, 276f);

            for (int i = 0; i < 20; i++)
            {
                GameController.Instance.miraHQEjectionBodies[i].SetActive(false);
            }
        }
        

        ClearData();

        timer = MapSettings.Instance.emergengyCooldown.text;
        emergencyCooldown = float.Parse(timer.Substring(0, timer.IndexOf(' ')));
        
        EmergencyCooldown();
    }
    
    //called to clear all the discussion/voting screen data for the next round
    private void ClearData()
    {
        for (int i = 0; i < infoName.Count; i++)
        {
            foreach(Transform child in names[i].transform)
            {
                if (child.name.IndexOf("Temp") != -1)
                {
                    Destroy(child.gameObject);
                }

                if (child.name == "Voters")
                {
                    foreach (Transform voterChild in child.transform)
                    {
                        Destroy(voterChild.gameObject);
                    }
                }
            }
            
            playerName[i].GetComponent<TMP_Text>().color = Color.black;
            namePlateHighlight[i].SetActive(false);
            vote[i].SetActive(false);
            voters[i].SetActive(false);
            reported[i].SetActive(false);
            calledEmergency[i].SetActive(false);
            names[i].SetActive(false);
        }   
        
        foreach (Transform child in skippedVotes.transform)
        {
            Destroy(child.gameObject);
        }
            
        skippedVotes.SetActive(false);
        skippedImage.SetActive(false);
        
        infoName = new List<string>();
        infoNumberInRoom = new List<string>();
        infoIsDead = new List<string>();
        infoSkin = new List<string>();
        infoHasReported = new List<string>();
        infoHasCalledEmergency = new List<string>();

        hasVoted = false;
    }
    
    IEnumerator VotingScreenData()
    {
        while (infoName.Count < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            yield return null;
        }
        
        for (int i = 0; i < infoNumberInRoom.Count - 1; i++)
        {
            for (int j = 0; j < infoNumberInRoom.Count - 1 - i; j++)
            {
                string info1 = infoNumberInRoom[j], info2 = infoNumberInRoom[j + 1];
               
                if (int.Parse(info1) > int.Parse(info2))
                {
                    temp = infoName[j];
                    infoName[j] = infoName[j + 1];
                    infoName[j + 1] = temp;
                    
                    temp = infoNumberInRoom[j];
                    infoNumberInRoom[j] = infoNumberInRoom[j + 1];
                    infoNumberInRoom[j + 1] = temp;
                    
                    temp = infoIsDead[j];
                    infoIsDead[j] = infoIsDead[j + 1];
                    infoIsDead[j + 1] = temp;
                    
                    temp = infoSkin[j];
                    infoSkin[j] = infoSkin[j + 1];
                    infoSkin[j + 1] = temp;
                    
                    temp = infoHasReported[j];
                    infoHasReported[j] = infoHasReported[j + 1];
                    infoHasReported[j + 1] = temp;
                    
                    temp = infoHasCalledEmergency[j];
                    infoHasCalledEmergency[j] = infoHasCalledEmergency[j + 1];
                    infoHasCalledEmergency[j + 1] = temp;
                }
            }
        }
        for (int i = 0; i < infoIsDead.Count ; i++)
        {
            for (int j = 0; j < infoIsDead.Count - 1 - i; j++)
            {
                if (infoIsDead[j] == "True")
                {
                    temp = infoName[j];
                    infoName[j] = infoName[j + 1];
                    infoName[j + 1] = temp;
                    
                    temp = infoNumberInRoom[j];
                    infoNumberInRoom[j] = infoNumberInRoom[j + 1];
                    infoNumberInRoom[j + 1] = temp;
                    
                    temp = infoIsDead[j];
                    infoIsDead[j] = infoIsDead[j + 1];
                    infoIsDead[j + 1] = temp;
                    
                    temp = infoSkin[j];
                    infoSkin[j] = infoSkin[j + 1];
                    infoSkin[j + 1] = temp;
                    
                    temp = infoHasReported[j];
                    infoHasReported[j] = infoHasReported[j + 1];
                    infoHasReported[j + 1] = temp;
                    
                    temp = infoHasCalledEmergency[j];
                    infoHasCalledEmergency[j] = infoHasCalledEmergency[j + 1];
                    infoHasCalledEmergency[j + 1] = temp;
                }
            }
        }

        for (int i = 0; i < infoName.Count; i++)
        {
            names[i].SetActive(true);
            
            playerName[i].GetComponent<TMP_Text>().text = infoName[i];

            if (GameController.Instance.impostersList.IndexOf(CreateAndJoinRooms.Instance.myNumberInRoom) != -1)
            {
                if (GameController.Instance.impostersList.IndexOf(int.Parse(infoNumberInRoom[i])) != -1)
                    playerName[i].GetComponent<TMP_Text>().color = Color.red;
            }
            
            if(infoIsDead[i] == "True")
            {
                namePlateHighlight[i].SetActive(true);
            }

            Instantiate(playerPhotosPrefab[int.Parse(infoSkin[i])], names[i].transform);

            if (infoHasReported[i] == "True")
            {
                reported[i].SetActive(true);
            }
            
            if (infoHasCalledEmergency[i] == "True")
            {
                calledEmergency[i].SetActive(true);
            }
        }
    }

    //called when you have voted
    public void IVoted()
    { 
        for (int i = 0; i < vote.Length; i++)
        {
            vote[i].SetActive(false);
        }
        
        skipVoteButton.SetActive(false);
        hasVoted = true;
    }
        
    public void AddVote(int votedTo, int voter, int voterIndex)
    {
        if(votedTo != -2)
            iVoted[voterIndex].SetActive(true);
        
        if(votedTo != -1 && votedTo != -2)
            Instantiate(voterPhotosPrefab[voter], voters[votedTo].transform);
        else
            Instantiate(voterPhotosPrefab[voter], skippedVotes.transform);
    }

    //used to set the volume same as the player chose
    public void SetVolume()
    {
        AudioListener.volume = PlayerValues.Instance.masterVolume;
    }
}