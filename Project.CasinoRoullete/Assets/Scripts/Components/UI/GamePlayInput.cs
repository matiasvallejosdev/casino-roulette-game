using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;

namespace Components
{
    public class GamePlayInput : MonoBehaviour
    {
        public GameCmdFactory gameCmdFactory;
        
        public void OnClick()
        {
            gameCmdFactory.PlayTurn().Execute();
        }
    }
}
