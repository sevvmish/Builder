using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHelperPCButtons : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private GameObject movementHelper;
    [SerializeField] private TextMeshProUGUI upArrowMove;
    [SerializeField] private TextMeshProUGUI downArrowMove;
    [SerializeField] private TextMeshProUGUI leftArrowMove;
    [SerializeField] private TextMeshProUGUI rightArrowMove;

    [Header("Jump")]
    [SerializeField] private GameObject jumpHelper;
    [SerializeField] private TextMeshProUGUI jumpText;
    [SerializeField] private TextMeshProUGUI jumpUpText;
    [SerializeField] private TextMeshProUGUI jumpDownText;

    [SerializeField] private TextMeshProUGUI blockArrowText;
    [SerializeField] private TextMeshProUGUI buildRegimeArrowText;
    [SerializeField] private TextMeshProUGUI optionsArrowText;

    [SerializeField] private GameObject buildingBlocks;
    [SerializeField] private TextMeshProUGUI setBlockText;
    [SerializeField] private TextMeshProUGUI rotateBlockText;
    [SerializeField] private TextMeshProUGUI cancelBlockText;
    [SerializeField] private TextMeshProUGUI howToDelBlocksText;

    [SerializeField] private GameObject deletingBlocks;
    [SerializeField] private TextMeshProUGUI delBlockText;
    [SerializeField] private TextMeshProUGUI howToBuildBlocksText;

    private bool isBuildMode;
    private bool isBuildingInBuilding;
    private GameManager gm;
    private BlockManager bm;
    private bool isWinWalkthrough;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        bm = gm.BlockManager;

        if (!Globals.IsMobile)
        {
            upArrowMove.text = Globals.Language.UpArrowLetter;
            downArrowMove.text = Globals.Language.DownArrowLetter;
            leftArrowMove.text = Globals.Language.LeftArrowLetter;
            rightArrowMove.text = Globals.Language.RightArrowLetter;

            jumpText.text = Globals.Language.JumpLetter;
            jumpUpText.text = Globals.Language.JumpUpLetter;
            jumpDownText.text = Globals.Language.JumpDownLetter;

            movementHelper.SetActive(true);
            jumpHelper.SetActive(true);

            blockArrowText.gameObject.SetActive(true);
            buildRegimeArrowText.gameObject.SetActive(true);
            optionsArrowText.gameObject.SetActive(true);
            blockArrowText.text = Globals.Language.BlockArrow;
            buildRegimeArrowText.text = Globals.Language.BuildRegimeArrow;
            optionsArrowText.text = Globals.Language.OptionsArrow;

            buildingBlocks.SetActive(true);
            setBlockText.gameObject.SetActive(true);
            rotateBlockText.gameObject.SetActive(!gm.IsWalkthroughGame);
            cancelBlockText.gameObject.SetActive(!gm.IsWalkthroughGame);
            howToDelBlocksText.gameObject.SetActive(!gm.IsWalkthroughGame);
            setBlockText.text = Globals.Language.SetBlockHelper;
            rotateBlockText.text = Globals.Language.RotateBlockHelper;
            cancelBlockText.text = Globals.Language.CancelBlockHelper;
            howToDelBlocksText.text = Globals.Language.HowToDelHelper;

            deletingBlocks.SetActive(false);
            delBlockText.text= Globals.Language.DelBlockHelper;
            howToBuildBlocksText.text = Globals.Language.HowToBuildHelper;

            if (gm.IsWalkthroughGame)
            {
                setBlockText.GetComponent<RectTransform>().anchoredPosition = new Vector2(315, -37);
            }
        }
        else
        {
            blockArrowText.gameObject.SetActive(false);
            buildRegimeArrowText.gameObject.SetActive(false);
            optionsArrowText.gameObject.SetActive(false);

            setBlockText.gameObject.SetActive(false);
            rotateBlockText.gameObject.SetActive(false);
            cancelBlockText.gameObject.SetActive(false);

            movementHelper.SetActive(false);
            jumpHelper.SetActive(false);
            buildingBlocks.SetActive(false);
            deletingBlocks.SetActive(false);
        }

        isBuildMode = !gm.IsBuildMode;
        isBuildingInBuilding = !bm.IsBuildingBlocks;
    }

    private void Update()
    {        
        if (Globals.IsMobile) return;

        if (gm.IsWinWalkthroughGame && !isWinWalkthrough)
        {
            isWinWalkthrough = true;
            buildRegimeArrowText.color = new Color(1, 1, 0, 0.5f);
        }

        if (gm.IsBuildMode != isBuildMode)
        {
            isBuildMode = gm.IsBuildMode;

            if (isBuildMode)
            {
                jumpText.gameObject.SetActive(false);
                jumpUpText.gameObject.SetActive(true);
                jumpDownText.gameObject.SetActive(true);
                buildingBlocks.SetActive(true);
                deletingBlocks.SetActive(false);
            }
            else if (!isBuildMode)
            {
                jumpText.gameObject.SetActive(true);
                jumpUpText.gameObject.SetActive(false);
                jumpDownText.gameObject.SetActive(false);
                buildingBlocks.SetActive(false);
                deletingBlocks.SetActive(false);
            }
        }

        if (gm.IsBuildMode && !gm.IsWalkthroughGame)
        {
            if (isBuildingInBuilding != bm.IsBuildingBlocks)
            {
                isBuildingInBuilding = bm.IsBuildingBlocks;

                if (isBuildingInBuilding)
                {
                    buildingBlocks.SetActive(true);
                    deletingBlocks.SetActive(false);
                }
                else
                {
                    buildingBlocks.SetActive(false);
                    if (!gm.IsWalkthroughGame) deletingBlocks.SetActive(true);
                }
            }
        }
        
        
        
    }

}
