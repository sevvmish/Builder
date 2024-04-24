using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class MakePurchase : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI errorButtonText;
    [SerializeField] private Button errorButton;
    [SerializeField] private GameObject errorPanel;


    private bool isReady;

    private void Start()
    {
        errorPanel.SetActive(false);

        errorButton.onClick.AddListener(() =>
        {
            SoundUI.Instance.PlayUISound(SoundsUI.click);
            errorPanel.SetActive(false);
        });
    }


    private void Update()
    {
        if (Globals.IsInitiated && !isReady)
        {
            isReady = true;
            YandexGame.PurchaseSuccessEvent = SuccessPurchased;
            YandexGame.PurchaseFailedEvent = FailedPurchased;
            YandexGame.ConsumePurchases();

            //errorText.text = Globals.Language.PurchaseError;
            //errorButtonText.text = Globals.Language.Close;

            if (Globals.IsAllRestarter)
            {
                Globals.IsAllRestarter = false;
                //GetRewardSystem.Instance.ShowEffect(RewardTypes.all_skins, 0);
                //GetRewardSystem.Instance.ShowEffect(RewardTypes.all_maps, 0);
                //GetRewardSystem.Instance.ShowEffect(RewardTypes.no_adv, 0);
            }
        }
        
    }

    public void Buy(string id)
    {
        YandexGame.BuyPayments(id);
    }

    private void SuccessPurchased(string id)
    {
        SoundUI.Instance.PlayUISound(SoundsUI.cash);

        switch(id)
        {
            case "no_adv":
                
                Globals.MainPlayerData.AdvOff = true;
                SaveLoadManager.Save();
                //Globals.IsDontShowIntro = true;
                //SceneManager.LoadScene("MainMenu");


                break;

            case "starter":
                
                
                break;

            case "all_maps":
                

                break;

            case "all_skins":
                
                //Globals.IsDontShowIntro = true;
                //SceneManager.LoadScene("MainMenu");

                //GetRewardSystem.Instance.ShowEffect(RewardTypes.all_skins, 0);
                

                break;

            case "get_all":


                

                //StartCoroutine(allRestarter());
                
                break;
        }

        YandexMetrica.Send(id);
    }

        

    private void FailedPurchased(string id)
    {
        print("FAILED TO BUY: " + id);
        StartCoroutine(playError());
    }
    private IEnumerator playError()
    {
        yield return new WaitForSeconds(0.3f);
        //SoundUI.Instance.PlayUISound(SoundsUI.error);
        errorPanel.SetActive(true);

        yield return new WaitForSeconds(5);
        errorPanel.SetActive(false);
    }

}
