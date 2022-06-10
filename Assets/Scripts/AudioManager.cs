using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0, 1)] public float volume;
    [Range(-3, 3)] public float pitch;
    public bool loop;
    [HideInInspector] public AudioSource source;

    public bool isMusic;
    public bool isUI;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public Sound[] sounds;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play("Menu");
    }

    public void Play(string _name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);

        if (s == null)
        {
            Debug.LogWarning("Sound " + _name + " not found!");
            return;
        }
        
        s.source.Play();
    }

    public void Stop(string _name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);

        if (s == null)
        {
            Debug.LogWarning("Sound " + _name + " not found!");
            return;
        }
        
        s.source.Stop();
    }
    
    public void SetPitch(string _name, float _pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _name);

        if (s == null)
        {
            Debug.LogWarning("Sound " + _name + " not found!");
            return;
        }

        s.source.pitch = _pitch;
    }

    public void SetVolume(bool _isMusic, float _vol)
    {
        foreach (var s in sounds)
        {
            if (!s.isUI)
            {
                if (s.isMusic == _isMusic)
                    s.source.volume = _vol;
            }
        }
    }

    public float GetMusicVolume()
    {
        foreach (var s in sounds)
        {
            if (s.isMusic)
                return s.source.volume;
        }

        return 1.0f;
    }
    
    public float GetSFXVolume()
    {
        foreach (var s in sounds)
        {
            if (!s.isMusic)
                return s.source.volume;
        }

        return 1.0f;
    }
}