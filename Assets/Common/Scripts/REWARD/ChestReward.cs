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

    public Button _rewardVideoButton;
    public Text rewardTimer;

    private Button rewardButton;
    private ulong lastChestOpen;

    public Fortune _fortuneSc;

    private bool isFortune = true;

    void Start()
    {
        PlayerPrefs.SetFloat("SecondsToWaitReward", 120);
        PlayerPrefs.SetFloat("SecondsToWaitRewardVideo", 60);

        sToWait = PlayerPrefs.GetFloat("SecondsToWaitReward");

        rewardButton = GetComponent<Button>();
        lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

        if (!isRewardReady())
        {
            isFortune = false;
            StartCoroutine(wait(0.01f, true, true));
        }
    }
    void Update()
    {
        if (!isFortune)
        {
            if (isRewardReady())
            {
                StartCoroutine(wait(1, false, true));
                return;
            } 
        }
    }
    /// <summary>
    /// Click button on reward fortune
    /// </summary>
    public void rewardClick()
    {
        if (isFortune) 
        {
            StartCoroutine(wait(2, true, false));
            PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
            _fortuneSc.StartingFortune();
        } 
    }
    /// <summary>
    /// Return true or false if is reward ready
    /// </summary>
    /// <returns></returns>
    private bool isRewardReady()
    {
        lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastRewardOpen"));

        ulong diff = ((ulong)DateTime.Now.Ticks - lastChestOpen);
        ulong m = diff / TimeSpan.TicksPerSecond;

        float secondsLeft = (float)(sToWait - m);

        //Debug.Log(secondsLeft);

        if (secondsLeft < 0)
        {
            rewardTimer.text = "Let's Go!";
            rewardButton.interactable = true;
            return true;
        }
        else
        {
            calculateTimeToNextFortune();
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

    /// <summary>
    /// Interactive Reward Buttons
    /// </summary>
    /// <param name="isVideo"></param>
    private void rewardVideo(bool isVideo, bool isInteracteble) 
    {
        if (isVideo) 
        {
            _rewardVideoButton.gameObject.SetActive(true);
            if (isInteracteble) 
            {
                _rewardVideoButton.interactable = true;
            } 
            else { _rewardVideoButton.interactable = false; }

            rewardButton.gameObject.SetActive(false);

            isFortune = false;
        }
        else 
        {
            _rewardVideoButton.gameObject.SetActive(false);

            rewardButton.gameObject.SetActive(true);
            if (isInteracteble)
            {
                rewardButton.interactable = true;
            }
            else{rewardButton.interactable = false;}

            isFortune = true;
        }
    }
    
    /// <summary>
    /// Wait the seconds to execute method 'rewardVideo' for check if is active or not the fortune.
    /// </summary>
    /// <param name="seg"></param>
    /// <param name="isVideo"></param>
    /// <returns></returns>
    IEnumerator wait(float seg, bool isVideo, bool isInteracteble)
    {
        yield return new WaitForSeconds(seg);
        rewardVideo(isVideo, isInteracteble);
    }
}
