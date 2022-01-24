using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Commands;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ViewModel;

namespace Components
{
    public class FortunePlayInput : MonoBehaviour
    {
        public RewardFortune rewardFortune;
        public GameCmdFactory gameCmdFactory;

        void Start()
        {
            OnClick();
        }
        
        public async void OnClick()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            gameCmdFactory.FortuneRewardTurn(rewardFortune).Execute();
        }
    }
}
