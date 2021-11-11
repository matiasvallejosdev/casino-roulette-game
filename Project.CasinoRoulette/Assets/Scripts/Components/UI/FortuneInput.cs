using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViewModel;
using UniRx;
using Commands;
using System;

namespace Components
{
    public class FortuneInput : MonoBehaviour
    {
        public CharacterCmdFactory characterCmdFactory;
        public RewardFortune rewardFortune;
        public GameCmdFactory gameCmdFactory;
        public CharacterTable characterTable;
        public char separatorAnchor;

        private bool isExecute = false;
        Int32 count = 1;
    
        void OnTriggerStay(Collider collider)
        {
            if(!isExecute && !rewardFortune.isPlay && rewardFortune.isPayment)
            {
                if(collider.CompareTag("AnchorSelectUI"))
                {
                    isExecute = true;
                    int pos = Convert.ToInt32(collider.name.Split(separatorAnchor)[1]);
                    FortuneWin(pos);
                }
            }
        }
        void FortuneWin(int pos)
        {
            gameCmdFactory.FortuneTurn(characterCmdFactory, characterTable, rewardFortune, pos).Execute();
        }
    }
}
