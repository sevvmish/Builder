using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main buttons for interface")]
    public GameObject JumpMain;
    public GameObject JumpUP;
    public GameObject JumpDOWN;
    public GameObject Mover;
    

    [Header("Main buttons for building mode")]
    [SerializeField] private Button buildCurrentBlockButton;
    [SerializeField] private Button deleteCurrentBlockButton;
    [SerializeField] private Button cancelLastBlockButton;
    [SerializeField] private Button rotateCurrentBlockButton;
    [SerializeField] private Button startDestroingBlockButton;
    [SerializeField] private Button startBuildingBlockButton;
    [SerializeField] private Button callBlocksButton;
    [SerializeField] private Button closeBlocksButton;

    [Header("BlocksMenu")]
    [SerializeField] private BlockMenuUI blockMenuUI;
    
    private SoundUI sounds;
    private GameManager gm;
    private AssetManager assets;
    [SerializeField] private BlockManager blockManager;

    private bool isSetupOnMode;

    // Start is called before the first frame update
    void Start()
    {
        startbuildingPrefs();

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

        cancelLastBlockButton.onClick.AddListener(() =>
        {
            sounds.PlayUISound(SoundsUI.click);            
            blockManager.CancelLastBlock();
        });

        isSetupOnMode = !gm.IsBuildMode;
    }

    private void Update()
    {
        if (gm.IsBuildMode != isSetupOnMode)
        {
            isSetupOnMode = gm.IsBuildMode;

            if (Globals.IsMobile)
            {
                if (gm.IsBuildMode)
                {
                    JumpMain.SetActive(false);
                    JumpUP.SetActive(true);
                    JumpDOWN.SetActive(true);

                }
                else
                {
                    JumpMain.SetActive(true);
                    JumpUP.SetActive(false);
                    JumpDOWN.SetActive(false);
                }
            }
            else
            {
                JumpMain.SetActive(false);
                JumpUP.SetActive(false);
                JumpDOWN.SetActive(false);
            }
            
            
        }
    }

    private void startbuildingPrefs()
    {
        blockManager.StartBuilding();

        if (Globals.IsMobile)
        {
            cancelLastBlockButton.gameObject.SetActive(true);
            buildCurrentBlockButton.gameObject.SetActive(true);
            deleteCurrentBlockButton.gameObject.SetActive(false);
            startBuildingBlockButton.gameObject.SetActive(false);
            startDestroingBlockButton.gameObject.SetActive(true);
            rotateCurrentBlockButton.gameObject.SetActive(true);
        }
        else
        {
            cancelLastBlockButton.gameObject.SetActive(false);
            buildCurrentBlockButton.gameObject.SetActive(false);
            deleteCurrentBlockButton.gameObject.SetActive(false);
            startBuildingBlockButton.gameObject.SetActive(false);
            startDestroingBlockButton.gameObject.SetActive(false);
            callBlocksButton.gameObject.SetActive(false);
            rotateCurrentBlockButton.gameObject.SetActive(false);
        }        
    }
    
    public void NewBlockChosen()
    {
        if (blockManager.CurrentActiveBlock != null && blockManager.CurrentActiveBlock.IsRotatable)
        {
            if (Globals.IsMobile) rotateCurrentBlockButton.gameObject.SetActive(true);
        }
        else
        {
            rotateCurrentBlockButton.gameObject.SetActive(false);
        }
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
}
