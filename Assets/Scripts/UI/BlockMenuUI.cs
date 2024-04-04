using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockMenuUI : MonoBehaviour
{    
    [Header("BlocksMenu")]
    //[SerializeField] private Button callBlocksButton;
    //[SerializeField] private Button closeBlocksButton;
    [SerializeField] private GameObject blocksPanel;
    [SerializeField] private GameObject blocksPanelExample;
    [SerializeField] private Transform blockMenuContainer;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private PointerDownOnly backCkick;

    [Header("Buttons")]
    [SerializeField] private Button floorsFilterButton;
    [SerializeField] private Button wallsFilterButton;
    [SerializeField] private Button roofsFilterButton;
    [SerializeField] private Button stairsFilterButton;
    [SerializeField] private Button beamsFilterButton;
    [SerializeField] private Button fencesFilterButton;

    [Header("FOR PC")]
    [SerializeField] private int leftPC = 30;
    [SerializeField] private int topPC = 50;
    [SerializeField] private Vector2 cellSizePC = new Vector2(160, 230);
    [SerializeField] private Vector2 spacingPC = new Vector2(50, 50);

    [Header("FOR Mob")]
    [SerializeField] private int leftM = 60;
    [SerializeField] private int topM = 70;
    [SerializeField] private Vector2 cellSizeM = new Vector2(192, 276);
    [SerializeField] private Vector2 spacingM = new Vector2(80, 120);


    private BlockTypes defaultStartBlock = BlockTypes.floor;
    
    private List<GameObject> floors = new List<GameObject>();
    private List<GameObject> walls = new List<GameObject>();
    private List<GameObject> roofs = new List<GameObject>();
    private List<GameObject> beams = new List<GameObject>();
    private List<GameObject> fences = new List<GameObject>();
    private List<GameObject> stairs = new List<GameObject>();

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

        if (Globals.IsMobile)
        {
            gridLayoutGroup.padding.left = leftM;
            gridLayoutGroup.padding.top = topM;

            gridLayoutGroup.cellSize = cellSizeM;
            gridLayoutGroup.spacing = spacingM;

            blocksPanel.transform.localScale = Vector3.one * 0.8f;
        }
        else
        {
            gridLayoutGroup.padding.left = leftPC;
            gridLayoutGroup.padding.top = topPC;

            gridLayoutGroup.cellSize = cellSizePC;
            gridLayoutGroup.spacing = spacingPC;

            blocksPanel.transform.localScale = Vector3.one * 0.9f;
        }
        

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

        roofsFilterButton.onClick.AddListener(() =>
        {
            if (defaultStartBlock == BlockTypes.roof) return;

            sounds.PlayUISound(SoundsUI.click);
            defaultStartBlock = BlockTypes.roof;
            filterBlocksPanel();
        });

        stairsFilterButton.onClick.AddListener(() =>
        {
            if (defaultStartBlock == BlockTypes.stair) return;

            sounds.PlayUISound(SoundsUI.click);
            defaultStartBlock = BlockTypes.stair;
            filterBlocksPanel();
        });

        beamsFilterButton.onClick.AddListener(() =>
        {
            if (defaultStartBlock == BlockTypes.beam) return;

            sounds.PlayUISound(SoundsUI.click);
            defaultStartBlock = BlockTypes.beam;
            filterBlocksPanel();
        });

        fencesFilterButton.onClick.AddListener(() =>
        {
            if (defaultStartBlock == BlockTypes.fence) return;

            sounds.PlayUISound(SoundsUI.click);
            defaultStartBlock = BlockTypes.fence;
            filterBlocksPanel();
        });

    }

    private void Update()
    {
        if (backCkick.IsPressed)
        {
            gm.GetUI.HideBlocksPanel();
        }
    }

    private void createBlocksPanel()
    {
        create(assets.GetArrayOfFloorsIds, ref floors);
        create(assets.GetArrayOfWallsIds, ref walls);
        create(assets.GetArrayOfRoofsIds, ref roofs);
        create(assets.GetArrayOfBeamsIds, ref beams);
        create(assets.GetArrayOfFencesIds, ref fences);
        create(assets.GetArrayOfStairsIds, ref stairs);

    }
    private void create(int[] sourceIDs, ref List<GameObject> sourceGameobjects)
    {        
        if (sourceIDs.Length > 0)
        {
            for (int i = 0; i < sourceIDs.Length; i++)
            {
                GameObject g = Instantiate(blocksPanelExample, blockMenuContainer);

                if (Globals.IsMobile)
                {
                    g.transform.localScale = Vector3.one * 1.4f;
                }

                g.GetComponent<BlockPanelUI>().SetData(sourceIDs[i], blockManager);
                sourceGameobjects.Add(g);
            }
        }
    }

    private void hideAll()
    {
        floors.ForEach(p => p.SetActive(false));
        walls.ForEach(p => p.SetActive(false));
        roofs.ForEach(p => p.SetActive(false));
        stairs.ForEach(p => p.SetActive(false));
        beams.ForEach(p => p.SetActive(false));
        fences.ForEach(p => p.SetActive(false));
    }

    public void ShowBlocksPanel()
    {
        sounds.PlayUISound(SoundsUI.click);

        if (blocksPanel.activeSelf)
        {
            HideAllPanel();
            return;
        }

        if (!Globals.IsMobile)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        

        blockManager.StartChoosing();
        
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

            case BlockTypes.roof:
                roofs.ForEach(p => p.SetActive(true));
                break;

            case BlockTypes.stair:
                stairs.ForEach(p => p.SetActive(true));
                break;

            case BlockTypes.beam:
                beams.ForEach(p => p.SetActive(true));
                break;

            case BlockTypes.fence:
                fences.ForEach(p => p.SetActive(true));
                break;
        }
    }

    public void HideAllPanel()
    {
        blocksPanel.SetActive(false);
        blockManager.StartBuilding();
        

        if (!Globals.IsMobile)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }            
    }
        
}
