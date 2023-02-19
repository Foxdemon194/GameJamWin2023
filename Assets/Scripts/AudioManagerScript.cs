using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManagerScript : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManagerScript instance;

    private void Update()
    {
        if (VineGrowing.isGrowing)
        {
            Play("Growing");
        }
        else
        {
            //StopS("Growing");

        }
    }

    private void Start()
    {
        Play("MainTheme");
    }

    private void Awake()
    {

        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name) //FindObjectOfType<AudioManagerScript>().Play("audio's name");
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.Log("No sound of name " + name + " was found");
            return;
        }
        s.source.Play();

    }
    
    public void StopS(string sound)//FindObjectOfType<AudioManagerScript>().StopS("audio's name")
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volume / 2f, s.volume / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitch/ 2f, s.pitch / 2f));

        s.source.Stop();
    }
}
