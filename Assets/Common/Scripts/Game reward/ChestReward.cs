using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.SceneManagement;
using System.IO;

public class ChestReward : MonoBehaviour
{
    private float sToWait { get; set; }
    private ulong lastChestOpen;

    public UnityEngine.UI.Button rewardVideoButton;
    private UnityEngine.UI.Button rewardButton;
    public Text rewardTimer;

    public Fortune _fortuneSc;

    void Start()
    {
        sToWait = PlayerPrefs.GetFloat("SecondsToWaitReward");
        lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

        rewardButton = GetComponent<UnityEngine.UI.Button>();

        if (!isRewardReady())
        {
            rewardVideoButton.interactable = true;
            rewardVideoButton.gameObject.SetActive(true);
            rewardButton.interactable = false;
        }
    }
    void Update()
    {
        if (!rewardButton.interactable)
        {
            if (isRewardReady())
            {
                rewardVideoButton.interactable = false;
                rewardVideoButton.gameObject.SetActive(false);
                rewardButton.interactable = true;
                return;
            }
            calculateTimeToNextFortune();
        }
    }
    /// <summary>
    /// Click button on reward fortune
    /// </summary>
    public void rewardClick()
    {
        rewardVideoButton.gameObject.SetActive(true);
        rewardVideoButton.interactable = true;
        rewardButton.interactable = false;
        PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
        _fortuneSc.StartingFortune();
    }
    /// <summary>
    /// Return true or false if is reward ready
    /// </summary>
    /// <returns></returns>
    private bool isRewardReady()
    {
        //lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

        ulong diff = ((ulong)DateTime.Now.Ticks - lastChestOpen);
        ulong m = diff / TimeSpan.TicksPerSecond;

        float secondsLeft = (float)(sToWait - m);

        if (secondsLeft < 0)
        {
            rewardTimer.text = "Let's Go!";
            return true;
        }
        else
        {
            return false;
        }
    }

    private void calculateTimeToNextFortune()
    {
        // Set the timer
        ulong diff = ((ulong)DateTime.Now.Ticks - lastChestOpen);
        ulong m = diff / TimeSpan.TicksPerSecond;

        float secondsLeft = (float)(sToWait - m);

        string t = "";
        // Hours
        t += ((int)secondsLeft / 3600).ToString() + "h:";
        secondsLeft -= ((int)secondsLeft / 3600) * 3600;
        // Minutes
        t += ((int)secondsLeft / 60).ToString("00") + "m:";
        // Seconds
        t += (secondsLeft % 60).ToString("00") + "s";

        rewardTimer.text = t;
    }

}
