using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main buttons")]
    [SerializeField] private Button buildCurrentBlockButton;
    [SerializeField] private Button deleteCurrentBlockButton;
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

    

    // Start is called before the first frame update
    void Start()
    {
        startbuildingPrefs();

        sounds = SoundUI.Instance;
        gm = GameManager.Instance;
        assets = gm.Assets;

        rotateCurrentBlockButton.gameObject.SetActive(false);
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
    }

    private void startbuildingPrefs()
    {
        blockManager.StartBuilding();
        buildCurrentBlockButton.gameObject.SetActive(true);
        deleteCurrentBlockButton.gameObject.SetActive(false);
        startBuildingBlockButton.gameObject.SetActive(false);
        startDestroingBlockButton.gameObject.SetActive(true);
    }
    
    public void NewBlockChosen()
    {
        if (blockManager.CurrentActiveBlock != null && blockManager.CurrentActiveBlock.IsRotatable)
        {
            rotateCurrentBlockButton.gameObject.SetActive(true);
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
