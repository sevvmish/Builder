using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockPanelUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button activateButton;
    [SerializeField] private Image[] sizes;
    [SerializeField] private Sprite full;
    [SerializeField] private Sprite empty;

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

        //SIZE
        sizes[0].sprite = empty;
        sizes[1].sprite = empty;
        sizes[2].sprite = empty;
        switch(block.BlockSize)
        {
            case BlockSizes.small:
                sizes[0].sprite = full;
                break;

            case BlockSizes.medium:
                sizes[0].sprite = full;
                sizes[1].sprite = full;
                break;

            case BlockSizes.large:
                sizes[0].sprite = full;
                sizes[1].sprite = full;
                sizes[2].sprite = full;
                break;
        }


        blockManager = bm;

        iconImage.sprite = block.BlockIcon;
        activateButton.onClick.AddListener(() => 
        {

            if (bm.CurrentActiveBlock != null && bm.CurrentActiveBlock.gameObject.GetComponent<Identificator>().ID == this.id) 
            {
                gm.GetUI.HideBlocksPanel();
                return;
            }

            blockManager.OnChangeCurrentBlock?.Invoke(this.id);
        });

        
    }
        
}
