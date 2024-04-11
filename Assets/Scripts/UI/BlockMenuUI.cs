using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockMenuUI : MonoBehaviour
{
    public bool IsPanelOpened => blocksPanel.activeSelf;


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
    [SerializeField] private Button partsFilterButton;
    [SerializeField] private Button othersFilterButton;

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

    [Header("Icons")]
    [SerializeField] private Image floorsImage;
    [SerializeField] private Image wallsImage;
    [SerializeField] private Image roofsImage;
    [SerializeField] private Image partsImage;
    [SerializeField] private Image othersImage;
    private int currentIndex = 1;
    private const int MAX_INDEX = 5;

    private BlockTypes defaultStartBlock = BlockTypes.floor;
    
    private List<GameObject> floors = new List<GameObject>();
    private List<GameObject> walls = new List<GameObject>();
    private List<GameObject> roofs = new List<GameObject>();
    private List<GameObject> parts = new List<GameObject>();
    private List<GameObject> others = new List<GameObject>();

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
        resetIcons();
        floorsImage.color = Color.yellow;
        floorsImage.transform.localScale = Vector3.one;
        currentIndex = 1;

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
            activateFloors();
        });

        wallsFilterButton.onClick.AddListener(() =>
        {
            activateWalls();
        });

        roofsFilterButton.onClick.AddListener(() =>
        {
            activateRoofs();
        });

        partsFilterButton.onClick.AddListener(() =>
        {
            activateParts();
        });

        othersFilterButton.onClick.AddListener(() =>
        {
            activateOthers();
        });
    }

    private void activateFloors()
    {
        if (defaultStartBlock == BlockTypes.floor) return;

        resetIcons();
        floorsImage.color = Color.yellow;
        floorsImage.transform.localScale = Vector3.one;

        currentIndex = 1;

        sounds.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.floor;
        filterBlocksPanel();
    }

    private void activateWalls()
    {
        if (defaultStartBlock == BlockTypes.wall) return;

        resetIcons();
        wallsImage.color = Color.yellow;
        wallsImage.transform.localScale = Vector3.one;

        currentIndex = 2;

        sounds.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.wall;
        filterBlocksPanel();
    }

    private void activateRoofs()
    {
        if (defaultStartBlock == BlockTypes.roof) return;

        resetIcons();
        roofsImage.color = Color.yellow;
        roofsImage.transform.localScale = Vector3.one;

        currentIndex = 3;

        sounds.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.roof;
        filterBlocksPanel();
    }

    private void activateParts()
    {
        if (defaultStartBlock == BlockTypes.parts) return;

        resetIcons();
        partsImage.color = Color.yellow;
        partsImage.transform.localScale = Vector3.one;

        currentIndex = 4;

        sounds.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.parts;
        filterBlocksPanel();
    }

    private void activateOthers()
    {
        if (defaultStartBlock == BlockTypes.others) return;

        resetIcons();
        othersImage.color = Color.yellow;
        othersImage.transform.localScale = Vector3.one;

        currentIndex = 5;

        sounds.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.others;
        filterBlocksPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && gm.PointerClickedCount <= 0 && !IsPanelOpened)
        {
            if (gm.IsBuildMode)
            {
                ShowBlocksPanel();
                gm.PointerClickedCount = 0.1f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && blocksPanel.activeSelf)
        {
            HideAllPanel();
        }

        if (Input.GetKeyDown(KeyCode.D) && blocksPanel.activeSelf)
        {
            currentIndex++;

            if (currentIndex > MAX_INDEX) currentIndex = 1;

            switch (currentIndex)
            {
                case 1:
                    activateFloors();
                    break;

                case 2:
                    activateWalls();
                    break;

                case 3:
                    activateRoofs();
                    break;

                case 4:
                    activateParts();
                    break;

                case 5:
                    activateOthers();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.A) && blocksPanel.activeSelf)
        {
            currentIndex--;

            if (currentIndex < 1) currentIndex = MAX_INDEX;

            switch (currentIndex)
            {
                case 1:
                    activateFloors();
                    break;

                case 2:
                    activateWalls();
                    break;

                case 3:
                    activateRoofs();
                    break;

                case 4:
                    activateParts();
                    break;

                case 5:
                    activateOthers();
                    break;
            }
        }


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
        create(assets.GetArrayOfPartsIds, ref parts);
        create(assets.GetArrayOfOthersIds, ref others);

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
        parts.ForEach(p => p.SetActive(false));
        others.ForEach(p => p.SetActive(false));
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

            case BlockTypes.parts:
                parts.ForEach(p => p.SetActive(true));
                break;

            case BlockTypes.others:
                others.ForEach(p => p.SetActive(true));
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

    private void resetIcons()
    {
        floorsImage.color = new Color(1, 1, 1, 0.8f);
        floorsImage.transform.localScale = Vector3.one * 0.75f;

        wallsImage.color = new Color(1, 1, 1, 0.8f);
        wallsImage.transform.localScale = Vector3.one * 0.75f;

        roofsImage.color = new Color(1, 1, 1, 0.8f);
        roofsImage.transform.localScale = Vector3.one * 0.75f;

        partsImage.color = new Color(1, 1, 1, 0.8f);
        partsImage.transform.localScale = Vector3.one * 0.75f;

        othersImage.color = new Color(1, 1, 1, 0.8f);
        othersImage.transform.localScale = Vector3.one * 0.75f;
    }
        
}
