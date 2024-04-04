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

    public string BlockArrow;
    public string BuildRegimeArrow;
    public string OptionsArrow;

    public string SetBlockHelper;
    public string RotateBlockHelper;
    public string CancelBlockHelper;
    public string HowToDelHelper;

    public string DelBlockHelper;
    public string HowToBuildHelper;

    public Translation() { }
}
