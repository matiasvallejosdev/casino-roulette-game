using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using ViewModel;

namespace Components
{
    public class GameRewardInput : MonoBehaviour
    {
        public GameCmdFactory gameCmdFactory;
        
        public void OnClick(string scene)
        {
            gameCmdFactory.RewardTurn(scene).Execute();
        }
    }
}
