using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button optionsB;
    [SerializeField] private Button exitB;
    [SerializeField] private Button continueB;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;

    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI exitText;
    [SerializeField] private TextMeshProUGUI soundText;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private Button resetCustomGame;
    [SerializeField] private GameObject resetCustomGameAllButton;
    [SerializeField] private TextMeshProUGUI resetCustomGameText;
    [SerializeField] private GameObject infoPanelReset;
    [SerializeField] private TextMeshProUGUI infoPanelResetText;
    [SerializeField] private Button makeReset;
    [SerializeField] private Button noReset;

    private Vector2 position;
    private RectTransform optionsRect;

    // Start is called before the first frame update
    void Start()
    {
        optionsRect = GetComponent<RectTransform>();
        position = optionsRect.anchoredPosition;

        continueText.text = Globals.Language.Continue;
        exitText.text = Globals.Language.Exit;
        soundText.text = Globals.Language.Sound;
        musicText.text = Globals.Language.Music;

        if (!GameManager.Instance.IsWalkthroughGame)
        {
            resetCustomGameAllButton.SetActive(true);
            resetCustomGameText.text = Globals.Language.ResetCustomGameText;
            infoPanelResetText.text = Globals.Language.InfoPanelResetText;
        }
        else
        {
            resetCustomGameAllButton.SetActive(false);
        }

        resetCustomGame.onClick.AddListener(() =>
        {
            SoundUI.Instance.PlayUISound(SoundsUI.click);
            infoPanelReset.gameObject.SetActive(true);
        });

        makeReset.onClick.AddListener(() =>
        {
            SoundUI.Instance.PlayUISound(SoundsUI.click);
            infoPanelReset.gameObject.SetActive(false);
            StartCoroutine(playReset());
        });

        noReset.onClick.AddListener(() =>
        {
            SoundUI.Instance.PlayUISound(SoundsUI.click);
            infoPanelReset.gameObject.SetActive(false);
        });

        if (!Globals.IsMobile)
        {
            optionsB.transform.localScale = Vector3.one * 0.7f;
        }
        optionsPanel.SetActive(false);
        optionsB.gameObject.SetActive(true);

        optionsB.onClick.AddListener(() =>
        {
            SoundUI.Instance.PlayUISound(SoundsUI.click);
            optionsB.interactable = false;
            optionsPanel.SetActive(true);
            optionsRect.anchoredPosition = position + new Vector2(1000, 1000);

            if (!Globals.IsMobile)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }

            Globals.IsOptions = true;

            OpenPanel();
        });

        exitB.onClick.AddListener(() =>
        {
            SoundUI.Instance.PlayUISound(SoundsUI.click);
            exitB.interactable = false;
            Globals.IsOptions = false;
            StartCoroutine(playStartLevel());
        });

        continueB.onClick.AddListener(() => 
        {
            SoundUI.Instance.PlayUISound(SoundsUI.click);            
            optionsPanel.SetActive(false);

            optionsB.interactable = true;
            optionsRect.anchoredPosition = position;

            if (!Globals.IsMobile)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Globals.IsOptions = false;
        });

        soundButton.onClick.AddListener(() =>
        {
            if (Globals.IsSoundOn)
            {
                Globals.IsSoundOn = false;
                soundButton.transform.GetChild(0).GetComponent<Image>().sprite = soundOffSprite;
                AudioListener.volume = 0;
            }
            else
            {
                SoundUI.Instance.PlayUISound(SoundsUI.click);
                Globals.IsSoundOn = true;
                soundButton.transform.GetChild(0).GetComponent<Image>().sprite = soundOnSprite;
                AudioListener.volume = 1f;
            }

            SaveLoadManager.Save();
        });

        musicButton.onClick.AddListener(() =>
        {
            if (Globals.IsMusicOn)
            {
                Globals.IsMusicOn = false;
                musicButton.transform.GetChild(0).GetComponent<Image>().sprite = musicOffSprite;
                AmbientMusic.Instance.StopAll();
            }
            else
            {
                SoundUI.Instance.PlayUISound(SoundsUI.click);
                Globals.IsMusicOn = true;
                musicButton.transform.GetChild(0).GetComponent<Image>().sprite = musicOnSprite;
                AmbientMusic.Instance.ContinuePlaying();
            }

            SaveLoadManager.Save();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf)
            {
                SoundUI.Instance.PlayUISound(SoundsUI.click);
                optionsPanel.SetActive(false);

                optionsB.interactable = true;
                optionsRect.anchoredPosition = position;

                if (!Globals.IsMobile)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                Globals.IsOptions = false;
            }
            else
            {
                SoundUI.Instance.PlayUISound(SoundsUI.click);
                optionsB.interactable = false;
                optionsPanel.SetActive(true);
                optionsRect.anchoredPosition = position + new Vector2(1000, 1000);

                if (!Globals.IsMobile)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }

                Globals.IsOptions = true;

                OpenPanel();
            }
        }
    }

    private IEnumerator playStartLevel()
    {
        GameManager.Instance.SaveGame();
        ScreenSaver.Instance.HideScreen();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator playReset()
    {
        Globals.MainPlayerData.CustomGameBlocks = new float[0];
        SaveLoadManager.Save();
        ScreenSaver.Instance.HideScreen();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Gameplay");
    }

    public void OpenPanel()
    {        
        if (Globals.IsSoundOn)
        {
            soundButton.transform.GetChild(0).GetComponent<Image>().sprite = soundOnSprite;
        }
        else
        {
            soundButton.transform.GetChild(0).GetComponent<Image>().sprite = soundOffSprite;
        }

        if (Globals.IsMusicOn)
        {
            musicButton.transform.GetChild(0).GetComponent<Image>().sprite = musicOnSprite;
        }
        else
        {
            musicButton.transform.GetChild(0).GetComponent<Image>().sprite = musicOffSprite;
        }
    }
}
