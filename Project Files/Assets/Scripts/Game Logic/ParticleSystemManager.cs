using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    //creating an instance of this gameObject
    public static ParticleSystemManager Instance;

    //arrays to store the particle effects
    public ParticleSystem[] smokeLaboratory, smokeAdmin, smokeDecontamination;

    void Awake()
    {
        //making sure only one instance of this gameObject is there
        if (Instance)                
        {
            Destroy(gameObject);    
            return;
        }
        Instance = this; 
    }
}
