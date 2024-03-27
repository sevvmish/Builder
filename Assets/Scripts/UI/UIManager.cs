using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main buttons")]
    [SerializeField] private Button buildCurrentBlockButton;
    [SerializeField] private Button rotateCurrentBlockButton;
    [SerializeField] private Button startDestroingBlockButton;
    [SerializeField] private Button startBuildingBlockButton;

    [Header("BlocksMenu")]
    [SerializeField] private BlockMenuUI blockMenuUI;
    
    private SoundUI sounds;
    private GameManager gm;
    private AssetManager assets;
    [SerializeField] private BlockManager blockManager;

    

    // Start is called before the first frame update
    void Start()
    {
        sounds = SoundUI.Instance;
        gm = GameManager.Instance;
        assets = gm.Assets;

        rotateCurrentBlockButton.gameObject.SetActive(false);
        buildCurrentBlockButton.onClick.AddListener(BuildCurrentBlock);

        rotateCurrentBlockButton.onClick.AddListener(() => 
        {
            sounds.PlayUISound(SoundsUI.click);

            if (blockManager.CurrentActiveBlock != null && blockManager.CurrentActiveBlock.IsRotatable)
            {
                blockManager.CurrentActiveBlock.Rotate();
            }            
        });

        startDestroingBlockButton.onClick.AddListener(() => { blockManager.StartDestroying(); });
        startBuildingBlockButton.onClick.AddListener(() => { blockManager.StartBuilding(); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            BuildCurrentBlock();
        }
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

    public void BuildCurrentBlock()
    {
        sounds.PlayUISound(SoundsUI.click);
        blockManager.OnBuildCurrentBlock?.Invoke();
    }

    public void PlayerCrossNewBlockError()
    {

    }

    public void HideBlocksPanel() => blockMenuUI.HideAllPanel();
    public void ShowBlocksPanel() => blockMenuUI.ShowBlocksPanel();
}
