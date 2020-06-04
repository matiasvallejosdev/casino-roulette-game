using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Reward : MonoBehaviour
{
    public float msToWait = 5000;

    private Button rewardButton;
    private ulong lastChestOpen;

    [SerializeField] private Fortune _fortuneSc;

    void Start()
    {
        rewardButton = GetComponent<Button>();
        lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

        if (isRewardReady())
        {
            rewardButton.interactable = true;
        }
    }

    void Update()
    {
        if (!rewardButton.IsInteractable())
        {
            if (isRewardReady())
            {
                rewardButton.interactable = true;
            }
        }
    }

    public void rewardClick()
    {
        PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
        lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));
        rewardButton.interactable = false;

        _fortuneSc.StartingFortune();
    }

    private bool isRewardReady()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - lastChestOpen);
        ulong m = diff / TimeSpan.TicksPerMillisecond;

        float secondsLeft = (float)(msToWait - m) / 1000f;

        if (secondsLeft < 0)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
