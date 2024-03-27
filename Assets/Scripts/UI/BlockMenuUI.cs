using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockMenuUI : MonoBehaviour
{    
    [Header("BlocksMenu")]
    [SerializeField] private Button callBlocksButton;
    [SerializeField] private GameObject blocksPanel;
    [SerializeField] private GameObject blocksPanelExample;
    [SerializeField] private Transform blockMenuContainer;

    [SerializeField] private Button floorsFilterButton;
    [SerializeField] private Button wallsFilterButton;

    private BlockTypes defaultStartBlock = BlockTypes.floor;
    
    private List<GameObject> floors = new List<GameObject>();
    private List<GameObject> walls = new List<GameObject>();

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

        blocksPanel.SetActive(false);
        createBlocksPanel();
        callBlocksButton.onClick.AddListener(() =>
        {
            ShowBlocksPanel();
        });

        floorsFilterButton.onClick.AddListener(() =>
        {
            if (defaultStartBlock == BlockTypes.floor) return;

            sounds.PlayUISound(SoundsUI.click);
            defaultStartBlock = BlockTypes.floor;
            filterBlocksPanel();
        });

        wallsFilterButton.onClick.AddListener(() =>
        {
            if (defaultStartBlock == BlockTypes.wall) return;

            sounds.PlayUISound(SoundsUI.click);
            defaultStartBlock = BlockTypes.wall;
            filterBlocksPanel();
        });
    }
    
    private void createBlocksPanel()
    {
        create(assets.GetArrayOfFloorsIds, ref floors);
        create(assets.GetArrayOfWallsIds, ref walls);
        
    }
    private void create(int[] sourceIDs, ref List<GameObject> sourceGameobjects)
    {        
        if (sourceIDs.Length > 0)
        {
            for (int i = 0; i < sourceIDs.Length; i++)
            {
                GameObject g = Instantiate(blocksPanelExample, blockMenuContainer);
                g.GetComponent<BlockPanelUI>().SetData(sourceIDs[i], blockManager);
                sourceGameobjects.Add(g);
            }
        }
    }

    private void hideAll()
    {
        floors.ForEach(p => p.SetActive(false));
        walls.ForEach(p => p.SetActive(false));
    }

    public void ShowBlocksPanel()
    {
        sounds.PlayUISound(SoundsUI.click);

        if (blocksPanel.activeSelf)
        {
            HideAllPanel();
            return;
        }

        callBlocksButton.gameObject.SetActive(false);
        blocksPanel.SetActive(true);

        filterBlocksPanel();
    }
    private void filterBlocksPanel()
    {
        hideAll();

        switch (defaultStartBlock)
        {
            case BlockTypes.floor:
                floors.ForEach(p => p.SetActive(true));
                break;

            case BlockTypes.wall:
                walls.ForEach(p => p.SetActive(true));
                break;
        }
    }

    public void HideAllPanel()
    {
        blocksPanel.SetActive(false);
        callBlocksButton.gameObject.SetActive(true);
    }
        
}
