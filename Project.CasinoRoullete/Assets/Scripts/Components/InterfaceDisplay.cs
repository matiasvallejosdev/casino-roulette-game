using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;
using System;

namespace Components
{
    public class InterfaceDisplay : MonoBehaviour
    {    
        public CharacterTable characterTable;
        public GameObject canvasUi;
        public GameObject lastAnchor;
        public GameObject moneyAnchor;
        public GameObject shadowGame;

        void Start()
        {
            characterTable.OnRound
                .Subscribe(DisplayInterface)
                .AddTo(this);
            
            characterTable.characterMoney.currentPayment
                .Subscribe(OnPaymentDisplay)
                .AddTo(this);
        }

        private void OnPaymentDisplay(int value)
        {
            bool display = value > 0 ? false : true;
            canvasUi.SetActive(display);
            lastAnchor.SetActive(display);
            moneyAnchor.SetActive(display);
        }

        public void DisplayInterface(bool isRound)
        {
            canvasUi.SetActive(!isRound);
            lastAnchor.SetActive(!isRound);
            moneyAnchor.SetActive(!isRound);
            shadowGame.SetActive(isRound);
        }
    }
}
