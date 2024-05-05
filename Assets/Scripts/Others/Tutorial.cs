using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject movementHintPC;
    [SerializeField] private TextMeshProUGUI movementHintPCText;
    [SerializeField] private GameObject movementHintMob;
    [SerializeField] private TextMeshProUGUI movementHintMobText;

    [SerializeField] private GameObject jumpHintPC;
    [SerializeField] private TextMeshProUGUI jumpHintPCText;
    [SerializeField] private GameObject jumpHintMob;
    [SerializeField] private TextMeshProUGUI jumpHintMobText;

    [SerializeField] private GameObject regimeHintPC;
    [SerializeField] private TextMeshProUGUI regimeHintPCText;
    [SerializeField] private GameObject regimeHintMob;
    [SerializeField] private TextMeshProUGUI regimeHintMobText;

    [SerializeField] private GameObject cameraHintPC;
    [SerializeField] private TextMeshProUGUI cameraHintPCText;
    [SerializeField] private GameObject cameraHintMob;
    [SerializeField] private TextMeshProUGUI cameraHintMobText;

    [SerializeField] private GameObject jumpHintInBuilding;
    [SerializeField] private TextMeshProUGUI jumpHintInBuildingText;

    [SerializeField] private GameObject currentBlockChosen;
    [SerializeField] private TextMeshProUGUI currentBlockChosenText;

    [SerializeField] private GameObject buildBlockPC;
    [SerializeField] private TextMeshProUGUI buildBlockPCText;
    [SerializeField] private GameObject buildBlockMob;
    [SerializeField] private TextMeshProUGUI buildBlockMobText;

    [SerializeField] private GameObject chooseBlockPC;
    [SerializeField] private TextMeshProUGUI chooseBlockPCText;
    [SerializeField] private GameObject chooseBlockMob;
    [SerializeField] private TextMeshProUGUI chooseBlockMobText;
    [SerializeField] private GameObject panelWithBlocks;

    [SerializeField] private GameObject endGameWalk;
    [SerializeField] private TextMeshProUGUI endGameWalkText;

    private bool isReady;
    private GameManager gm;
    private LevelControl lc;
    private float baseAwait = 3f;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        if (gm.IsWalkthroughGame)
        {
            lc = gm.LevelControl;
        }
        

        movementHintPC.SetActive(false);
        movementHintMob.SetActive(false);
        jumpHintPC.SetActive(false);
        jumpHintMob.SetActive(false);
        regimeHintPC.SetActive(false);
        regimeHintMob.SetActive(false);
        cameraHintPC.SetActive(false);
        cameraHintMob.SetActive(false);
        jumpHintInBuilding.SetActive(false);
        currentBlockChosen.SetActive(false);
        buildBlockPC.SetActive(false);
        buildBlockMob.SetActive(false);
        chooseBlockPC.SetActive(false);
        chooseBlockMob.SetActive(false);
        endGameWalk.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.IsInitiated && !isReady)
        {
            isReady = true;
            localize();
            if (!Globals.MainPlayerData.IsTutWalk && gm.IsWalkthroughGame)
            {
                Globals.MainPlayerData.IsTutWalk = true;
                SaveLoadManager.Save();
                StartCoroutine(playTutWalk());
            }
        }
    }

    private void localize()
    {
        movementHintPCText.text = Globals.Language.MovementHintPCText;
        movementHintMobText.text = Globals.Language.MovementHintMobText;

        jumpHintPCText.text = Globals.Language.JumpHintPCText;
        jumpHintMobText.text = Globals.Language.JumpHintMobText;

        regimeHintPCText.text = Globals.Language.RegimeHintPCText;
        regimeHintMobText.text = Globals.Language.RegimeHintMobText;

        cameraHintPCText.text = Globals.Language.CameraHintPCText;
        cameraHintMobText.text = Globals.Language.CameraHintMobText;

        jumpHintInBuildingText.text = Globals.Language.JumpHintInBuildingText;

        currentBlockChosenText.text = Globals.Language.CurrentBlockChosenText;

        buildBlockPCText.text = Globals.Language.BuildBlockPCText;
        buildBlockMobText.text = Globals.Language.BuildBlockMobText;

        chooseBlockPCText.text = Globals.Language.ChooseBlockPCText;
        chooseBlockMobText.text = Globals.Language.ChooseBlockMobText;
        endGameWalkText.text = Globals.Language.EndGameWalkText;
    }
    private IEnumerator playTutWalk()
    {
        yield return new WaitForSeconds(3);

        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        if (Globals.IsMobile)
        {            
            movementHintMob.SetActive(true);
        }
        else
        {
            movementHintPC.SetActive(true);
        }

        yield return new WaitForSeconds(baseAwait);

        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        if (Globals.IsMobile)
        {
            jumpHintMob.SetActive(true);
        }
        else
        {
            jumpHintPC.SetActive(true);
        }

        yield return new WaitForSeconds(baseAwait);

        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        if (Globals.IsMobile)
        {
            movementHintMob.SetActive(false);
            cameraHintMob.SetActive(true);
        }
        else
        {
            movementHintPC.SetActive(false);
            cameraHintPC.SetActive(true);
        }

        yield return new WaitForSeconds(baseAwait);

        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        if (Globals.IsMobile)
        {
            jumpHintMob.SetActive(false);            
        }
        else
        {
            jumpHintPC.SetActive(false);            
        }

        yield return new WaitForSeconds(baseAwait);

        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        if (Globals.IsMobile)
        {
            regimeHintMob.SetActive(true);
            cameraHintMob.SetActive(false);
        }
        else
        {
            regimeHintPC.SetActive(true);
            cameraHintPC.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        while (!gm.IsBuildMode)
        {
            yield return new WaitForSeconds(0.1f);
        }

        movementHintPC.SetActive(false);
        movementHintMob.SetActive(false);
        jumpHintMob.SetActive(false);
        jumpHintPC.SetActive(false);
        cameraHintMob.SetActive(false);
        cameraHintPC.SetActive(false);

        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        if (Globals.IsMobile)
        {
            regimeHintMob.SetActive(false);
        }
        else
        {
            regimeHintPC.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        jumpHintInBuilding.SetActive(true);

        yield return new WaitForSeconds(baseAwait);
        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        currentBlockChosen.SetActive(true);

        yield return new WaitForSeconds(baseAwait);
        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        jumpHintInBuilding.SetActive(false);
        if (Globals.IsMobile)
        {
            buildBlockMob.SetActive(true);
        }
        else
        {
            buildBlockPC.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        for (float i = 0; i < 20; i += 0.1f)
        {
            if (lc.CurrentStageNumber() > 1) break;
            yield return new WaitForSeconds(0.1f);
        }

        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        currentBlockChosen.SetActive(false);
        if (Globals.IsMobile)
        {
            chooseBlockMob.SetActive(true);
            buildBlockMob.SetActive(false);
        }
        else
        {
            chooseBlockPC.SetActive(true);
            buildBlockPC.SetActive(false);
        }

        bool isPanelOpened = false;
        yield return new WaitForSeconds(0.5f);
        for (float i = 0; i < 20; i += 0.1f)
        {
            if (panelWithBlocks.activeSelf || gm.IsWinWalkthroughGame)
            {
                isPanelOpened = true;
                break;
            }
                
            yield return new WaitForSeconds(0.1f);
        }

        if (gm.IsWinWalkthroughGame)
        {
            movementHintPC.SetActive(false);
            movementHintMob.SetActive(false);
            jumpHintPC.SetActive(false);
            jumpHintMob.SetActive(false);
            regimeHintPC.SetActive(false);
            regimeHintMob.SetActive(false);
            cameraHintPC.SetActive(false);
            cameraHintMob.SetActive(false);
            jumpHintInBuilding.SetActive(false);
            currentBlockChosen.SetActive(false);
            buildBlockPC.SetActive(false);
            buildBlockMob.SetActive(false);
            chooseBlockPC.SetActive(false);
            chooseBlockMob.SetActive(false);
            endGameWalk.SetActive(false);
        }


        SoundUI.Instance.PlayUISound(SoundsUI.pop);
        if (Globals.IsMobile)
        {
            chooseBlockMob.SetActive(false);
        }
        else
        {
            chooseBlockPC.SetActive(false);
        }

        while (isPanelOpened && panelWithBlocks.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (!gm.IsWinWalkthroughGame)
        {
            endGameWalk.SetActive(true);
            yield return new WaitForSeconds(baseAwait*2);
            endGameWalk.SetActive(false);
        }

    }
}
