using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusic : MonoBehaviour
{
    public static AmbientMusic Instance { get; private set; }

    private AudioSource _audio;

    [SerializeField] private AudioClip levelIntro;
    [SerializeField] private AudioClip loopMelody1;
    [SerializeField] private AudioClip loopMelody2;
    [SerializeField] private AudioClip loopMelody3;
    [SerializeField] private AudioClip loopMelody4;
    [SerializeField] private AudioClip loopMelody5;
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
            case AmbientMelodies.level_intro:
                _audio.Stop();
                _audio.volume = 0.5f;
                _audio.clip = levelIntro;
                _audio.Play();
                break;

            case AmbientMelodies.loop_melody1:
                _audio.Stop();                
                _audio.volume = 0.3f;
                _audio.loop = true;
                _audio.clip = loopMelody1;
                _audio.Play();
                break;

            case AmbientMelodies.loop_melody2:
                _audio.Stop();
                _audio.volume = 0.3f;
                _audio.loop = true;
                _audio.clip = loopMelody2;
                _audio.Play();
                break;

            case AmbientMelodies.loop_melody3:
                _audio.Stop();
                _audio.pitch = 0.96f;
                _audio.volume = 0.2f;
                _audio.loop = true;
                _audio.clip = loopMelody3;
                _audio.Play();
                break;

            case AmbientMelodies.loop_melody4:
                _audio.Stop();
                _audio.volume = 0.2f;
                _audio.loop = true;
                _audio.clip = loopMelody4;
                _audio.Play();
                break;

            case AmbientMelodies.loop_melody5:
                _audio.Stop();
                _audio.pitch = 0.96f;
                _audio.volume = 0.2f;
                _audio.loop = true;
                _audio.clip = loopMelody5;
                _audio.Play();
                break;

            case AmbientMelodies.forest:
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
        StartCoroutine(playScenario1());
    }
    private IEnumerator playScenario1()
    {
        int rows = 0;

        while (true)
        {
            for (int i = 0; i < 100; i++)
            {
                if (!_audio.isPlaying) break;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            PlayAmbient(AmbientMelodies.loop_melody1);
            _audio.loop = false;
            _audio.volume = 0.2f;
            yield return new WaitForSeconds(7.5f);

            for (int i = 0; i < 100; i++)
            {
                if (!_audio.isPlaying) break;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            PlayAmbient(AmbientMelodies.loop_melody2);
            _audio.loop = false;
            _audio.volume = 0.2f;
            yield return new WaitForSeconds(7.5f);

            rows++;

            if (rows == 3)
            {
                rows = 0;
                yield return new WaitForSeconds(15f);
            }

        }
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
