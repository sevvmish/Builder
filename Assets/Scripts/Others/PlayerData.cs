using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{   
    public string L;
    public int M;
    public int S;
    public int Mus;

    public float Zoom;
    public bool IsZoomCorrected;

    public bool IsLowFPS;

    public int Level;

    public bool AdvOff;

    public bool IsTutWalk;
    public bool IsTutCustom;

    public PlayerData()
    {        
        L = ""; //prefered language
        M = 1; //mobile platform? 1 - true;
        S = 1; // sound on? 1 - true;        
        Mus = 1; // music
        Zoom = 50; //camera zoom
        IsZoomCorrected = false;
        Level = 0;

        IsLowFPS = false;
        
        AdvOff = false;

        IsTutWalk = false;
        IsTutCustom = false;

        Debug.Log("created PlayerData instance");
    }


}
