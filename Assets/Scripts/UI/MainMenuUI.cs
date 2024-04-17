using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button walkthrough;
    [SerializeField] private Button custom;

    // Start is called before the first frame update
    void Start()
    {
        ScreenSaver.Instance.ShowScreen();

        if (Globals.IsInitiated)
        {
            playWhenInitialized();
            localize();
        }

        walkthrough.onClick.AddListener(() =>
        {
            walkthrough.interactable = false;
            Globals.IsWalkthroughEnabled = true;
            StartCoroutine(playStartLevel());
        });

        custom.onClick.AddListener(() =>
        {
            custom.interactable = false;
            Globals.IsWalkthroughEnabled = false;
            StartCoroutine(playStartLevel());
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (YandexGame.SDKEnabled && !Globals.IsInitiated)
        {
            Globals.IsInitiated = true;

            SaveLoadManager.Load();

            print("SDK enabled: " + YandexGame.SDKEnabled);
            Globals.CurrentLanguage = YandexGame.EnvironmentData.language;
            print("language set to: " + Globals.CurrentLanguage);

            Globals.IsMobile = Globals.IsMobileChecker();//= YandexGame.EnvironmentData.isMobile;
                        
            print("platform mobile: " + Globals.IsMobile);

            if (Globals.MainPlayerData.S == 1)
            {
                Globals.IsSoundOn = true;
                AudioListener.volume = 1;
            }
            else
            {
                Globals.IsSoundOn = false;
                AudioListener.volume = 0;
            }

            if (Globals.MainPlayerData.Mus == 1)
            {
                Globals.IsMusicOn = true;
            }
            else
            {
                Globals.IsMusicOn = false;
            }

            print("sound is: " + Globals.IsSoundOn);

            if (Globals.TimeWhenStartedPlaying == DateTime.MinValue)
            {
                Globals.TimeWhenStartedPlaying = DateTime.Now;
                Globals.TimeWhenLastInterstitialWas = DateTime.Now;
                Globals.TimeWhenLastRewardedWas = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
            }

            //TO DEL
            //Globals.MainPlayerData.Lvl = 10;

            localize();
            playWhenInitialized();
        }        
    }

    private void playWhenInitialized()
    {
        if (!Globals.MainPlayerData.IsZoomCorrected)
        {
            if (Globals.IsMobile)
            {
                Globals.MainPlayerData.Zoom = 60;
            }
            else
            {
                Globals.MainPlayerData.Zoom = 60;
            }
            Globals.MainPlayerData.IsZoomCorrected = true;
            SaveLoadManager.Save();
        }
        

        //StartCoroutine(playStartLevel());
    }
    private IEnumerator playStartLevel()
    {
        ScreenSaver.Instance.HideScreen();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Gameplay");
    }

    private void localize()
    {
        Globals.Language = Localization.GetInstanse(Globals.CurrentLanguage).GetCurrentTranslation();

    }
}
