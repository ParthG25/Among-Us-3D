using UnityEngine;

public class VentManager : MonoBehaviour
{
    //instance of this script
    public static VentManager Instance;

    //stores the index of the active vent
    public int activeVent;

    //stores the network of the connected vents; an imposter can only vent through the connected ones
    public string ventNetwork;

    //stores true if the user is in a state to vent
    public bool canVent;

    //stores true if the imposter is in a vent
    public bool inVent;

    //stores the count of the number of players on the vent
    public int[] onVentCount = new int[0];

    //stores the transform of all the vents
    public Transform[] vents;
    public GameObject[] mapVentNetworks;

    private void Awake()
    {
        //making sure only one instance of this gameObject is there
        if (Instance)                
        {
            Destroy(gameObject);    
            return;
        }
        Instance = this;
    }

    //called when a imposter clicks on the vent icon on the map to change vents
    public void VentFromMap(int index)
    {
        activeVent = index;
        FindObjectOfType<AudioManager>().Play("VentTravel");
    }
}
