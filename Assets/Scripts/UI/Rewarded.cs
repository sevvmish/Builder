using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class Rewarded : MonoBehaviour
{
    public Action OnRewardedEndedOK;
    public Action OnError;

    private bool isRewardedOK;

    private void Start()
    {
        YandexGame.OpenVideoEvent = rewardStarted;
        YandexGame.RewardVideoEvent = rewardedClosedOK;
        YandexGame.CloseVideoEvent = advRewardedClosed;
        YandexGame.ErrorVideoEvent = advRewardedError;
    }

    public void ShowRewardedVideo()
    {
        isRewardedOK = false;        
        YandexGame.RewVideoShow(155);

    }

    private void rewardStarted()
    {
        print("reward started OK");
        Time.timeScale = 0;
        if (Globals.IsSoundOn)
        {
            AudioListener.volume = 0;
        }
    }

    private void rewardedClosedOK(int value)
    {
        //155
        if (value == 155)
        {
            isRewardedOK = true;
            print("reward closed OK 155");
            OnRewardedEndedOK?.Invoke();
        }

        //OnRewardedEndedOK = null;
        //OnError = null;

        Globals.TimeWhenLastRewardedWas = DateTime.Now;
    }

    private void advRewardedClosed()
    {
        print("rewarded was closed ok");
        Time.timeScale = 1;
        if (Globals.IsSoundOn)
        {
            AudioListener.volume = 1;
        }


        if (isRewardedOK)
        {
            print("invoke when rewarded OK");
            //OnRewardedEndedOK?.Invoke();
        }
        else
        {
            print("invoke when rewarded ERROR");
            //OnError?.Invoke();
        }

        //OnRewardedEndedOK = null;
        //OnError = null;

    }

    private void advRewardedError()
    {
        print("rewarded was ERROR!");
        Time.timeScale = 1;
        if (Globals.IsSoundOn)
        {
            AudioListener.volume = 1;
        }

        OnError?.Invoke();
        OnRewardedEndedOK = null;
        OnError = null;

    }
}
