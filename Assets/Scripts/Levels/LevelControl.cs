using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public int StagesAmount => Stages.Length;
    public int MaxLevels => gm.Assets.Levels.Length;
    public LevelData GetLevelData { get => levelData; }
    public Stage[] Stages { get => stages; }

    private Transform stagesLocation;
    private LevelData levelData;
    private Stage[] stages;
    private GameManager gm;

    
    public void SetData()
    {
        gm = GameManager.Instance;

        print("Level: " + Globals.CurrentLevel + " = " + Globals.MainPlayerData.Level);

        stagesLocation = gm.Assets.Levels[Globals.CurrentLevel];
        print(stagesLocation.gameObject.name + " !!!!!!!!!!!!");
        print(Globals.MainPlayerData.Level + " = " + Globals.CurrentLevel);
        levelData = stagesLocation.GetComponent<LevelData>();
        stages = new Stage[stagesLocation.childCount];

        for (int i = 0; i < stages.Length; i++)
        {
            stages[i] = new Stage();

            for (int j = 0; j < stagesLocation.GetChild(i).childCount; j++)
            {
                if (stagesLocation.GetChild(i).GetChild(j).TryGetComponent(out Block b))
                {
                    int id = b.GetComponent<Identificator>().ID;
                    GameObject g = gm.Assets.GetGameObjectByID(id);
                    g.transform.position = b.transform.position;
                    g.transform.eulerAngles = b.transform.eulerAngles;
                    g.transform.localScale = stagesLocation.GetChild(i).GetChild(j).localScale;

                    Block newB = g.GetComponent<Block>();
                    newB.SetVisualization(true);
                    stages[i].Blocks.Add(newB);
                    g.SetActive(false);
                }
            }

            stages[i].Assess();
        }

        UpdateProgress();
    }

    public void UpdateProgress()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].IsStageDone())
            {
                //
            }
            else
            {
                stages[i].Assess();
            }
        }

        if (GetCurrentStage() == null)
        {
            Debug.Log("GAME WIN!!!");
        }
        else
        {
            updateBlocks();
        }
    }
    public int GetFirstID()
    {
        Stage s = GetCurrentStage();

        if (s == null )
        {
            gm.WinGameWithVisualization();
            return 0;
        }

        if (s.Blocks.Count > 0)
        {
            for (int i = 0; i < s.Blocks.Count; i++)
            {
                if (!s.Blocks[i].IsFinalized)
                {
                    return s.Blocks[i].GetComponent<Identificator>().ID;
                }
            }
        }
        else
        {
            return 0;
        }
        
        return 0;
    }

    public bool IsIDForBlockOK(int id)
    {
        Stage s = GetCurrentStage();

        if (s == null)
        {
            gm.WinGameWithVisualization();
            return false;
        }

        if (s.Blocks.Count > 0)
        {
            for (int i = 0; i < s.Blocks.Count; i++)
            {
                if (!s.Blocks[i].IsFinalized && s.Blocks[i].ID.ID == id)
                {
                    return true;
                }
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    public int CurrentStageNumber()
    {
        int result = -1;
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].IsStageDone())
            {
                //
            }
            else
            {
                result = i;
                break;
            }
        }
        
        return result+1;
    }

    public Stage GetCurrentStage()
    {        
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].IsStageDone())
            {
                //
            }
            else
            {
                return stages[i];
            }
        }

        return null;
    }

    

    public void SetVisible(bool isVisible)
    {
        Stage s = GetCurrentStage();
        if (s == null)
        {
            return;
        }

        if (isVisible)
        {            
            for (int i = 0; i < s.Blocks.Count; i++)
            {
                if (!s.Blocks[i].IsFinalized)
                {
                    s.Blocks[i].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < s.Blocks.Count; i++)
            {
                if (!s.Blocks[i].IsFinalized)
                {
                    s.Blocks[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void updateBlocks()
    {
        Stage s = GetCurrentStage();
        
        for (int i = 0; i < s.Blocks.Count; i++)
        {
            if (!s.Blocks[i].IsFinalized)
            {
                if (gm.IsBuildMode) s.Blocks[i].gameObject.SetActive(true);
                s.Blocks[i].SetVisualization(true);
            }
        }

        gm.GetUI.BlockMenuUI.UpdateIconsForVis(s);
    }
}
