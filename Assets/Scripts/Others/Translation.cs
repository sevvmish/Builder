using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Translations", menuName = "Languages", order = 1)]
public class Translation : ScriptableObject
{
    public string UpArrowLetter;
    public string DownArrowLetter;
    public string LeftArrowLetter;
    public string RightArrowLetter;

    public string JumpLetter;
    public string JumpUpLetter;
    public string JumpDownLetter;

    public string RegimeSpectator;
    public string RegimeBuilder;
    public string RegimeBuilderDeleter;

    public string Blocks;
    public string Build;

    public string ToBuild;
    public string ToDelete;
    public string ToRotate;
    public string ToCancel;
    public string ToBuildRegime;
    public string ToDeleteRegime;

    public string Continue;
    public string Exit;
    public string Sound;
    public string Music;

    public string PlayerIsObstacle;

    public string BlockArrow;
    public string BuildRegimeArrow;
    public string OptionsArrow;

    public string SetBlockHelper;
    public string RotateBlockHelper;
    public string CancelBlockHelper;
    public string HowToDelHelper;

    public string DelBlockHelper;
    public string HowToBuildHelper;

    public string Stage;
    public string StageFrom;
    public string StagesAmount;
    public string CurrentStage;

    public string[] MissionName;

    public string Play;
    public string PlayAgain;
    public string CustomGame;
    
    public string Level;
    public string Curr;

    public string BuildingCompleted;
    public string ContinuePlay;
    public string ContinuePlayHelper;

    public string MovementHintPCText;
    public string MovementHintMobText;

    public string JumpHintPCText;
    public string JumpHintMobText;

    public string RegimeHintPCText;
    public string RegimeHintMobText;

    public string CameraHintPCText;
    public string CameraHintMobText;

    public string JumpHintInBuildingText;
    public string CurrentBlockChosenText;

    public string BuildBlockPCText;
    public string BuildBlockMobText;

    public string ChooseBlockPCText;
    public string ChooseBlockMobText;

    public string EndGameWalkText;

    public string CustomGameHintText;
    public string WalkGameHintText;

    public Translation() { }
}
