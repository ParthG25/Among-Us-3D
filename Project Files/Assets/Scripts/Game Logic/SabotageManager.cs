using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SabotageManager : MonoBehaviour
{
    //making an instance of this gameObject
    public static SabotageManager Instance;
    
    //assigning a photon view to this game object
    private PhotonView PV;
    
    public GameObject[] sabotages;
    public GameObject ventInstructions;

    //stores the highlights for the sabotage buttons
    public Slider[] doorSabotageHighlights, criticalSabotageHighlights;

    //stores the completeion state of the tasks
    private bool[] myPolusTasks = new bool[29];
    private bool[] myTheSkeldTasks = new bool[23];
    private bool[] myMiraHQTasks = new bool[21];

    //stores the number of people pressing the left and the right reactor respectively
    public int leftReactorCount, rightReactorCount;
    public bool reactorSabotaged, lightsSabotaged;
    public float timer;

    //objects of the class Reactor
    public Reactor[] reactors;

    //objects of the class ReactorPalm
    public ReactorPalm[] palms;

    //object of the class FixLights
    public FixLights fixLights;

    public GameObject reactorIcon, electricityIcon;

    public bool reactorHeld;
    public string activeReactorSide;

    // Start is called before the first frame update
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        StartCoroutine(CriticalButtonHighlight());
    }

    //called by the local player if they are the imposter to update the sabotage status for all the imposters
    public void StartCooldown(int index)
    {
        PV.RPC("RPC_StartCooldown", RpcTarget.All, index);
    }

    //RPC called to start the button cooldown for all the imposters
    [PunRPC]
    private void RPC_StartCooldown(int index)
    {
        doorSabotageHighlights[index].value = 1;
        doorSabotageHighlights[index].gameObject.SetActive(true);
        StartCoroutine(ButtonHighlight(index));
    }

    //called by the imposter who initiated a sabotage    
    public void StartCriticalCooldown(int index)
    {
        PV.RPC("RPC_StartCriticalCooldown", RpcTarget.All, index);
    }

    //RPC called for executing the sabotage
    [PunRPC]
    private void RPC_StartCriticalCooldown(int index)
    {
        //Sabotaging the lights
        if (index == 0)
        {
            //cut lights
            lightsSabotaged = true;
            InterfaceManager.Instance.importantMessage.text = "Fix Lights!";

            if(!CreateAndJoinRooms.Instance.isImposter)
            {
                RenderSettings.fogStartDistance = 0;
                RenderSettings.fogEndDistance = 15;
                Camera.current.GetComponent<Camera>().farClipPlane = 17;
            }
            
            electricityIcon.SetActive(true);
            
            fixLights.OnEnable();
        }
        
        //Sabotaging the Reactors
        if (index == 1)
        {
            //reactor meltdown
            reactorSabotaged = true;

            if (SceneManager.GetActiveScene().name == "Polus")
            {
                InterfaceManager.Instance.importantMessage.text = "Reset Reactor! (60 s)";

                for (int i = 0; i < myPolusTasks.Length; i++)
                {
                    myPolusTasks[i] = TaskManager.Instance.activeTasks[i];
                }

                for (int i = 0; i < TaskManager.Instance.activeTasks.Length; i++)
                {
                    TaskManager.Instance.activeTasks[i] = false;
                }

            }

            if (SceneManager.GetActiveScene().name == "TheSkeld")
            {
                InterfaceManager.Instance.importantMessage.text = "Reset Reactor! (30 s)";

                for (int i = 0; i < myTheSkeldTasks.Length; i++)
                {
                    myTheSkeldTasks[i] = TaskManager.Instance.activeTasks[i];
                }

                for (int i = 0; i < TaskManager.Instance.activeTasks.Length; i++)
                {
                    TaskManager.Instance.activeTasks[i] = false;
                }
            }

            if (SceneManager.GetActiveScene().name == "MiraHQ")
            {
                InterfaceManager.Instance.importantMessage.text = "Reset Reactor! (45 s)";

                for (int i = 0; i < myMiraHQTasks.Length; i++)
                {
                    myMiraHQTasks[i] = TaskManager.Instance.activeTasks[i];
                }

                for (int i = 0; i < TaskManager.Instance.activeTasks.Length; i++)
                {
                    TaskManager.Instance.activeTasks[i] = false;
                }
            }

            StartCoroutine(ReactorMeltdownTimer());
            
            reactorIcon.SetActive(true);
        }
        
        //covering the critial sabotage buttons woth the highlights
        for (int i = 0; i < criticalSabotageHighlights.Length; i++)
        {
            criticalSabotageHighlights[i].value = 1;
            criticalSabotageHighlights[i].gameObject.SetActive(true);
        }
    }

    //called when we need to reset the sabotage timer
    public void ResetTimer()
    {
        PV.RPC("RPC_ResetTimer", RpcTarget.All, timer);
    }
   
    //called by the master client when we need to reset the sabotage timer
    [PunRPC]
    private void RPC_ResetTimer(float index)
    {
        timer = index;
    }

    //called by the local player if they fixed the lights
    public void FixedLights()
    {
        PV.RPC("RPC_FixedLights", RpcTarget.All);
    }

    //RPC called when the lights have been fixed
    [PunRPC]
    private void RPC_FixedLights()
    {
        fixLights.sabotageFixed = true;
        lightsSabotaged = false;
        
        InterfaceManager.Instance.importantMessage.text = "";

        if(!CreateAndJoinRooms.Instance.isImposter)
        {
            RenderSettings.fogStartDistance = 15;
            RenderSettings.fogEndDistance = 40;
            Camera.current.GetComponent<Camera>().farClipPlane = 42;

        }
            
        StartCoroutine(CriticalButtonHighlight());
        electricityIcon.SetActive(false);
    }

    //called when the reactor has been sabotaged
    IEnumerator ReactorMeltdownTimer()
    {
        FindObjectOfType<AudioManager>().Play("Siren");

        if (SceneManager.GetActiveScene().name == "Polus")
            timer = 60.0f;
        if (SceneManager.GetActiveScene().name == "TheSkeld")
            timer = 30.0f;
        if (SceneManager.GetActiveScene().name == "MiraHQ")
            timer = 45.0f;

        while (timer > 0.0f)
        {
            if(!InterfaceManager.Instance.inAnimation)
                timer -= Time.deltaTime;
            
            InterfaceManager.Instance.importantMessage.text = "Reset Reactor! (" + timer.ToString("F0") +" s)";
            
            yield return null;
        }
        
        //if the timer runs out and both the plams haven't been touched simultaneously, imoposters win
        if (leftReactorCount == 0 || rightReactorCount == 0)
        {
            InterfaceManager.Instance.GameOver("ImpostersWin");
        }
        else
        {
            reactorIcon.SetActive(false);
            
            for (int i = 0; i < reactors.Length; i++)
            {
                reactors[i].sabotageFixed = true;
                palms[i].GetComponent<Image>().color = Color.red;
            }

            StartCoroutine(CriticalButtonHighlight());
            reactorSabotaged = false;
            InterfaceManager.Instance.importantMessage.text = "";
            leftReactorCount = 0;
            rightReactorCount = 0;

            if (SceneManager.GetActiveScene().name == "Polus")
            {
                for (int i = 0; i < myPolusTasks.Length; i++)
                {
                    TaskManager.Instance.activeTasks[i] = myPolusTasks[i];
                }
            }

            if (SceneManager.GetActiveScene().name == "TheSkeld")
            {
                for (int i = 0; i < myTheSkeldTasks.Length; i++)
                {
                    TaskManager.Instance.activeTasks[i] = myTheSkeldTasks[i];
                }
            }

            if (SceneManager.GetActiveScene().name == "MiraHQ")
            {
                for (int i = 0; i < myMiraHQTasks.Length; i++)
                {
                    TaskManager.Instance.activeTasks[i] = myMiraHQTasks[i];
                }
            }
        }
    }
    
    //starting the cooldown for the door sabotage highlights
    IEnumerator ButtonHighlight(int index)
    {
        float timer = 20.0f;
        
        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            doorSabotageHighlights[index].value = timer / 20.0f; 
                
            yield return null;
        }
        
        doorSabotageHighlights[index].gameObject.SetActive(false);
    }
    
    //starting the cooldwon for the critical sabotage highlights
    IEnumerator CriticalButtonHighlight()
    {
        float timer = 20.0f;
        
        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;
           
            for (int i = 0; i < criticalSabotageHighlights.Length; i++)
                criticalSabotageHighlights[i].value = timer / 20.0f;

            yield return null;
        }
        
        for (int i = 0; i < criticalSabotageHighlights.Length; i++)
            criticalSabotageHighlights[i].gameObject.SetActive(false);
    }

    //called by the local player when he presses a plam
    public void Increment(string side)
    {
        PV.RPC("RPC_Increment", RpcTarget.All, side);
    }
    
    //called by the local player when he releases a plam
    public void Decrement(string side)
    {
        PV.RPC("RPC_Decrement", RpcTarget.All, side);
    }
    
    //RPC called when a palm is pressed
    [PunRPC]
    private void RPC_Increment(string name)
    {
        if (name == "left")
            leftReactorCount++;
        if (name == "right")
            rightReactorCount++;
        
        if(leftReactorCount > 0 && rightReactorCount > 0)
        {
            FindObjectOfType<AudioManager>().Stop("Siren");

            timer = 0f;
        }
    }
    
    //RPC called when a palm is released
    [PunRPC]
    private void RPC_Decrement(string name)
    {
        if (name == "left")
            leftReactorCount--;
        if (name == "right")
            rightReactorCount--;
    }
}