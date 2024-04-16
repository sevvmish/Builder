using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasScaler CanvasScaler;
    [SerializeField] private TextMeshProUGUI regimeText;
    private int currentRegime = 0;
    private Coroutine lastRegimeTextCoroutine;

    [Header("Main buttons for interface")]
    public GameObject JumpMain;
    public GameObject JumpUP;
    public GameObject JumpDOWN;
    public GameObject Mover;
    public GameObject Joystick;
    

    [Header("Signs")]
    [SerializeField] private TextMeshProUGUI blockText;
    [SerializeField] private TextMeshProUGUI buildText;


    [Header("Main buttons for building mode")]
    [SerializeField] private Button buildCurrentBlockButton;
    [SerializeField] private Button deleteCurrentBlockButton;
    [SerializeField] private Button cancelLastBlockButton;
    [SerializeField] private Button rotateCurrentBlockButton;
    [SerializeField] private Button startDestroingBlockButton;
    [SerializeField] private Button startBuildingBlockButton;
    [SerializeField] private Button callBlocksButton;
    [SerializeField] private Button closeBlocksButton;
    [SerializeField] private Button closeBlocksButtonForVis;
    [SerializeField] private Button BuildingModeButton;
    [SerializeField] private GameObject BuildingModeCloseSign;


    [Header("Options")]
    [SerializeField] private Button optionsButton;

    [Header("BlocksMenu")]
    [SerializeField] private BlockMenuUI blockMenuUI;
    public BlockMenuUI BlockMenuUI { get => blockMenuUI; }

    private SoundUI sounds;
    private GameManager gm;
    private AssetManager assets;
    [SerializeField] private BlockManager blockManager;

    private bool isSetupOnMode;

    private void Awake()
    {
        if (Globals.IsMobile)
        {
            CanvasScaler.matchWidthOrHeight = 0.9f;
        }
        else
        {
            CanvasScaler.matchWidthOrHeight = 0.1f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //startbuildingPrefs();
        blockText.text = Globals.Language.Blocks;
        buildText.text = Globals.Language.Build;

        rotateCurrentBlockButton.gameObject.SetActive(false);

        if (!Globals.IsMobile)
        {
            optionsButton.transform.localScale = Vector3.one * 0.7f;
            callBlocksButton.transform.localScale = Vector3.one * 0.7f;
            BuildingModeButton.transform.localScale = Vector3.one * 0.7f;
        }

        sounds = SoundUI.Instance;
        gm = GameManager.Instance;
        assets = gm.Assets;


        buildCurrentBlockButton.onClick.AddListener(()=> 
        {
            blockManager.BuildCurrentBlockCall();
        });

        deleteCurrentBlockButton.onClick.AddListener(() =>
        {
            blockManager.DeleteCurrentBlock();
        });

        rotateCurrentBlockButton.onClick.AddListener(() => 
        {            
            blockManager.Rotate();          
        });

        startDestroingBlockButton.onClick.AddListener(() => 
        {
            blockManager.StartDestroying();
            cancelLastBlockButton.gameObject.SetActive(false);
            deleteCurrentBlockButton.gameObject.SetActive(true);
            buildCurrentBlockButton.gameObject.SetActive(false);
            startDestroingBlockButton.gameObject.SetActive(false);
            startBuildingBlockButton.gameObject.SetActive(true);
        });

        startBuildingBlockButton.onClick.AddListener(() => 
        {
            startbuildingPrefs();         
            
        });

        callBlocksButton.onClick.AddListener(() =>
        {
            ShowBlocksPanel();
            cancelLastBlockButton.gameObject.SetActive(false);
            buildCurrentBlockButton.gameObject.SetActive(false);
            deleteCurrentBlockButton.gameObject.SetActive(false);
            startBuildingBlockButton.gameObject.SetActive(false);
            startDestroingBlockButton.gameObject.SetActive(false);
        });

        closeBlocksButton.onClick.AddListener(() =>
        {
            sounds.PlayUISound(SoundsUI.click);
            blockMenuUI.HideAllPanel();

            startbuildingPrefs();
        });

        closeBlocksButtonForVis.onClick.AddListener(() =>
        {
            sounds.PlayUISound(SoundsUI.click);
            blockMenuUI.HideAllPanel();

            startbuildingPrefs();
        });

        cancelLastBlockButton.onClick.AddListener(() =>
        {
            sounds.PlayUISound(SoundsUI.click);            
            blockManager.CancelLastBlock();
        });

        BuildingModeButton.onClick.AddListener(() =>
        {            
            gm.SetBuildingMode(!gm.IsBuildMode);
        });

        isSetupOnMode = !gm.IsBuildMode;
    }

    

    private void Update()
    {
        if (BuildingModeButton.interactable && gm.IsWinWalkthroughGame)
        {
            BuildingModeButton.interactable = false;
            buildText.color = new Color(1, 1, 0, 0.5f);
            BuildingModeButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }

        if (!gm.IsBuildMode && currentRegime != 1)
        {
            currentRegime = 1;
            lastRegimeTextCoroutine = StartCoroutine(changeRegimeText(Globals.Language.RegimeSpectator));
        }
        else if (gm.IsBuildMode && (blockManager.IsBuildingBlocks || blockManager.IsChoosingBlocks) && currentRegime != 2)
        {
            currentRegime = 2;
            lastRegimeTextCoroutine = StartCoroutine(changeRegimeText(Globals.Language.RegimeBuilder));
        }
        else if (gm.IsBuildMode && blockManager.IsDestroingBlocks && currentRegime != 3)
        {
            currentRegime = 3;
            lastRegimeTextCoroutine = StartCoroutine(changeRegimeText(Globals.Language.RegimeBuilderDeleter));
        }


        if (gm.IsBuildMode != isSetupOnMode)
        {
            isSetupOnMode = gm.IsBuildMode;
            BuildingModeCloseSign.SetActive(gm.IsBuildMode);

            if (Globals.IsMobile)
            {
                Joystick.SetActive(true);

                if (gm.IsBuildMode)
                {
                    JumpMain.SetActive(false);
                    JumpUP.SetActive(true);
                    JumpDOWN.SetActive(true);
                    startbuildingPrefs();
                }
                else
                {
                    JumpMain.SetActive(true);
                    JumpUP.SetActive(false);
                    JumpDOWN.SetActive(false);
                    startNonbuildingPrefs();
                }                
            }
            else
            {
                if (gm.IsBuildMode)
                {                    
                    startbuildingPrefs();
                }
                else
                {                    
                    startNonbuildingPrefs();
                }

                JumpMain.SetActive(false);
                JumpUP.SetActive(false);
                JumpDOWN.SetActive(false);
                Joystick.SetActive(false);
            }
        }
    }



    private void startbuildingPrefs()
    {
        BuildingModeButton.gameObject.SetActive(true);
        callBlocksButton.gameObject.SetActive(true);

        if (Globals.IsMobile)
        {
            if (gm.IsWalkthroughGame)
            {
                cancelLastBlockButton.gameObject.SetActive(false);
                buildCurrentBlockButton.gameObject.SetActive(true);
                deleteCurrentBlockButton.gameObject.SetActive(false);
                startBuildingBlockButton.gameObject.SetActive(false);
                startDestroingBlockButton.gameObject.SetActive(false);
                rotateCurrentBlockButton.gameObject.SetActive(false);
            }
            else
            {
                cancelLastBlockButton.gameObject.SetActive(true);
                buildCurrentBlockButton.gameObject.SetActive(true);
                deleteCurrentBlockButton.gameObject.SetActive(false);
                startBuildingBlockButton.gameObject.SetActive(false);
                startDestroingBlockButton.gameObject.SetActive(true);
                rotateCurrentBlockButton.gameObject.SetActive(true);
            }            
            
        }
        else
        {
            cancelLastBlockButton.gameObject.SetActive(false);
            buildCurrentBlockButton.gameObject.SetActive(false);
            deleteCurrentBlockButton.gameObject.SetActive(false);
            startBuildingBlockButton.gameObject.SetActive(false);
            startDestroingBlockButton.gameObject.SetActive(false);
            rotateCurrentBlockButton.gameObject.SetActive(false);
        }        
    }

    private void startNonbuildingPrefs()
    {
        cancelLastBlockButton.gameObject.SetActive(false);
        buildCurrentBlockButton.gameObject.SetActive(false);
        deleteCurrentBlockButton.gameObject.SetActive(false);
        startBuildingBlockButton.gameObject.SetActive(false);
        startDestroingBlockButton.gameObject.SetActive(false);
        rotateCurrentBlockButton.gameObject.SetActive(false);
        callBlocksButton.gameObject.SetActive(false);

        BuildingModeButton.gameObject.SetActive(true);
    }

    public void NewBlockChosen()
    {
        if (blockManager.CurrentActiveBlock != null && blockManager.CurrentActiveBlock.IsRotatable)
        {
            if (Globals.IsMobile && !gm.IsWalkthroughGame) rotateCurrentBlockButton.gameObject.SetActive(true);
        }
        else
        {
            rotateCurrentBlockButton.gameObject.SetActive(false);
        }

        blockMenuUI.UpdateAdditionalBlockInfo();
    }

    
    public void PlayerCrossNewBlockError()
    {

    }

    public void HideBlocksPanel()
    {
        blockMenuUI.HideAllPanel();
        startbuildingPrefs();
    }
        
    public void ShowBlocksPanel() => blockMenuUI.ShowBlocksPanel();

    private IEnumerator changeRegimeText(string newText)
    {
        if (lastRegimeTextCoroutine != null) StopCoroutine(lastRegimeTextCoroutine);

        regimeText.gameObject.SetActive(true);
        regimeText.color = new Color(1, 1, 0, 1);
        regimeText.text = newText;
        regimeText.transform.localScale = Vector3.zero;
        regimeText.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.15f);
        regimeText.transform.DOShakeScale(0.2f, 1, 30).SetEase(Ease.InOutBounce);

        yield return new WaitForSeconds(1.5f);
        regimeText.color = new Color(1, 1, 0, 0.66f);

        yield return new WaitForSeconds(1f);
        regimeText.color = new Color(1, 1, 0, 0.33f);

        yield return new WaitForSeconds(0.5f);
        regimeText.gameObject.SetActive(false);
    }
}
