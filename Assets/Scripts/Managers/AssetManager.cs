using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssetManager : MonoBehaviour
{
    [SerializeField] private Transform blockLocation;

    [SerializeField] private Transform floorsLocation;
    [SerializeField] private Transform wallsLocation;
    
    private Dictionary<int, GameObject> linkIDtoAsset = new Dictionary<int, GameObject>();

    //TYPES
    private List<int> floorsIDs = new List<int>();
    private List<int> wallsIDs = new List<int>();
    //=
    [SerializeField] private GameObject markerExample;
    private GameObject marker;
    public Transform GetMarker => marker.transform;
    //=
    [SerializeField] private GameObject markerDestroerExample;
    private GameObject markerDestroer;
    public Transform GetMarkerDestroer => markerDestroer.transform;

    private void Awake()
    {
        initAssetsLink(floorsLocation);
        initAssetsLink(wallsLocation);

        marker = Instantiate(markerExample);
        marker.SetActive(false);

        markerDestroer = Instantiate(markerDestroerExample);
        markerDestroer.SetActive(false);
    }

    public int[] GetArrayOfFloorsIds => floorsIDs.ToArray();
    public int[] GetArrayOfWallsIds => wallsIDs.ToArray();

    public GameObject GetGameObjectByID(int ID)
    {
        GameObject g = Instantiate(linkIDtoAsset[ID], blockLocation);
        g.SetActive(true);
        return g;
    }

    public Block GetBlockDataByID(int ID)
    {
        return linkIDtoAsset[ID].GetComponent<Block>();
    }

    private void initAssetsLink(Transform location)
    {
        for (int i = 0; i < location.childCount; i++)
        {
            int id = location.GetChild(i).GetComponent<Identificator>().ID;

            if (linkIDtoAsset.ContainsKey(id))
            {
                print("ERROR! more than one IDs!");
            }
            else
            {
                linkIDtoAsset.Add(id, location.GetChild(i).gameObject);

                switch(location.GetChild(i).GetComponent<Block>().BlockType)
                {
                    case BlockTypes.floor:
                        floorsIDs.Add(id);
                        break;

                    case BlockTypes.wall:
                        wallsIDs.Add(id);
                        break;
                }
            }
        }
    }
}
