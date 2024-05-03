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
    [SerializeField] private TextMeshProUGUI stageInformer;


    private Stage currentStage;
    private Dictionary<int, BlockPanelUI> panelsForVis = new Dictionary<int, BlockPanelUI>();

    [Header("Block Additional INFO")]
    [SerializeField] private GameObject blockAdditionalInfo;
    [SerializeField] private TextMeshProUGUI amountLeftText;
    [SerializeField] private Slider progressLeft;
    [SerializeField] private Image blockIcon;
    private Block currentBlock;
    private Vector2 blockAdditionalInfoPosition;
    private bool isFirstStageInform;


    [Header("BlocksMenu")]    
    [SerializeField] private GameObject blocksPanel;
    [SerializeField] private GameObject blocksPanelExample;
    [SerializeField] private Transform blockMenuContainer;
    [SerializeField] private RectTransform rectContainer;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private PointerDownOnly backCkick;

    private float YcoordInFloorsNonVis;
    private float YcoordInWallsNonVis;
    private float YcoordInRoofsNonVis;
    private float YcoordInPartsNonVis;
    private float YcoordInOthersNonVis;

    [Header("Buttons")]
    [SerializeField] private Button floorsFilterButton;
    [SerializeField] private Button wallsFilterButton;
    [SerializeField] private Button roofsFilterButton;
    [SerializeField] private Button partsFilterButton;
    [SerializeField] private Button othersFilterButton;

    [Header("FOR PC")]
    private int leftPC = 50;
    private int topPC = 50;
    private int botomPC = 50;
    private Vector2 cellSizePC = new Vector2(160, 230);
    private Vector2 cellSizePCver2 = new Vector2(150, 220);
    private Vector2 spacingPC = new Vector2(55, 60);

    //[Header("FOR Mob")]
    private int leftM = 70;
    private int topM = 70;
    private int botomM = 70;
    private Vector2 cellSizeM = new Vector2(190, 270);
    private Vector2 cellSizeMver2 = new Vector2(175, 250);
    private Vector2 spacingM = new Vector2(100, 120);

    [Header("Icons")]
    [SerializeField] private Image floorsImage;
    [SerializeField] private Image wallsImage;
    [SerializeField] private Image roofsImage;
    [SerializeField] private Image partsImage;
    [SerializeField] private Image othersImage;
    [SerializeField] private GameObject floorsImageOutline;
    [SerializeField] private GameObject wallsImageOutline;
    [SerializeField] private GameObject roofsImageOutline;
    [SerializeField] private GameObject partsImageOutline;
    [SerializeField] private GameObject othersImageOutline;
    private int currentIndex = 1;
    private const int MAX_INDEX = 5;

    private BlockTypes defaultStartBlock = BlockTypes.floor;

    private Dictionary<int, BlockPanelUI> panelsForNonVis = new Dictionary<int, BlockPanelUI>();
    
    private List<GameObject> floors = new List<GameObject>();
    private List<GameObject> walls = new List<GameObject>();
    private List<GameObject> roofs = new List<GameObject>();
    private List<GameObject> parts = new List<GameObject>();
    private List<GameObject> others = new List<GameObject>();

    //private SoundUI sounds;
    private GameManager gm;
    private LevelControl lc;
    private BlockManager bm;
    private AssetManager assets;

    [SerializeField] private BlockManager blockManager;


    private void Awake()
    {
        //sounds = SoundUI.Instance;
        gm = GameManager.Instance;
        lc = gm.LevelControl;
        bm = gm.BlockManager;
        assets = gm.Assets;

        if (Globals.IsMobile)
        {
            gridLayoutGroup.padding.left = leftM;
            gridLayoutGroup.padding.top = topM;
            gridLayoutGroup.padding.bottom = botomM;

            gridLayoutGroup.cellSize = cellSizeM;
            gridLayoutGroup.spacing = spacingM;

            blocksPanel.transform.localScale = Vector3.one * 0.8f;

            gridLayoutGroupForVis.padding.left = leftM;
            gridLayoutGroupForVis.padding.top = topM;
            gridLayoutGroupForVis.padding.bottom = botomM;

            gridLayoutGroupForVis.cellSize = cellSizeM;
            gridLayoutGroupForVis.spacing = spacingM;

            blocksPanelForVis.transform.localScale = Vector3.one * 0.8f;
        }
        else
        {
            gridLayoutGroup.padding.left = leftPC;
            gridLayoutGroup.padding.top = topPC;
            gridLayoutGroup.padding.bottom = botomPC;

            gridLayoutGroup.cellSize = cellSizePC;
            gridLayoutGroup.spacing = spacingPC;

            blocksPanel.transform.localScale = Vector3.one * 0.7f;

            gridLayoutGroupForVis.padding.left = leftPC;
            gridLayoutGroupForVis.padding.top = topPC;
            gridLayoutGroupForVis.padding.bottom = botomPC;

            gridLayoutGroupForVis.cellSize = cellSizePC;
            gridLayoutGroupForVis.spacing = spacingPC;

            blocksPanelForVis.transform.localScale = Vector3.one * 0.7f;

            floorsFilterButton.transform.localScale = Vector3.one * 0.9f;
            wallsFilterButton.transform.localScale = Vector3.one * 0.9f;
            roofsFilterButton.transform.localScale = Vector3.one * 0.9f;
            partsFilterButton.transform.localScale = Vector3.one * 0.9f;
            othersFilterButton.transform.localScale = Vector3.one * 0.9f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {        
        blocksPanel.SetActive(false);
        blocksPanelForVis.SetActive(false);

        if (gm.IsWalkthroughGame)
        {
            missionName.text = getMissionName(Globals.CurrentLevel);

            if (missionName.text.Length <= 14)
            {
                missionName.fontSize = 50;
            }
            else
            {
                missionName.fontSize = 40;
            }

            missionIcon.sprite = lc.GetLevelData.MissionIcon;
        }
        blockAdditionalInfo.SetActive(false);
        blockAdditionalInfoPosition = blockAdditionalInfo.GetComponent<RectTransform>().anchoredPosition;
        

        createBlocksPanel();
        resetIcons();
        floorsImage.color = Color.yellow;
        floorsImage.transform.localScale = Vector3.one;
        currentIndex = 1;
        floorsImageOutline.SetActive(true);

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

    public void UpdateOutlines()
    {
        if (bm.CurrentActiveBlock == null) return;

        if (gm.IsWalkthroughGame)
        {            
            foreach (int key in panelsForVis.Keys)
            {
                if (bm.CurrentActiveBlock.ID.ID != key && panelsForVis[key].GetOutline)
                {
                    panelsForVis[key].SetOutline(false);
                }
                else if (bm.CurrentActiveBlock.ID.ID == key && !panelsForVis[key].GetOutline)
                {
                    panelsForVis[key].SetOutline(true);
                }
            }
        }
        else
        {            
            foreach (int key in panelsForNonVis.Keys)
            {
                if (bm.CurrentActiveBlock.ID.ID != key && panelsForNonVis[key].GetOutline)
                {
                    panelsForNonVis[key].SetOutline(false);
                }
                else if (bm.CurrentActiveBlock.ID.ID == key && !panelsForNonVis[key].GetOutline)
                {
                    panelsForNonVis[key].SetOutline(true);
                }
            }
        }
    }

    
    private void getLastSpot()
    {
        StartCoroutine(playGetLastSlot());
    }    
    private IEnumerator playGetLastSlot()
    {
        yield return new WaitForSeconds(Time.deltaTime);

        switch (defaultStartBlock)
        {
            case BlockTypes.floor:
                rectContainer.anchoredPosition = new Vector2(0, YcoordInFloorsNonVis);
                break;

            case BlockTypes.wall:
                rectContainer.anchoredPosition = new Vector2(0, YcoordInWallsNonVis);
                break;

            case BlockTypes.roof:
                rectContainer.anchoredPosition = new Vector2(0, YcoordInRoofsNonVis);
                break;

            case BlockTypes.parts:
                rectContainer.anchoredPosition = new Vector2(0, YcoordInPartsNonVis);
                break;

            case BlockTypes.others:
                rectContainer.anchoredPosition = new Vector2(0, YcoordInOthersNonVis);
                break;
        }
    }

    private void saveLastSpot()
    {
        saveLastSpotForNonVis(rectContainer.anchoredPosition.y);
    }

    private void saveLastSpotForNonVis(float rectY)
    {
        switch(defaultStartBlock)
        {
            case BlockTypes.floor:
                YcoordInFloorsNonVis = rectY;
                break;

            case BlockTypes.wall:
                YcoordInWallsNonVis = rectY;
                break;

            case BlockTypes.roof:
                YcoordInRoofsNonVis = rectY;
                break;

            case BlockTypes.parts:
                YcoordInPartsNonVis = rectY;
                break;

            case BlockTypes.others:
                YcoordInOthersNonVis = rectY;
                break;
        }
    }


    private void activateFloors()
    {
        if (defaultStartBlock == BlockTypes.floor) return;

        resetIcons();
        floorsImage.color = Color.yellow;
        floorsImage.transform.localScale = Vector3.one;

        currentIndex = 1;

        //sounds.PlayUISound(SoundsUI.click);
        SoundUI.Instance.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.floor;
        filterBlocksPanel();
        floorsImageOutline.SetActive(true);
    }

    private void activateWalls()
    {
        if (defaultStartBlock == BlockTypes.wall) return;

        resetIcons();
        wallsImage.color = Color.yellow;
        wallsImage.transform.localScale = Vector3.one;

        currentIndex = 2;

        SoundUI.Instance.PlayUISound(SoundsUI.click);
        //sounds.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.wall;
        filterBlocksPanel();
        wallsImageOutline.SetActive(true);
    }

    private void activateRoofs()
    {
        if (defaultStartBlock == BlockTypes.roof) return;

        resetIcons();
        roofsImage.color = Color.yellow;
        roofsImage.transform.localScale = Vector3.one;

        currentIndex = 3;

        //sounds.PlayUISound(SoundsUI.click);
        SoundUI.Instance.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.roof;
        filterBlocksPanel();
        roofsImageOutline.SetActive(true);
    }

    private void activateParts()
    {
        if (defaultStartBlock == BlockTypes.parts) return;

        resetIcons();
        partsImage.color = Color.yellow;
        partsImage.transform.localScale = Vector3.one;

        currentIndex = 4;

        //sounds.PlayUISound(SoundsUI.click);
        SoundUI.Instance.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.parts;
        filterBlocksPanel();
        partsImageOutline.SetActive(true);
    }

    private void activateOthers()
    {
        if (defaultStartBlock == BlockTypes.others) return;
        

        resetIcons();
        othersImage.color = Color.yellow;
        othersImage.transform.localScale = Vector3.one;

        currentIndex = 5;

        //sounds.PlayUISound(SoundsUI.click);
        SoundUI.Instance.PlayUISound(SoundsUI.click);
        defaultStartBlock = BlockTypes.others;
        filterBlocksPanel();
        othersImageOutline.SetActive(true);
    }

    private IEnumerator showAdditionalInfo()
    {
        blockAdditionalInfo.gameObject.SetActive(true);

        RectTransform r = blockAdditionalInfo.GetComponent<RectTransform>();
        r.anchoredPosition = blockAdditionalInfoPosition + new Vector2(1000, 0);
        r.DOAnchorPos(blockAdditionalInfoPosition, 0.1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.1f);        
    }
    private IEnumerator hideAdditionalInfo()
    {
        RectTransform r = blockAdditionalInfo.GetComponent<RectTransform>();        
        r.DOAnchorPos(blockAdditionalInfoPosition + new Vector2(1000, 0), 0.1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.1f);
        blockAdditionalInfo.gameObject.SetActive(false);
    }

    private void Update()
    {        

        if (!gm.IsBuildMode && blockAdditionalInfo.activeSelf)
        {
            //blockAdditionalInfo.SetActive(false);
            StartCoroutine(hideAdditionalInfo());
        }

        
        if (!isFirstStageInform && gm.IsWalkthroughGame && gm.IsBuildMode)
        {
            isFirstStageInform = true;

            SoundUI.Instance.PlayUISoundSuccess3(1f);

            StartCoroutine(changeRegimeText(1f, $"{Globals.Language.Stage}: {lc.CurrentStageNumber()} {Globals.Language.StageFrom} {lc.StagesAmount}"));
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



        if (Input.GetKeyDown(KeyCode.J))
        {
            rectContainer.anchoredPosition = new Vector2(0, 388.1f);
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
            checkCellSize();
            return;
        }

        if (currentStage != null && !currentStage.Equals(stage))
        {
            foreach (int key in panelsForVis.Keys)
            {
                Destroy(panelsForVis[key].gameObject);
            }
            SoundUI.Instance.PlayUISoundSuccess3(0.1f);
            StartCoroutine(changeRegimeText(0, $"{Globals.Language.Stage}: {lc.CurrentStageNumber()} {Globals.Language.StageFrom} {lc.StagesAmount}"));
            gm.MainPlayerControl.CheckUnstuck();
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

        checkCellSize();
    }
    private void checkCellSize()
    {
        int leftBlocks = 0;

        foreach (int key in panelsForVis.Keys)
        {
            if (panelsForVis[key].gameObject.activeSelf)
            {
                leftBlocks++;
            }
        }

        if (gm.IsWalkthroughGame && !Globals.IsMobile && leftBlocks > 8)
        {
            gridLayoutGroupForVis.cellSize = cellSizePCver2;
        }
        else if (gm.IsWalkthroughGame && !Globals.IsMobile && leftBlocks <= 8)
        {
            gridLayoutGroupForVis.cellSize = cellSizePC;
        }

        if (gm.IsWalkthroughGame && Globals.IsMobile && leftBlocks > 6)
        {
            gridLayoutGroupForVis.cellSize = cellSizeMver2;
        }
        else if (gm.IsWalkthroughGame && Globals.IsMobile && leftBlocks <= 6)
        {
            gridLayoutGroupForVis.cellSize = cellSizeM;
        }
    }

    public void UpdateAdditionalBlockInfoNonWalk()
    {
        if (!gm.IsBuildMode) return;

        Block block = bm.CurrentActiveBlock;
        if (block == null)
        {
            //blockAdditionalInfo.SetActive(false);
            StartCoroutine(hideAdditionalInfo());
            return;
        }
        else
        {
            if (!blockAdditionalInfo.activeSelf)
            {
                //blockAdditionalInfo.SetActive(true);
                StartCoroutine(showAdditionalInfo());
            }
        }

        if (currentBlock != null && currentBlock.ID.ID != block.ID.ID)
        {
            currentBlock = block;
            StartCoroutine(playChangeAdditionalInfo(blockAdditionalInfo.transform, block));
            return;
        }
        else if (currentBlock != null && currentBlock.ID.ID == block.ID.ID)
        {
            //blockAdditionalInfo.transform.DOPunchPosition(new Vector3(UnityEngine.Random.Range(-20,20), UnityEngine.Random.Range(-10, 10), 1), 0.3f, 30).SetEase(Ease.InOutQuad);

            int rnd = UnityEngine.Random.Range(0, 2);
            int x1 = rnd == 1 ? 1 : -1;
            rnd = UnityEngine.Random.Range(0, 2);
            int x2 = rnd == 1 ? 1 : -1;

            blockAdditionalInfo.transform.DOPunchPosition(new Vector3(25 * x1, 25 * x2, 1), 0.3f, 30).SetEase(Ease.InOutQuad);
        }

        currentBlock = block;

        blockIcon.sprite = block.BlockIcon;

        
        amountLeftText.gameObject.SetActive(false);
        progressLeft.gameObject.SetActive(false);
    }

    public void UpdateAdditionalBlockInfo()
    {        
        if (!gm.IsBuildMode) return;
                
        Block block = bm.CurrentActiveBlock;
        if (block == null)
        {
            //blockAdditionalInfo.SetActive(false);
            StartCoroutine(hideAdditionalInfo());
            return;
        }
        else
        {
            if (!blockAdditionalInfo.activeSelf)
            {
                //blockAdditionalInfo.SetActive(true);
                StartCoroutine(showAdditionalInfo());
            }            
        }

        if (currentBlock != null && currentBlock.ID.ID != block.ID.ID)
        {
            currentBlock = block;
            StartCoroutine(playChangeAdditionalInfo(blockAdditionalInfo.transform, block));
            return;
        }
        else if (currentBlock != null && currentBlock.ID.ID == block.ID.ID)
        {
            //blockAdditionalInfo.transform.DOPunchPosition(new Vector3(UnityEngine.Random.Range(-20,20), UnityEngine.Random.Range(-10, 10), 1), 0.3f, 30).SetEase(Ease.InOutQuad);

            int rnd = UnityEngine.Random.Range(0, 2);
            int x1 = rnd == 1 ? 1 : -1;
            rnd = UnityEngine.Random.Range(0, 2);
            int x2 = rnd == 1 ? 1 : -1;

            blockAdditionalInfo.transform.DOPunchPosition(new Vector3(25 * x1, 25 * x2, 1), 0.3f, 30).SetEase(Ease.InOutQuad);
        }

        currentBlock = block;

        blockIcon.sprite = block.BlockIcon;

        int overall = 0;
        int left = 0;
        int done = 0;

        Stage s = lc.GetCurrentStage();

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

        if (gm.IsWalkthroughGame)
        {
            int overall = 0;
            int left = 0;
            int done = 0;

            Stage s = lc.GetCurrentStage();

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
        }
        else
        {
            amountLeftText.gameObject.SetActive(false);
            progressLeft.gameObject.SetActive(false);
        }

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
                if (!panelsForNonVis.ContainsKey(sourceIDs[i])) panelsForNonVis.Add(sourceIDs[i], g.GetComponent<BlockPanelUI>());
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
        //sounds.PlayUISound(SoundsUI.click);
        SoundUI.Instance.PlayUISound(SoundsUI.click);

        if (gm.IsWalkthroughGame)
        {
            if (blocksPanelForVis.activeSelf)
            {
                HideAllPanel();
                return;
            }

            //blocksPanelForVis.SetActive(true);

            try
            {
                StartCoroutine(playShow(blocksPanelForVis.transform));
                updateMissionInfo();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex + " 331");
            }

                     
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
        r.DOAnchorPos(new Vector2(0, 0), 0.2f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.2f);
        
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

        getLastSpot();
    }

    public void HideAllPanel()
    {
        saveLastSpot();

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
        r.DOAnchorPos(new Vector2(-1200, 0), 0.1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.1f);
        t.gameObject.SetActive(false);
    }

    private void resetIcons()
    {
        if (gm.IsWalkthroughGame) return;

        floorsImage.color = new Color(1, 1, 1, 0.8f);
        floorsImage.transform.localScale = Vector3.one * 0.8f;

        wallsImage.color = new Color(1, 1, 1, 0.8f);
        wallsImage.transform.localScale = Vector3.one * 0.8f;

        roofsImage.color = new Color(1, 1, 1, 0.8f);
        roofsImage.transform.localScale = Vector3.one * 0.8f;

        partsImage.color = new Color(1, 1, 1, 0.8f);
        partsImage.transform.localScale = Vector3.one * 0.8f;

        othersImage.color = new Color(1, 1, 1, 0.8f);
        othersImage.transform.localScale = Vector3.one * 0.8f;

        floorsImageOutline.SetActive(false);
        wallsImageOutline.SetActive(false);
        roofsImageOutline.SetActive(false);
        partsImageOutline.SetActive(false);
        othersImageOutline.SetActive(false);
    }

    private string getMissionName(int level)
    {
        return Globals.Language.MissionName[level];        
    }

    private IEnumerator changeRegimeText(float delay, string newText)
    {        
        yield return new WaitForSeconds(delay);
        stageInformer.gameObject.SetActive(true);
        stageInformer.color = new Color(1, 1, 0, 1);
        stageInformer.text = newText;        
        RectTransform r = stageInformer.GetComponent<RectTransform>();
        Vector2 pos = new Vector2(0, -200);
        r.anchoredPosition = pos + new Vector2(-1500,0);
        r.DOAnchorPos(pos, 0.25f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.25f);

        stageInformer.transform.DOShakeScale(0.2f, 1, 30).SetEase(Ease.InOutBounce);
        yield return new WaitForSeconds(2.5f);

        r.DOAnchorPos(pos + new Vector2(1500, 0), 0.25f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.25f);
        stageInformer.gameObject.SetActive(false);
      
    }

}
