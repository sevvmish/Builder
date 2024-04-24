using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    public static AmbientMusic Instance { get; private set; }

    private AudioSource _audio;
        
    [SerializeField] private AudioClip forest;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _audio = GetComponent<AudioSource>();
    }

    public void StopAll()
    {
        StopAllCoroutines();
        _audio.Stop();
    }

    public void ContinuePlaying()
    {
        PlayScenario1();
    }


    public void PlayAmbient(AmbientMelodies _type)
    {
        if (!Globals.IsMusicOn || !Globals.IsSoundOn) return;

        
        _audio.pitch = 1;
        _audio.volume = 0.6f;

        switch (_type)
        {
            
            case AmbientMelodies.forest:
                _audio.Stop();
                _audio.volume = 0.3f;
                _audio.loop = true;
                _audio.clip = forest;
                _audio.Play();
                
                break;

            default:
                _audio.Stop();
                _audio.volume = 0.3f;
                _audio.loop = true;
                _audio.clip = forest;
                _audio.Play();
                break;

        }
    }

    public void PlayScenario1()
    {
        if (!Globals.IsMusicOn) return;
        PlayAmbient(AmbientMelodies.forest);
    }
}

public enum AmbientMelodies
{
    level_intro,
    loop_melody1,
    loop_melody2,
    loop_melody3,
    loop_melody4,
    loop_melody5,
    forest
}
