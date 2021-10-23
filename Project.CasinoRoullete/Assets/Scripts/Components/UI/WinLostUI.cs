﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ViewModel;
using UniRx;
using System;

namespace Components
{
    public class WinLostUI : MonoBehaviour
    {
        public CharacterTable characterTable;
        public GameObject winLostContainer;
        public int secInScreen = 0;
        public Text tittleLabel = null;
        public Text valueLabel =  null;

        private int _delayInitialization = 3;

        void Start()
        {
            characterTable.characterMoney.currentPayment
                .Subscribe(OnRoundFinished)
                .AddTo(this);

            StartCoroutine(delayStart());
        }

        IEnumerator delayStart()
        {
            yield return new WaitForSeconds(_delayInitialization);  
        }

        private void OnRoundFinished(int payment)
        {
            if(payment == 0)
                return;

            WinLost(payment);
        }

        public void WinLost(int number)
        {
            bool isWin = number >= 0 ? true : false; 
            StartCoroutine(Winner(number, isWin));
        }

        IEnumerator Winner(int number, bool isWin)
        {
            tittleLabel.text = isWin ? "You win!" : "You lost!";
            valueLabel.text = number.ToString();

            winLostContainer.SetActive(true);

            if (isWin)
            {valueLabel.color = new Color(173f, 255f, 131f, 1f);}
            else
            {valueLabel.color = new Color(255f, 131f, 132f, 1f);}

            yield return new WaitForSeconds(secInScreen);

            characterTable.characterMoney.currentPayment.Value = 0;
            winLostContainer.SetActive(false);
        }
    }
}
