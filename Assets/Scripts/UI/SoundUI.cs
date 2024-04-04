using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundUI : MonoBehaviour
{
    public static SoundUI Instance { get; private set; }
    private AudioSource _audio;

    [SerializeField] private AudioClip Error1;
    [SerializeField] private AudioClip Error2;    
    [SerializeField] private AudioClip Click;
    [SerializeField] private AudioClip Pop;
    [SerializeField] private AudioClip Success1;
    [SerializeField] private AudioClip Success2;
    [SerializeField] private AudioClip Cash;
    [SerializeField] private AudioClip WoodBuild;
    [SerializeField] private AudioClip WoodDestroy;

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

    public void PlayBuild(Block b)
    {
        switch(b.MaterialType)
        {
            case MaterialTypes.wood:
                _audio.clip = WoodBuild;
                break;

            default:
                _audio.clip = WoodBuild;
                break;
        }

        _audio.Stop();
        _audio.pitch = 1;
        _audio.Play();
    }

    public void PlayDestroy(Block b)
    {
        switch (b.MaterialType)
        {
            case MaterialTypes.wood:
                _audio.clip = WoodDestroy;
                break;

            default:
                _audio.clip = WoodDestroy;
                break;
        }

        _audio.Stop();
        _audio.pitch = 1;
        _audio.Play();
    }

    public void PlayUISound(SoundsUI _type)
    {
        _audio.pitch = 1;

        switch (_type)
        {            
            case SoundsUI.error1:
                _audio.Stop();
                _audio.clip = Error1;
                _audio.Play();
                break;

            case SoundsUI.error2:
                _audio.Stop();
                _audio.clip = Error2;
                _audio.Play();
                break;

            case SoundsUI.pop:
                _audio.Stop();
                _audio.clip = Pop;
                _audio.Play();
                break;

            case SoundsUI.click:
                _audio.Stop();
                _audio.clip = Click;
                _audio.Play();
                break;

            
            case SoundsUI.success1:
                _audio.Stop();
                _audio.pitch = 1f;
                _audio.clip = Success1;
                _audio.Play();
                break;

            case SoundsUI.success2:
                _audio.Stop();
                _audio.pitch = 1f;
                _audio.clip = Success2;
                _audio.Play();
                break;

            case SoundsUI.cash:
                _audio.Stop();
                _audio.pitch = 1f;
                _audio.clip = Cash;
                _audio.Play();
                break;

        }
    }
}

public enum SoundsUI
{
    none,
    error1,
    error2,    
    pop,
    click,
    success1,
    success2,
    cash
}
