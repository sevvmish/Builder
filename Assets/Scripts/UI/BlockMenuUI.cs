using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockMenuUI : MonoBehaviour
{
    public bool IsPanelOpened => blocksPanel.activeSelf;
    
    [Header("BlocksMenu for visualization")]
    [SerializeField] private GameObject blocksPanelForVis;
    [SerializeField] private Transform blockMenuContainerForVis;
    [SerializeField] private GridLayoutGroup gridLayoutGroupForVis;
    [SerializeField] private PointerDownOnly backCkickForVis;
    [SerializeField] private TextMeshProUGUI missionName;
    [SerializeField] private TextMeshProUGUI missionStage;
    [SerializeField] private Slider missionProgress;
    [SerializeField] private Image missionIcon;

    private Stage currentStage;
    private Dictionary<int, BlockPanelUI> panelsForVis = new Dictionary<int, BlockPanelUI>();

    [Header("Block Additional INFO")]
    [SerializeField] private GameObject blockAdditionalInfo;
    [SerializeField] private TextMeshProUGUI amountLeftText;
    [SerializeField] private Slider progressLeft;
    [SerializeField] private Image blockIcon;
    private Block currentBlock;
    private Vector2 blockAdditionalInfoPosition;


    [Header("BlocksMenu")]    
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
    private LevelControl lc;
    private BlockManager bm;
    private AssetManager assets;

    [SerializeField] private BlockManager blockManager;


    private void Awake()
    {
        sounds = SoundUI.Instance;
        gm = GameManager.Instance;
        lc = gm.LevelControl;
        bm = gm.BlockManager;
        assets = gm.Assets;
    }

    // Start is called before the first frame update
    void Start()
    {        
        blocksPanel.SetActive(false);
        blocksPanelForVis.SetActive(false);

        if (gm.IsWalkthroughGame)
        {
            missionName.text = getMissionName(Globals.CurrentLevel);
            missionIcon.sprite = lc.GetLevelData.MissionIcon;
        }
        blockAdditionalInfo.SetActive(false);
        blockAdditionalInfoPosition = blockAdditionalInfo.GetComponent<RectTransform>().anchoredPosition;

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

            gridLayoutGroupForVis.padding.left = leftM;
            gridLayoutGroupForVis.padding.top = topM;

            gridLayoutGroupForVis.cellSize = cellSizeM;
            gridLayoutGroupForVis.spacing = spacingM;

            blocksPanelForVis.transform.localScale = Vector3.one * 0.8f;
        }
        else
        {
            gridLayoutGroup.padding.left = leftPC;
            gridLayoutGroup.padding.top = topPC;

            gridLayoutGroup.cellSize = cellSizePC;
            gridLayoutGroup.spacing = spacingPC;

            blocksPanel.transform.localScale = Vector3.one * 0.9f;

            gridLayoutGroupForVis.padding.left = leftPC;
            gridLayoutGroupForVis.padding.top = topPC;

            gridLayoutGroupForVis.cellSize = cellSizePC;
            gridLayoutGroupForVis.spacing = spacingPC;

            blocksPanelForVis.transform.localScale = Vector3.one * 0.9f;
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
        if (!gm.IsBuildMode && blockAdditionalInfo.activeSelf)
        {
            blockAdditionalInfo.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Tab) && gm.PointerClickedCount <= 0 && !IsPanelOpened)
        {
            if (gm.IsBuildMode)
            {
                if (gm.IsWalkthroughGame)
                {
                    ShowBlocksPanel();
                    gm.PointerClickedCount = 0.1f;
                }
                else
                {
                    ShowBlocksPanel();
                    gm.PointerClickedCount = 0.1f;
                }

                
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


        if (backCkick.IsPressed || backCkickForVis.IsPressed)
        {
            gm.GetUI.HideBlocksPanel();
        }
    }

    public void UpdateIconsForVis(Stage stage)
    {        
        if (currentStage != null && currentStage.Equals(stage))
        {
            foreach (int b in panelsForVis.Keys)
            {
                panelsForVis[b].Amount = 0;
            }

            for (int i = 0; i < currentStage.Blocks.Count; i++)
            {
                if (!currentStage.Blocks[i].IsFinalized)
                {
                    panelsForVis[currentStage.Blocks[i].ID.ID].Amount++;
                }                
            }
            
            return;
        }

        if (currentStage != null && !currentStage.Equals(stage))
        {
            foreach (int key in panelsForVis.Keys)
            {
                Destroy(panelsForVis[key].gameObject);
            }
        }

        currentStage = stage;
        panelsForVis.Clear();

        for (int i = 0; i < currentStage.Blocks.Count; i++)
        {
            if (panelsForVis.ContainsKey(currentStage.Blocks[i].ID.ID))
            {
                panelsForVis[currentStage.Blocks[i].ID.ID].Amount++;
            }
            else
            {
                GameObject g = Instantiate(blocksPanelExample, blockMenuContainerForVis);

                if (Globals.IsMobile)
                {
                    g.transform.localScale = Vector3.one * 1.4f;
                }

                int id = currentStage.Blocks[i].GetComponent<Identificator>().ID;
                g.GetComponent<BlockPanelUI>().SetData(id, blockManager);
                panelsForVis.Add(id, g.GetComponent<BlockPanelUI>());
            }
        }

        
    }

    public void UpdateAdditionalBlockInfo()
    {        
        if (!gm.IsBuildMode) return;
                
        Block block = bm.CurrentActiveBlock;
        if (block == null)
        {
            blockAdditionalInfo.SetActive(false);
            return;
        }
        else
        {
            blockAdditionalInfo.SetActive(true);
        }

        if (currentBlock != null && currentBlock.ID.ID != block.ID.ID)
        {
            currentBlock = block;
            StartCoroutine(playChangeAdditionalInfo(blockAdditionalInfo.transform, block));
            return;
        }

        currentBlock = block;

        blockIcon.sprite = block.BlockIcon;

        int overall = 0;
        int left = 0;
        int done = 0;

        Stage s = lc.CurrentStage();

        for (int i = 0; i < s.Blocks.Count; i++)
        {
            if (s.Blocks[i].ID.ID == block.ID.ID)
            {
                if (s.Blocks[i].IsFinalized)
                {
                    done++;
                }
                else
                {                    
                    left++;
                }

                overall++;
            }
        }

        amountLeftText.text = "x" + left;
        progressLeft.value = (float)done / overall;
    }
    private IEnumerator playChangeAdditionalInfo(Transform t, Block b)
    {        
        RectTransform r = t.GetComponent<RectTransform>();
        r.DOAnchorPos(blockAdditionalInfoPosition + new Vector2(1000, 0), 0.1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.1f);

        blockIcon.sprite = b.BlockIcon;

        int overall = 0;
        int left = 0;
        int done = 0;

        Stage s = lc.CurrentStage();

        for (int i = 0; i < s.Blocks.Count; i++)
        {
            if (s.Blocks[i].ID.ID == b.ID.ID)
            {
                if (s.Blocks[i].IsFinalized)
                {
                    done++;
                }
                else
                {
                    left++;
                }

                overall++;
            }
        }

        amountLeftText.text = "x" + left;
        progressLeft.value = (float)done / overall;

        r.DOAnchorPos(blockAdditionalInfoPosition, 0.1f).SetEase(Ease.InOutQuad);
    }

    private void updateMissionInfo()
    {   
        missionStage.text = $"{Globals.Language.Stage}: {lc.CurrentStageNumber()} {Globals.Language.StageFrom} {lc.StagesAmount}";
        missionProgress.value = (float)lc.CurrentStageNumber() / lc.StagesAmount;
    }

    private void createBlocksPanel()
    {
        if (gm.IsWalkthroughGame)
        {
            //
        }
        else
        {
            create(assets.GetArrayOfFloorsIds, ref floors);
            create(assets.GetArrayOfWallsIds, ref walls);
            create(assets.GetArrayOfRoofsIds, ref roofs);
            create(assets.GetArrayOfPartsIds, ref parts);
            create(assets.GetArrayOfOthersIds, ref others);
        }
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

        if (gm.IsWalkthroughGame)
        {
            if (blocksPanelForVis.activeSelf)
            {
                HideAllPanel();
                return;
            }

            //blocksPanelForVis.SetActive(true);
            StartCoroutine(playShow(blocksPanelForVis.transform));
            updateMissionInfo();            
        }
        else
        {
            if (blocksPanel.activeSelf)
            {
                HideAllPanel();
                return;
            }

            blocksPanel.SetActive(true);
            StartCoroutine(playShow(blocksPanel.transform));
            filterBlocksPanel();
        }

        if (!Globals.IsMobile)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        blockManager.StartChoosing();
    }
    private IEnumerator playShow(Transform t)
    {
        t.gameObject.SetActive(true);

        RectTransform r = t.GetComponent<RectTransform>();
        r.anchoredPosition = new Vector2 (-1200, 0);
        r.DOAnchorPos(new Vector2(0, 0), 0.25f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.25f);
        
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
        if (gm.IsWalkthroughGame)
        {
            //blocksPanelForVis.SetActive(false);
            StartCoroutine(playHide(blocksPanelForVis.transform));
            blockManager.StartBuilding();
        }
        else
        {
            //blocksPanel.SetActive(false);
            StartCoroutine(playHide(blocksPanel.transform));
            blockManager.StartBuilding();
        }
        
        

        if (!Globals.IsMobile)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }            
    }
    private IEnumerator playHide(Transform t)
    {
        RectTransform r = t.GetComponent<RectTransform>();
        r.DOAnchorPos(new Vector2(-1200, 0), 0.25f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.25f);
        t.gameObject.SetActive(false);
    }

    private void resetIcons()
    {
        if (gm.IsWalkthroughGame) return;

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

    private static string getMissionName(int level)
    {
        switch (level)
        {
            case 0:
                return Globals.Language.MissionName0;

            case 1:
                return Globals.Language.MissionName1;

            case 2:
                return Globals.Language.MissionName2;
        }

        return "";
    }
        
}
