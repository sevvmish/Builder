using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreviewUI : MonoBehaviour
{
    public int CurrentLevelNumber { get; private set; }
    public int MaxLevels => levels.childCount;

    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform levels;

    [SerializeField] private RectTransform textsPlace;
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI levelStagesAmount;
    [SerializeField] private TextMeshProUGUI levelStagesAmountText;
            
    private Vector3 cameraLvl5 = new Vector3 (-7.5f, 20, -15);


    public void ScrollToCurrent()
    {
        CurrentLevelNumber = Globals.MainPlayerData.Level;
        ShowLevel(CurrentLevelNumber);
    }

    public void CurrentLevelMinus()
    {
        CurrentLevelNumber--;
        ShowLevel(CurrentLevelNumber);
    }

    public void CurrentLevelPlus()
    {
        CurrentLevelNumber++;
        ShowLevel(CurrentLevelNumber);
    }

    private void resetAll()
    {
        for (int i = 0; i < levels.childCount; i++)
        {
            levels.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ShowLevel(int level)
    {
        resetAll();

        

        if (level >= levels.childCount) return;

        
        levels.GetChild(level).gameObject.SetActive(true);

        //SCALE

        if (level < 19)
        {
            levels.localScale = Vector3.one * 0.75f;
        }
        else
        {
            levels.localScale = Vector3.one;
        }
        

        if (level < 5)
        {
            mainCamera.position = cameraLvl5;
        }

        
        if (Globals.CurrentLevel == level)
        {
            levelNameText.gameObject.SetActive(true);
            levelNameText.text = Globals.Language.CurrentStage + ":";
        }
        else
        {
            levelNameText.gameObject.SetActive(false);
        }

        levelName.text = getMissionName(level);

        if (levelName.text.Length > 17)
        {
            levelName.fontSize = 40;
        }
        else
        {
            levelName.fontSize = 50;
        }

        levelStagesAmountText.text = Globals.Language.StagesAmount + ":";
        levelStagesAmount.text = levels.GetChild(level).childCount.ToString();
    }

    public void UpdateData()
    {
        CurrentLevelNumber = Globals.MainPlayerData.Level;
        /*
        if (Globals.IsMobile)
        {
            textsPlace.anchoredPosition = new Vector2(0, 20);
        }
        else
        {
            textsPlace.anchoredPosition = new Vector2(0, 100);
        }
        
        if (!Globals.MainPlayerData.AdvOff && (DateTime.Now - Globals.TimeWhenLastInterstitialWas).TotalSeconds >= Globals.INTERSTITIAL_COOLDOWN)
        {
            playInterstitial(level);
            yield break;
        }
         
         */

        ShowLevel(CurrentLevelNumber);
    }

    private string getMissionName(int level)
    {
        return Globals.Language.MissionName[level];
    }
}
