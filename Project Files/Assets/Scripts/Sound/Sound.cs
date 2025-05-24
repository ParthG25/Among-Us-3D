using UnityEngine;

[System.Serializable]
public class Sound
{
    //name of the sound
    public string name;
    
    public AudioClip clip;

    //settings for audio clips
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public bool loop;

    [HideInInspector] 
    public AudioSource source;
}