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
        public RewardFortune rewardFortune;
        public GameCmdFactory gameCmdFactory;
        public char separatorAnchor;

        private bool isExecute = false;
        Int32 count = 1;
    
        void OnTriggerEnter(Collider collider)
        {
            if(!rewardFortune.isPlay && rewardFortune.hasInitialize)
            {
                if(collider.CompareTag("AnchorSelectUI"))
                {
                    int pos = Convert.ToInt32(collider.name.Split(separatorAnchor)[1]);
                    FortuneWin(pos);
                }
            }
        }
        void FortuneWin(int pos)
        {
            gameCmdFactory.FortuneTurn(rewardFortune, pos).Execute();
        }
    }
}
