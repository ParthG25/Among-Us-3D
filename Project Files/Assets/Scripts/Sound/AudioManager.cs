using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] sounds;
    
    // Use this for awake method
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            
            //assigning setting values to audio clips from unity windows
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        FindObjectOfType<AudioManager>().Play("Ambience");
    }

    //function to play a sound
    public void Play(string name)
    {
        //If sound is found in the array, then plays it, else returns out of the function
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: \"" + name + "\" not found");
            return;
        }
        s.source.Play();
    }

     //function to stop a sound
    public void Stop(string name)
    {
        //If sound is found in the array, then plays it, else returns out of the function
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: \"" + name + "\" not found");
            return;
        }
        s.source.Stop();
    }
}