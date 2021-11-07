using System.Collections;
using System.Collections.Generic;
using Commands;
using Components;
using UnityEngine;
using ViewModel;

namespace Components
{
    public class GameRestoreInput : MonoBehaviour
    {
        public CharacterTable characterTable;
        public GameCmdFactory gameCmdFactory;
        public void OnClick() 
        {
            gameCmdFactory.RestoreTableTurn(characterTable).Execute();
        }
    }
}
