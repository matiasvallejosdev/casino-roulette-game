using System.Collections;
using System.Collections.Generic;
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
        
        public Button rewardButton;
        
        public void OnClick()
        {
            rewardButton.interactable = false;
            gameCmdFactory.FortuneRewardTurn(rewardFortune).Execute();
        }
    }
}
