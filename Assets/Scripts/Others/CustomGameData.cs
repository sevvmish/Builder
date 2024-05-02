using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomGameData : MonoBehaviour
{
    [SerializeField] private Transform blocksLocation;

    [SerializeField] private TextMeshProUGUI chooseMapText;
    [SerializeField] private GameObject chooseMapPanel;

    [SerializeField] private Button mapForest;
    [SerializeField] private TextMeshProUGUI forestText;

    [SerializeField] private Button mapRiver;
    [SerializeField] private TextMeshProUGUI riverText;

    private float[] currentData;
    private GameManager gm;
    private AssetManager assets;

    private float cooldown = 10f;
    private float _timer;

    private void Start()
    {
        gm = GameManager.Instance;
        assets = gm.Assets;

        chooseMapPanel.SetActive(false);



        if (Globals.IsWalkthroughEnabled)
        {
            enabled = false;
            return;
        }

        print(Globals.IsWalkthroughEnabled + " !!!!!!!!!!!!!!!!!!! ");

        if (Globals.MainPlayerData.CustomGameBlocks.Length > 0)
        {
            LoadData();
        }
        else
        {
            chooseMap();
        }

        
        mapForest.onClick.AddListener(() => 
        {
            assets.SetLevel(Maps.forest);
            chooseMapPanel.SetActive(false);
            if (!Globals.IsMobile)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Globals.IsOptions = false;

        });

        mapRiver.onClick.AddListener(() =>
        {
            assets.SetLevel(Maps.river);
            chooseMapPanel.SetActive(false);
            if (!Globals.IsMobile)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Globals.IsOptions = false;

        });
    }

    public void LoadData()
    {
        if (Globals.MainPlayerData.CustomGameBlocks.Length == 0)
        {
            return;
        }

        currentData = Globals.MainPlayerData.CustomGameBlocks;
        assets.SetLevel((Maps)currentData[0]);

        if (Globals.MainPlayerData.CustomGameBlocks.Length == 1)
        {
            return;
        }

        int arrLength = Globals.MainPlayerData.CustomGameBlocks.Length;

        int amount = (arrLength - 1);

        for (int i = 1; i < amount; i+=5)
        {
            GameObject b = assets.GetGameObjectByID((int)currentData[i]);
            b.transform.position = new Vector3(currentData[i + 1], currentData[i + 2], currentData[i + 3]);
            b.transform.eulerAngles = new Vector3(0, currentData[i + 4], 0);
        }
    }

    public void SaveData()
    {
        if (blocksLocation.transform.childCount == 0) return;

        List<float> newData = new List<float>();

        newData.Add((float)assets.CurrentMap);

        for (int i = 0; i < blocksLocation.transform.childCount; i++)
        {
            if (blocksLocation.transform.GetChild(i).gameObject.activeSelf && blocksLocation.transform.GetChild(i).TryGetComponent(out Block b) && b.IsFinalized)
            {
                newData.Add(b.ID.ID);
                newData.Add(b.transform.position.x);
                newData.Add(b.transform.position.y);
                newData.Add(b.transform.position.z);
                newData.Add(b.transform.eulerAngles.y);
            }
        }

        currentData = newData.ToArray();

        if (Globals.MainPlayerData.CustomGameBlocks.Length != currentData.Length)
        {
            Globals.MainPlayerData.CustomGameBlocks = currentData;
            SaveLoadManager.Save();
            print("game saved");
        }
    }

    private void Update()
    {
        if (_timer > cooldown)
        {
            _timer = 0;
            SaveData();
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }

    private void chooseMap()
    {
        

        if (!Globals.IsMobile)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        chooseMapPanel.SetActive(true);
        chooseMapText.text = Globals.Language.ChooseMapText;
        forestText.text = Globals.Language.ForestText;
        riverText.text = Globals.Language.RiverText;
    }
}
