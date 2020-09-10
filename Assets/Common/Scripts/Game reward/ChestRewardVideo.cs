using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using Random = System.Random;

namespace GameServices 
{
    public class ChestRewardVideo : MonoBehaviour
    {
        public Text _txtEarnMoney;

        private int _section;
        public int[] _payment;

        [SerializeField] private bool isInMenu = false;
        [SerializeField] private bool isInGame = false;

        UnityEngine.UI.Button button;

        private ulong lastRewardVideoOpen;
        private float sToWait { get; set; }

        public void Start()
        {
            button = GetComponent<UnityEngine.UI.Button>();

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
            if (isInMenu && !isInGame) 
            {
                // When state in menu
                Random tmp = new Random();
                _section = tmp.Next(0, 2);

                MenuUi.Instance.OnRewardFinishedUI("Excelent!", _payment[_section].ToString(), true);
                Debug.Log("Reward the player!");
                Debug.Log("You win: $ " + _payment[_section]);

                MoneySystemController.Instance._cashNew = _payment[_section];
                MoneySystemController.Instance.SavePlayerCash();

                MenuUi.Instance.SetMoneyUi();
            } 
            else if(!isInMenu && !isInGame)
            {
                // When state in roullete fortune
                Random tmp = new Random();
                _section = tmp.Next(0, 2);

                Ui.Instance.turnWinOrLost("Excelent!", _payment[_section].ToString(), true, _payment[_section]);
                Debug.Log("Reward the player!");
                Debug.Log("You win in game: $ " + _payment[_section]);

                MoneySystemController.Instance._cashNew = _payment[_section];
                MoneySystemController.Instance.SavePlayerCash();
            } 
            else if(!isInMenu && isInGame) 
            {
                // When state in game
                Random tmp = new Random();
                _section = tmp.Next(0, 2);

                CanvasUI.Instance.turnWinOrLost("Excelent!", _payment[_section].ToString(), true, _payment[_section]);
                Debug.Log("Reward the player!");
                Debug.Log("You win in game: $ " + _payment[_section]);

                RoundController.Instance.OnRewardFinished(_payment[_section]);
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
}
