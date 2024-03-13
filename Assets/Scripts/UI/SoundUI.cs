using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundUI : MonoBehaviour
{
    public static SoundUI Instance { get; private set; }
    private AudioSource _audio;

    [SerializeField] private AudioClip ErrorClip;
    [SerializeField] private AudioClip Win;
    [SerializeField] private AudioClip Lose;
    [SerializeField] private AudioClip Click;
    [SerializeField] private AudioClip positiveSoundClip;
    [SerializeField] private AudioClip Tick;
    [SerializeField] private AudioClip Pop;
    [SerializeField] private AudioClip BeepTick;
    [SerializeField] private AudioClip BeepOut;
    [SerializeField] private AudioClip Success;
    [SerializeField] private AudioClip Success2;
    [SerializeField] private AudioClip Success3;
    [SerializeField] private AudioClip Cash;

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

    public void PlayUISound(SoundsUI _type)
    {
        _audio.pitch = 1;

        switch (_type)
        {
            case SoundsUI.tick:
                _audio.Stop();
                _audio.clip = Tick;
                _audio.Play();
                break;

            case SoundsUI.error:
                _audio.Stop();
                _audio.clip = ErrorClip;
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

            case SoundsUI.positive:
                _audio.Stop();
                _audio.clip = positiveSoundClip;
                _audio.Play();
                break;

            case SoundsUI.win:
                _audio.Stop();
                _audio.clip = Win;
                _audio.Play();
                break;

            case SoundsUI.lose:
                _audio.Stop();
                _audio.clip = Lose;
                _audio.Play();
                break;

            case SoundsUI.beep_tick:
                _audio.Stop();   
                _audio.pitch = 1.5f;
                _audio.clip = BeepTick;
                _audio.Play();
                break;

            case SoundsUI.beep_out:
                _audio.Stop();
                _audio.pitch = 0.8f;
                _audio.clip = BeepOut;
                _audio.Play();
                break;

            case SoundsUI.success:
                _audio.Stop();
                _audio.pitch = 1f;
                _audio.clip = Success;
                _audio.Play();
                break;

            case SoundsUI.success2:
                _audio.Stop();
                _audio.pitch = 1f;
                _audio.clip = Success2;
                _audio.Play();
                break;

            case SoundsUI.success3:
                _audio.Stop();
                _audio.pitch = 1f;
                _audio.clip = Success3;
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
    error,
    positive, 
    tick,
    pop,
    click,
    win,
    lose,
    beep_tick,
    beep_out, 
    success,
    cash,
    success2,
    success3
}
