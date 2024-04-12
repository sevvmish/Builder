using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AssetManager : MonoBehaviour
{
    public Material VisualizationMaterial { get => visualizationMaterial; }

    [SerializeField] private Transform blockLocation;

    [SerializeField] private Transform floorsLocation;
    [SerializeField] private Transform wallsLocation;
    [SerializeField] private Transform roofsLocation;
    [SerializeField] private Transform partsLocation;
    [SerializeField] private Transform othersLocation;

    [SerializeField] private Material visualizationMaterial;

    private Dictionary<int, GameObject> linkIDtoAsset = new Dictionary<int, GameObject>();

    //TYPES
    private List<int> floorsIDs = new List<int>();
    private List<int> wallsIDs = new List<int>();
    private List<int> roofsIDs = new List<int>();
    private List<int> partsIDs = new List<int>();
    private List<int> othersIDs = new List<int>();

    //=
    [SerializeField] private GameObject markerExample;
    private GameObject marker;
    public Transform GetMarker => marker.transform;
    //=
    [SerializeField] private GameObject markerDestroyerExample;
    private GameObject markerDestroyer;
    public Transform GetMarkerDestroyer => markerDestroyer.transform;

    private int indexBuilder = 0;

    private void Awake()
    {
        initAssetsLink(floorsLocation);
        initAssetsLink(wallsLocation);
        initAssetsLink(roofsLocation);
        initAssetsLink(partsLocation);
        initAssetsLink(othersLocation);

        marker = Instantiate(markerExample);
        marker.SetActive(false);

        markerDestroyer = Instantiate(markerDestroyerExample);
        markerDestroyer.SetActive(false);

    }

    public int[] GetArrayOfFloorsIds => floorsIDs.ToArray();
    public int[] GetArrayOfWallsIds => wallsIDs.ToArray();
    public int[] GetArrayOfRoofsIds => roofsIDs.ToArray();
    public int[] GetArrayOfPartsIds => partsIDs.ToArray();
    public int[] GetArrayOfOthersIds => othersIDs.ToArray();

    public GameObject GetGameObjectByID(int ID)
    {
        GameObject g = Instantiate(linkIDtoAsset[ID], blockLocation);
        g.gameObject.name += "-" + indexBuilder.ToString();
        indexBuilder++;
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
                print("ERROR! more than one IDs! - "+ id);
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

                    case BlockTypes.roof:
                        roofsIDs.Add(id);
                        break;

                    case BlockTypes.stair:
                        partsIDs.Add(id);
                        break;

                    case BlockTypes.beam:
                        partsIDs.Add(id);
                        break;

                    case BlockTypes.fence:
                        partsIDs.Add(id);
                        break;

                    case BlockTypes.garden_ground:
                        othersIDs.Add(id);
                        break;

                    case BlockTypes.furniture:
                        othersIDs.Add(id);
                        break;
                }
            }
        }
    }
}
