using UnityEngine;

public class PlayerValues : MonoBehaviour
{
    public bool isUnlocked;
    public string playerName;
    public float mouseSensitivity;
    public float masterVolume;
    public int bodySkin;
    public int headSkin;
    public int faceSkin;
    
    public static PlayerValues Instance;

    private void Awake()
    {
        //making sure only one instance of this gameObject is there
        if (Instance)                
        {
            Destroy(gameObject);  
            return;
        }
        DontDestroyOnLoad(gameObject);        
        Instance = this;           

        //setting the frame rate of the game to 60
        Application.targetFrameRate = 60;       
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        //loads player data on start if data exists, else assigns default values
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            playerName = data.playerName;
            mouseSensitivity = data.mouseSensitivity;
            masterVolume = data.masterVolume;
            bodySkin = data.bodySkin;
            headSkin = data.headSkin;
            faceSkin = data.faceSkin;
            isUnlocked = data.isUnlocked;
        }
        else
        {
            playerName = "";
            mouseSensitivity = 100f;
            masterVolume = 1;
            bodySkin = 0;
            headSkin = 0;
            faceSkin = 0;
            isUnlocked = false;
        }

        AudioListener.volume = PlayerValues.Instance.masterVolume;
        
        //opening the home screen if the user has previously entered the password
        if(isUnlocked)
            MenuManager.Instance.OpenMenu("HomeScreen");
    }

    //called to save all the player values
    public void SaveValues()
    {
        SaveSystem.SavePlayer(Instance);
    }
}
