using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssetManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Skins;

    [SerializeField] private GameAsset[] Assets;
    private Dictionary<int, int> linkIDtoAsset = new Dictionary<int, int>();

    private void Awake()
    {
        for (int i = 0; i < Assets.Length; i++)
        {
            if (linkIDtoAsset.ContainsKey(Assets[i].ID)) print("ERROR: asset key allready in use for # - " + Assets[i].ID);
            linkIDtoAsset.Add(Assets[i].ID, i);
        }
    }

    public GameObject GetGameObjectByID(int ID)
    {
        return Assets[linkIDtoAsset[ID]].MainObject;
    }

    public AssetTypes GetTypeByID(int ID)
    {
        return Assets[linkIDtoAsset[ID]].AssetType;
    }

    public GameObject GetSkin(int index) => Skins[index];
}

[Serializable]
public struct GameAsset
{
    public int ID;
    public string Name;
    public GameObject MainObject;
    public AssetTypes AssetType;

    public GameAsset(int iD, string name, GameObject mainObject, AssetTypes a)
    {
        ID = iD;
        Name = name;
        MainObject = mainObject;
        AssetType = a;
    }
}

[Serializable]
public enum AssetTypes
{
    none,
    terrain,
    tree,
    grass,
    plant,
    stone,
    rock
}
