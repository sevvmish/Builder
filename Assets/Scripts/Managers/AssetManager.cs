using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssetManager : MonoBehaviour
{    
    [SerializeField] private Identificator[] Assets;
    private Dictionary<int, GameObject> linkIDtoAsset = new Dictionary<int, GameObject>();

    [SerializeField] private GameObject markerExample;
    private GameObject marker;
    public Transform GetMarker => marker.transform;

    private void Awake()
    {
        for (int i = 0; i < Assets.Length; i++)
        {
            if (linkIDtoAsset.ContainsKey(Assets[i].ID))
            {
                print("ERROR! more than one IDs!");
            }
            else
            {
                linkIDtoAsset.Add(Assets[i].ID, Assets[i].gameObject);
            }
        }

        marker = Instantiate(markerExample);
        marker.SetActive(false);
    }

    public GameObject GetGameObjectByID(int ID)
    {
        GameObject g = Instantiate(linkIDtoAsset[ID]);
        g.SetActive(true);
        return g;
    }

    
}
