using UnityEngine;
using System.Linq;
using YG;

public class SaveLoadManager
{
    
    private const string ID = "Player29";

    public static void Save()
    {        
        Globals.MainPlayerData.L = Globals.CurrentLanguage;
        Globals.MainPlayerData.M = Globals.IsMobile ? 1 : 0;
        Globals.MainPlayerData.Mus = Globals.IsMusicOn ? 1 : 0;
        Globals.MainPlayerData.S = Globals.IsSoundOn ? 1 : 0;

        string data = JsonUtility.ToJson(Globals.MainPlayerData);
        Debug.Log(data);
        PlayerPrefs.SetString(ID, data);
        YandexGame.savesData.MainSave1 = data;

        try
        {            
            YandexGame.SaveProgress();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError("error saving data, defaults loaded");            
        }        
    }


    public static void Load()
    {
        string fromSave = "";
        YandexGame.LoadProgress();

        try
        {
            fromSave = YandexGame.savesData.MainSave1;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError("error loading data, defaults loaded");

        }


        if (!string.IsNullOrEmpty(fromSave))
        {


            Debug.Log("loaded: " + fromSave);
            try
            {
                Globals.MainPlayerData = JsonUtility.FromJson<PlayerData>(fromSave);
            }
            catch (System.Exception)
            {
                Globals.MainPlayerData = new PlayerData();
            }

        }
        else
        {
            fromSave = PlayerPrefs.GetString(ID);

            if (string.IsNullOrEmpty(fromSave))
            {
                Globals.MainPlayerData = new PlayerData();
            }
            else
            {
                Globals.MainPlayerData = JsonUtility.FromJson<PlayerData>(fromSave);
            }
        }

    }

}
