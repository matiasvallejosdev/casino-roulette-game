using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using ViewModel;

namespace Components
{
    public class GameMusicInput : MonoBehaviour
    {
        public GameSound gameSound;
        public GameCmdFactory gameCmdFactory;
        
        public void OnClick()
        {
            gameCmdFactory.MusicTurnCmd(gameSound, !gameSound.isMusicOn.Value).Execute();
        }
    }
}
