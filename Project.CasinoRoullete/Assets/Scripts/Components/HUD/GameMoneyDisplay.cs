using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UniRx;
using ViewModel;
using System;

namespace Components
{
    public class GameMoneyDisplay : MonoBehaviour
    {
        public Text moneyLabel;
        public Text betLabel;

        public CharacterTable characterTable;


        // Start is called before the first frame update
        void Start()
        {
            characterTable.characterMoney.characterBet
                .Subscribe(OnChangeBet)
                .AddTo(this);

            characterTable.characterMoney.characterMoney
                .Subscribe(OnChangeMoney)
                .AddTo(this);

            //RoundController.Instance.OnRoundChanged.AddListener(HandleRoundStateChanged);
            //RoundController.Instance.OnApuestaChanged.AddListener(HandleApuestaStateChanged);
        }

        private void OnChangeBet(int value)
        {
            betLabel.text = value.ToString();
        }

        private void OnChangeMoney(int value)
        {
            moneyLabel.text = value.ToString();
        }
    }
}
