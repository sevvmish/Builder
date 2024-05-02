using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class MainMenuUI : MonoBehaviour
{
    [Header("buttons")]
    [SerializeField] private Button walkthrough;
    [SerializeField] private TextMeshProUGUI walkthroughText;
    [SerializeField] private Button custom;
    [SerializeField] private TextMeshProUGUI customText;
    [SerializeField] private Button reset;

    [Header("levels")]
    [SerializeField] private LevelPreviewUI levelPreview;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button leftButtonScroll;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button rightButtonScroll;
    [SerializeField] private GameObject lockedIcon;
    [SerializeField] private GameObject doneIcon;
    [SerializeField] private Sprite yellow;
    [SerializeField] private Sprite grey;
    [SerializeField] private TextMeshProUGUI blockLevelText;

    [Header("player lvl")]
    [SerializeField] private TextMeshProUGUI PlayerLevel;
    [SerializeField] private TextMeshProUGUI currText;
    [SerializeField] private TextMeshProUGUI lvlText;

    [Header("tutorial")]
    [SerializeField] private GameObject customGameHint;
    [SerializeField] private TextMeshProUGUI customGameHintText;
    [SerializeField] private GameObject walkGameHint;
    [SerializeField] private TextMeshProUGUI walkGameHintText;

    private SoundUI sounds;

    private void Awake()
    {   
        if (Globals.IsMobile)
        {
            QualitySettings.antiAliasing = 2;

            if (Globals.IsLowFPS)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
            else
            {
                QualitySettings.shadows = ShadowQuality.HardOnly;
                QualitySettings.shadowResolution = ShadowResolution.Medium;
            }

        }
        else
        {
            QualitySettings.antiAliasing = 4;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        sounds = SoundUI.Instance;
        lockedIcon.SetActive(false);
        doneIcon.SetActive(false);

        rightButtonScroll.gameObject.SetActive(false);
        leftButtonScroll.gameObject.SetActive(false);

        customGameHint.SetActive(false);
        walkGameHint.SetActive(false);

        leftButton.onClick.AddListener(() =>
        {
            if (levelPreview.CurrentLevelNumber < 1) return;
            sounds.PlayUISound(SoundsUI.click);
            levelPreview.CurrentLevelMinus();
            updateLevelInfo();
        });

        leftButtonScroll.onClick.AddListener(() =>
        {
            if (levelPreview.CurrentLevelNumber == Globals.MainPlayerData.Level) return;
            sounds.PlayUISound(SoundsUI.click);
            levelPreview.ScrollToCurrent();
            updateLevelInfo();
        });

        rightButtonScroll.onClick.AddListener(() =>
        {
            if (levelPreview.CurrentLevelNumber == Globals.MainPlayerData.Level) return;
            sounds.PlayUISound(SoundsUI.click);
            levelPreview.ScrollToCurrent();
            updateLevelInfo();
        });

        rightButton.onClick.AddListener(() =>
        {
            if (levelPreview.CurrentLevelNumber >= levelPreview.MaxLevels) return;
            sounds.PlayUISound(SoundsUI.click);
            levelPreview.CurrentLevelPlus();
            updateLevelInfo();
        });
                
        if (Globals.IsInitiated)
        {
            localize();
            playWhenInitialized();
            
        }

        walkthrough.onClick.AddListener(() =>
        {
            sounds.PlayUISound(SoundsUI.success3);
            Globals.CurrentLevel = levelPreview.CurrentLevelNumber;
            walkthrough.interactable = false;
            Globals.IsWalkthroughEnabled = true;
            StartCoroutine(playStartLevel());
        });

        reset.onClick.AddListener(() =>
        {
            resetData();
        });

        custom.onClick.AddListener(() =>
        {
            sounds.PlayUISound(SoundsUI.success3);
            custom.interactable = false;
            Globals.IsWalkthroughEnabled = false;
            StartCoroutine(playStartLevel());
        });
    }

    private void resetData()
    {
        Globals.MainPlayerData = new PlayerData();
        SaveLoadManager.Save();
        Globals.CurrentLevel = 0;
        Globals.IsMainMenuTutorial = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void updateLevelInfo()
    {
        if (levelPreview.CurrentLevelNumber < 1)
        {
            leftButton.gameObject.SetActive(false);
        }
        else
        {
            leftButton.gameObject.SetActive(true);
        }

        if (levelPreview.CurrentLevelNumber >= (levelPreview.MaxLevels - 1))
        {
            rightButton.gameObject.SetActive(false);
        }
        else
        {
            rightButton.gameObject.SetActive(true);
        }

        if (levelPreview.CurrentLevelNumber > Globals.MainPlayerData.Level)
        {
            lockedIcon.SetActive(true);
            rightButtonScroll.gameObject.SetActive(false);
            leftButtonScroll.gameObject.SetActive(true);

            blockLevelText.gameObject.SetActive(true);
            blockLevelText.text = levelPreview.CurrentLevelNumber + " " + Globals.Language.Level;

            doneIcon.SetActive(false);
            
            walkthrough.interactable = false;
            walkthrough.GetComponent<Image>().sprite = grey;
            walkthroughText.text = Globals.Language.Play;
        }
        else if ((Globals.MainPlayerData.Level == levelPreview.MaxLevels))
        {
            lockedIcon.SetActive(false);
            rightButtonScroll.gameObject.SetActive(false);
            leftButtonScroll.gameObject.SetActive(false);

            blockLevelText.gameObject.SetActive(false);
            doneIcon.SetActive(true);

            walkthrough.interactable = true;
            walkthrough.GetComponent<Image>().sprite = yellow;
            walkthroughText.text = Globals.Language.PlayAgain;
        }
        else if ((levelPreview.CurrentLevelNumber < Globals.MainPlayerData.Level))
        {
            lockedIcon.SetActive(false);
            rightButtonScroll.gameObject.SetActive(true);
            leftButtonScroll.gameObject.SetActive(false);

            blockLevelText.gameObject.SetActive(false);
            doneIcon.SetActive(true);

            walkthrough.interactable = true;
            walkthrough.GetComponent<Image>().sprite = yellow;
            walkthroughText.text = Globals.Language.PlayAgain;
        }
        else if (levelPreview.CurrentLevelNumber == Globals.MainPlayerData.Level)
        {
            lockedIcon.SetActive(false);
            rightButtonScroll.gameObject.SetActive(false);
            leftButtonScroll.gameObject.SetActive(false);

            blockLevelText.gameObject.SetActive(false);
            doneIcon.SetActive(false);

            walkthrough.interactable = true;
            walkthrough.GetComponent<Image>().sprite = yellow;
            walkthroughText.text = Globals.Language.Play;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            resetData();
        }

        if (YandexGame.SDKEnabled && !Globals.IsInitiated)
        {
            Globals.IsInitiated = true;

            SaveLoadManager.Load();

            print("SDK enabled: " + YandexGame.SDKEnabled);
            Globals.CurrentLanguage = YandexGame.EnvironmentData.language;
            print("language set to: " + Globals.CurrentLanguage);

            Globals.IsMobile = Globals.IsMobileChecker();
            Globals.IsLowFPS = Globals.MainPlayerData.IsLowFPS;
            Globals.CurrentLevel = Globals.MainPlayerData.Level;
                        
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

            YandexGame.StickyAdActivity(!Globals.MainPlayerData.AdvOff);

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

    private IEnumerator playTutorial()
    {
        yield return new WaitForSeconds(2f);

        customGameHintText.text = Globals.Language.CustomGameHintText;
        walkGameHintText.text = Globals.Language.WalkGameHintText;

        sounds.PlayUISound(SoundsUI.pop);
        walkGameHint.SetActive(true);
        yield return new WaitForSeconds(3f);

        sounds.PlayUISound(SoundsUI.pop);
        customGameHint.SetActive(true);
        yield return new WaitForSeconds(1f);
        walkGameHint.SetActive(false);
        yield return new WaitForSeconds(5f);
        customGameHint.SetActive(false);
    }

    private void playWhenInitialized()
    {
        if (Globals.MainPlayerData.Level == 0)
        {
            Globals.IsWalkthroughEnabled = true;
            SceneManager.LoadScene("Gameplay");
            return;
        }


        ScreenSaver.Instance.ShowScreen();

        if (Globals.MainPlayerData.Level == 1 && !Globals.IsMainMenuTutorial)
        {
            Globals.IsMainMenuTutorial = true;
            StartCoroutine(playTutorial());
        }

        AmbientMusic.Instance.PlayScenario1();

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

        currText.text = Globals.Language.Curr;
        lvlText.text = Globals.Language.Level;
        PlayerLevel.text = Globals.MainPlayerData.Level.ToString();

        levelPreview.UpdateData();
        updateLevelInfo();

        if (levelPreview.CurrentLevelNumber == 0)
        {
            custom.interactable = false;
            custom.GetComponent<Image>().sprite = grey;
            customText.text = Globals.Language.CustomGame;
        }
        else
        {
            custom.interactable = true;
            custom.GetComponent<Image>().sprite = yellow;
            customText.text = Globals.Language.CustomGame;
        }

    }
    private IEnumerator playStartLevel()
    {
        ScreenSaver.Instance.HideScreen();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Gameplay");
    }

    private void localize()
    {
        if (Globals.Language == null)
        {
            Globals.Language = Localization.GetInstanse(Globals.CurrentLanguage).GetCurrentTranslation();
        }

        
    }
}
