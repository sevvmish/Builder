using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockPanelUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button activateButton;

    private int id;
    private BlockManager blockManager;
    private Block block;

    private GameManager gm;
    private AssetManager assets;


    public void SetData(int id, BlockManager bm)
    {
        gm = GameManager.Instance;
        assets = gm.Assets;

        this.id = id;
        block = assets.GetBlockDataByID(id);

        blockManager = bm;

        iconImage.sprite = block.BlockIcon;
        activateButton.onClick.AddListener(() => 
        {            
            blockManager.OnChangeCurrentBlock?.Invoke(this.id);
        });
    }
        
}
