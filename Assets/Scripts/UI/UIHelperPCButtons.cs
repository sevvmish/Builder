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

    private bool isBuildMode;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;

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

            blockArrowText.text = Globals.Language.BlockArrow;
            buildRegimeArrowText.text = Globals.Language.BuildRegimeArrow;
            optionsArrowText.text = Globals.Language.OptionsArrow;

        }
        else
        {
            movementHelper.SetActive(false);
            jumpHelper.SetActive(false);
        }

        isBuildMode = !gm.IsBuildMode;
    }

    private void Update()
    {
        if (Globals.IsMobile) return;

        if (gm.IsBuildMode != isBuildMode)
        {
            isBuildMode = gm.IsBuildMode;

            if (isBuildMode)
            {
                jumpText.gameObject.SetActive(false);
                jumpUpText.gameObject.SetActive(true);
                jumpDownText.gameObject.SetActive(true);
            }
            else if(!isBuildMode)
            {
                jumpText.gameObject.SetActive(true);
                jumpUpText.gameObject.SetActive(false);
                jumpDownText.gameObject.SetActive(false);
            }
        }
    }

}
