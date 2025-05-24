using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public static SpawnPoints Instance;

    public Transform[] polusSpawnPoints;            //Storing spawn points in Polus map
    public Transform[] polusEmergencyPoints;        //Storing emergency spawn points in Polus map
    public Transform[] theSkeldSpawnPoints;         //Storing spawn points in TheSkeld map
    public Transform[] miraHQSpawnPoints;           //Storing spawn points in MiraHQ map
    public Transform[] miraHQEmergencyPoints;       //Storing emergency spawn points in MiraHQ map
    
    private void Awake()
    {
        //making sure only once instance of this gameObject is there
        if (Instance)               
        {
            Destroy(gameObject);    
            return;
        }      
        Instance = this;                    
    }
}