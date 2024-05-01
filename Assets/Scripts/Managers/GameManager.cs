using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using YG;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Others")]
    [SerializeField] private AssetManager assetManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform cameraBody;    
    [SerializeField] private CameraControl cameraControl;
    [SerializeField] private Transform playersLocation;
    [SerializeField] private UIManager UI;
    [SerializeField] private BlockManager blockManager;
    [SerializeField] private TextMeshProUGUI texter;
    [SerializeField] private LevelControl levelControl;
    [SerializeField] private Interstitial interstitial;
    [SerializeField] private Light mainLight;

    private InputControl playerInput;

    public PlayerControl MainPlayerControl { get; private set; }

    public Camera GetCamera() => _camera;
    public Transform GetCameraBody() => cameraBody;
    public CameraControl GetCameraControl() => cameraControl;
    public Transform GetPlayersLocation() => playersLocation;
    public Transform GetMainPlayerTransform() => mainPlayer;    
    public AssetManager Assets => assetManager;
    public UIManager GetUI => UI;
    public LevelControl LevelControl => levelControl;
    public BlockManager BlockManager => blockManager;
    public float PointerClickedCount;

    //GAME START    
    public bool IsGameStarted { get; private set; }
    public bool IsBuildMode { get; private set; }    
    public bool IsWalkthroughGame { get; private set; }
    public bool IsWinWalkthroughGame { get; private set; }

    private Transform mainPlayer;    
    private float cameraShakeCooldown;
    private bool isWalkthroughtDone;
    private bool isLastLevelEnded;

    public TextMeshProUGUI Texter => texter;
    public Vector3 pointForMarker => playerInput.GetMarkerPosition;
    public Block blockForMarker => playerInput.GetMarkerAim;



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

        if (Globals.MainPlayerData != null) YandexGame.StickyAdActivity(!Globals.MainPlayerData.AdvOff);

        setLevel(Globals.CurrentLevel);

        /*
        //TODEL======================
        Globals.MainPlayerData = new PlayerData();        
        Globals.IsInitiated = true;
        Globals.IsMobile = false;
        Globals.IsSoundOn = true;
        Globals.IsMusicOn = true;
        Globals.Language = Localization.GetInstanse("ru").GetCurrentTranslation();
        Globals.CurrentLevel = Globals.MainPlayerData.Level;
        Globals.IsWalkthroughEnabled = true;
        if (Globals.IsMobile)
        {
            Globals.MainPlayerData.Zoom = 50;
        }
        else
        {
            Globals.MainPlayerData.Zoom = 60;
        }*/
        //===========================

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

        

        mainPlayer = AddPlayer(true, Vector3.zero, Vector3.zero).transform;
        mainPlayer.position = new Vector3(0, 0, 0);
        cameraControl.SetData(mainPlayer, cameraBody, _camera.transform);        
        mainPlayer.gameObject.name = "Main Player";

        playerInput = mainPlayer.GetComponent<InputControl>();
        //IsBuildMode = true;
        IsGameStarted = true;

        IsWalkthroughGame = Globals.IsWalkthroughEnabled;        
    }

    private void Start()
    {
        AmbientMusic.Instance.PlayScenario1();
        ScreenSaver.Instance.ShowScreen();

        if (IsWalkthroughGame)
        {
            levelControl.SetData();
        }
    }
    
    public void WinGameWithVisualization()
    {
        if (!isWalkthroughtDone)
        {
            isWalkthroughtDone = true;
            IsBuildMode = false;
            blockManager.StopBuilding();
            IsWinWalkthroughGame = true;

            if (Globals.CurrentLevel == Globals.MainPlayerData.Level)
            {
                if (Globals.MainPlayerData.Level < (levelControl.MaxLevels -1))
                {
                    Globals.MainPlayerData.Level++;
                    YandexGame.NewLeaderboardScores("lider", Globals.MainPlayerData.Level);
                    YandexMetrica.Send("level" + Globals.MainPlayerData.Level);
                    SaveLoadManager.Save();
                }                
            }

            if (Globals.CurrentLevel < (levelControl.MaxLevels - 1))
            {
                Globals.CurrentLevel++;
                if (Globals.CurrentLevel == 1) isLastLevelEnded = true;
            }
            else
            {
                isLastLevelEnded = true;
            }
            
            UI.GameWin();
        }        
    }

    public void ToNextLevel()
    {
        print("Level: " + Globals.CurrentLevel + " = " + Globals.MainPlayerData.Level);

        if (!Globals.MainPlayerData.AdvOff && (DateTime.Now - Globals.TimeWhenLastInterstitialWas).TotalSeconds >= Globals.INTERSTITIAL_COOLDOWN)
        {
            startInterstitial();
        }
        else
        {
            startLevel();
        }
    }

    private void startInterstitial()
    {
        StartCoroutine(playInterstitial());
    }
    private IEnumerator playInterstitial()
    {
        ScreenSaver.Instance.HideScreen();
        yield return new WaitForSeconds(1);
        interstitial.OnEnded = fastStart;
        interstitial.ShowInterstitialVideo();
    }

    private void fastStart()
    {
        if (isLastLevelEnded)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("Gameplay");
        }        
    }

    private void startLevel()
    {
        StartCoroutine(playStartLevel());
    }
    private IEnumerator playStartLevel()
    {
        ScreenSaver.Instance.HideScreen();
        yield return new WaitForSeconds(1);
        
        if (isLastLevelEnded)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("Gameplay");
        }
    }

    public void SetBuildingMode(bool isActive)
    {
        if (IsWinWalkthroughGame && isActive) return;

        IsBuildMode = isActive;
        SoundUI.Instance.PlayUISound(SoundsUI.success2);

        if (isActive)
        {
            if (IsWalkthroughGame) levelControl.SetVisible(true);
            blockManager.StartBuilding();
        }
        else
        {
            if (IsWalkthroughGame) levelControl.SetVisible(false);
            blockManager.StopBuilding();
        }
    }

        
    public void ShakeScreen(float _time, float strength, int vibra)
    {
        if (cameraShakeCooldown > 0) return;

        _time = _time < 0.1f ? 0.1f : _time;
        strength = strength < 1f ? 1f : strength;
        vibra = vibra < 10 ? 10 : vibra;


        _camera.transform.DOShakePosition(_time, strength, vibra);
        cameraShakeCooldown = _time + 0.1f;
    }
  

    private void Update()
    {
        if (PointerClickedCount > 0) PointerClickedCount -= Time.deltaTime;

        if (cameraShakeCooldown > 0) cameraShakeCooldown -= Time.deltaTime;
              
        if (Input.GetKeyDown(KeyCode.P))
        {
            Globals.MainPlayerData = new PlayerData();
            SaveLoadManager.Save();
            SceneManager.LoadScene("MainMenu");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            WinGameWithVisualization();
        }
    }



    public GameObject AddPlayer(bool isMain, Vector3 pos, Vector3 rot)
    {
        //main template
        GameObject g = Instantiate(Resources.Load<GameObject>("main player"));
        g.transform.parent = playersLocation;
        g.transform.position = pos;
        g.transform.eulerAngles = rot;
        g.AddComponent<PlayerControl>();

        //vfx
        GameObject vfx = Instantiate(Resources.Load<GameObject>("player vfx"), g.transform);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localEulerAngles = Vector3.zero;
        g.GetComponent<PlayerControl>().SetEffectControl(vfx.GetComponent<EffectsControl>());

        //player
        GameObject skin = Instantiate(Resources.Load<GameObject>("skin2"));
        skin.transform.parent = g.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;

        
        g.GetComponent<PlayerControl>().SetSkinData(skin.GetComponent<Animator>());

        g.AddComponent<InputControl>();
        g.AddComponent<AudioListener>();
        MainPlayerControl = g.GetComponent<PlayerControl>();

        g.SetActive(true);
        
        return g;
    }

    private void setLevel(int level)
    {
        if (level == 8)
        {
            mainLight.intensity = 0.8f;
            assetManager.SetLevel(Maps.river);
        }
        else if (level == 0)
        {
            mainLight.intensity = 1.15f;
            assetManager.SetLevel(Maps.village);
        }
        else if (level <= 10)
        {
            mainLight.intensity = 1f;
            assetManager.SetLevel(Maps.forest);
        }
        else
        {
            mainLight.intensity = 1.15f;
            assetManager.SetLevel(Maps.village);
        }
    }
}

public enum Maps
{
    river,
    village,
    forest
}
