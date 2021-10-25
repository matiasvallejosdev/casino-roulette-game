using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using ViewModel;

namespace Components
{
    public class GameDeleteInput : MonoBehaviour
    {
        public CharacterTable characterTable;
        public GameCmdFactory gameCmdFactory;
        public MagnetDestroyerDisplay magnetDestroyerDisplay;

        public void OnClick()
        {
            gameCmdFactory.ResetTableTurn(magnetDestroyerDisplay, characterTable).Execute();
        }
    }
}
