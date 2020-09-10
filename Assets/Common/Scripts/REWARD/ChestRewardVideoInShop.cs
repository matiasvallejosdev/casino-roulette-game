using GameServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestRewardVideoInShop : MonoBehaviour
{
    public Text _txtEarnMoney;

    private int _section;
    public int[] _payment;

    Button button;

    private ulong lastRewardVideoOpen;
    private float sToWait { get; set; }

    public void Start()
    {
        button = GetComponent<Button>();

        ServiceManager.Instance.OnRewardVideoFinished.AddListener(HandleOnRewardVideoFinish);

        sToWait = PlayerPrefs.GetFloat("SecondsToWaitRewardVideo");
        lastRewardVideoOpen = ulong.Parse(PlayerPrefs.GetString("lastRewardVideoOpen"));

        if (!isRewardReady())
        {
            button.interactable = false;
        }
    }
    /// <summary>
    /// Handle of the video finish and reward if is true
    /// </summary>
    /// <param name="isFinish"></param>
    private void HandleOnRewardVideoFinish(bool isFinish)
    {
        Debug.Log("The payment has finished!");
        PlayerPrefs.SetString("lastRewardVideoOpen", DateTime.Now.Ticks.ToString());
        if (isFinish == true)
        {
            isWinReward();
        }
    }

    private void Update()
    {
        if (isRewardReady())
        {
            return;
        }
    }
    /// <summary>
    /// Depend if is in Menu. Execute the Payment and change the scene.
    /// </summary>
    private void isWinReward()
    {
        game_manager.Instance.toggleShop();
        if (game_manager.Instance.getIsInMenu())
        {
            Debug.Log("Is in shop and in menu! .. " + game_manager.Instance.getIsInMenu());

            // When state in shop in menu
            System.Random tmp = new System.Random();
            _section = tmp.Next(0, 2);

            MenuUi.Instance.rewardVideoUiFinished("Excelent!", _payment[_section].ToString(), true);
            Debug.Log("Reward the player!");
            Debug.Log("You win: $ " + _payment[_section]);

            MoneySystemController.Instance._cashNew = _payment[_section];
            MoneySystemController.Instance.SavePlayerCash();

            MenuUi.Instance.setMoneyUi();
        }
        else if (!game_manager.Instance.getIsInMenu())
        {
            Debug.Log("Is in shop and in game! .. " + game_manager.Instance.getIsInMenu());

            // When state in shop in menu
            System.Random tmp = new System.Random();
            _section = tmp.Next(0, 2);

            CanvasUI.Instance.turnWinOrLost("Excelent!", _payment[_section].ToString(), true, _payment[_section]);
            Debug.Log("Reward the player!");
            Debug.Log("You win in game: $ " + _payment[_section]);

            RoundController.Instance.OnRewardFinished(_payment[_section]);
            RoundController.Instance.ActivateButtons(true);
        }
    }
    /// <summary>
    /// Return true or false depend if the reward video is avaiable
    /// </summary>
    /// <returns></returns>
    private bool isRewardReady()
    {
        lastRewardVideoOpen = ulong.Parse(PlayerPrefs.GetString("lastRewardVideoOpen"));

        ulong diff = ((ulong)DateTime.Now.Ticks - lastRewardVideoOpen);
        ulong m = diff / TimeSpan.TicksPerSecond;

        float secondsLeft = (float)(sToWait - m);

        if (secondsLeft < 0)
        {
            _txtEarnMoney.text = "Earn Money!";
            button.interactable = true;
            return true;
        }
        else
        {
            button.interactable = false;
            calculateTimeToNextVideo();
            return false;
        }

    }
    /// <summary>
    /// Calculate the time to the next video is update statement
    /// </summary>
    private void calculateTimeToNextVideo()
    {
        // Set the timer
        ulong diff = ((ulong)DateTime.Now.Ticks - lastRewardVideoOpen);
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

        _txtEarnMoney.text = t;
    }
}
