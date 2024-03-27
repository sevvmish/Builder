using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using YG;
using DG.Tweening;

[DefaultExecutionOrder(-2)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Controls")]
    [SerializeField] private Joystick joystick;

    [Header("Others")]
    [SerializeField] private AssetManager assetManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform cameraBody;    
    [SerializeField] private CameraControl cameraControl;
    [SerializeField] private Transform playersLocation;
    [SerializeField] private UIManager UI;
    [SerializeField] private BlockManager blockManager;
    [SerializeField] private TextMeshProUGUI texter;
    private InputControl playerInput;

    public PlayerControl MainPlayerControl { get; private set; }

    public Joystick GetJoystick() => joystick;
    public Camera GetCamera() => _camera;
    public Transform GetCameraBody() => cameraBody;
    public CameraControl GetCameraControl() => cameraControl;
    public Transform GetPlayersLocation() => playersLocation;
    public Transform GetMainPlayerTransform() => mainPlayer;    
    public AssetManager Assets => assetManager;
    public UIManager GetUI => UI;
    public BlockManager BlockManager => blockManager;
    public float PointerClickedCount;

    //GAME START    
    public bool IsGameStarted { get; private set; }
    public bool IsBuildMode { get; private set; }

    private Transform mainPlayer;    
    private float cameraShakeCooldown;
   
    public TextMeshProUGUI Texter => texter;
    public Vector3 pointForMarker => playerInput.GetMarker;
    


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

        if (Globals.MainPlayerData != null) YandexGame.StickyAdActivity(true);
                
        //TODEL
        Globals.MainPlayerData = new PlayerData();
        Globals.MainPlayerData.Zoom = 0;
        Globals.IsInitiated = true;
        Globals.IsMobile = false;
        Globals.IsSoundOn = true;
        Globals.IsMusicOn = true;
        Globals.Language = Localization.GetInstanse(Globals.CurrentLanguage).GetCurrentTranslation();
        Globals.MainRandom = new System.Random(Globals.MainPlayerData.Seed);

        if (Globals.IsMobile)
        {
            QualitySettings.antiAliasing = 2;
            QualitySettings.shadows = ShadowQuality.Disable;//ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
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

    }

    

    private void Start()
    {
        IsGameStarted = true;
        IsBuildMode = true;
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
              

        /*
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddPlayerFinished(MainPlayerControl);
        }*/        
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
        GameObject skin = Instantiate(Resources.Load<GameObject>("skin1"));
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

    
}
