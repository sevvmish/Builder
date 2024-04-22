using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreviewUI : MonoBehaviour
{
    public int CurrentLevelNumber { get; private set; }

    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform levels;

    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI levelStagesAmount;
    [SerializeField] private TextMeshProUGUI levelStagesAmountText;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private GameObject lockedIcon;

    private bool isReady;
    private SoundUI sounds;
    private Vector3 cameraLvl5 = new Vector3 (-7.5f, 20, -15);

    private void Start()
    {
        sounds = SoundUI.Instance;
        lockedIcon.SetActive(false);

        leftButton.onClick.AddListener(() => 
        {
            if (CurrentLevelNumber < 1) return;
            sounds.PlayUISound(SoundsUI.click);
            CurrentLevelNumber--;
            showLevel(CurrentLevelNumber);
        });

        rightButton.onClick.AddListener(() =>
        {
            if (CurrentLevelNumber >= levels.childCount) return;
            sounds.PlayUISound(SoundsUI.click);
            CurrentLevelNumber++;
            showLevel(CurrentLevelNumber);
        });
    }

    private void resetAll()
    {
        for (int i = 0; i < levels.childCount; i++)
        {
            levels.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void showLevel(int level)
    {
        if (CurrentLevelNumber < 1)
        {
            leftButton.gameObject.SetActive(false);
        }
        else
        {
            leftButton.gameObject.SetActive(true);
        }

        if (CurrentLevelNumber >= (levels.childCount-1))
        {
            rightButton.gameObject.SetActive(false);
        }
        else
        {
            rightButton.gameObject.SetActive(true);
        }

        if (level >= levels.childCount) return;

        resetAll();
        levels.GetChild(level).gameObject.SetActive(true);

        if (CurrentLevelNumber > Globals.CurrentLevel)
        {
            lockedIcon.SetActive(true);
        }
        else
        {
            lockedIcon.SetActive(false);
        }        

        if (level < 5)
        {
            mainCamera.position = cameraLvl5;
        }

        levelNameText.text = Globals.Language.CurrentStage + ":";
        levelName.text = getMissionName(level);
        levelStagesAmountText.text = Globals.Language.StagesAmount + ":";
        levelStagesAmount.text = levels.GetChild(level).childCount.ToString();
    }

    private void Update()
    {
        if (Globals.IsInitiated && !isReady)
        {
            isReady = true;
            CurrentLevelNumber = Globals.CurrentLevel;
            resetAll();
            showLevel(CurrentLevelNumber);
        }
    }

    private string getMissionName(int level)
    {
        return Globals.Language.MissionName[level];
    }
}
