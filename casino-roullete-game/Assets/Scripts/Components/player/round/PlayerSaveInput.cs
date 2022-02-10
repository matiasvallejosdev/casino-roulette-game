using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Commands;
using UnityEngine;
using ViewModel;
using UniRx;
using Managers;

namespace Components
{
    public class PlayerSaveInput : MonoBehaviour
    {
        // Player save system
        // Save game
        // Load game

        public CharacterTable characterTable;
        public CharacterCmdFactory characterCmdFactory;

        private void Start()
        {
            characterTable.OnSaveGame
                .Subscribe(SaveRound)
                .AddTo(this);
            
            characterTable.OnLoadGame
                .Subscribe(LoadRound)
                .AddTo(this);
        }
        public void SaveRound(bool value) 
        {
            if(!value)
                return;

            characterCmdFactory.SavePlayer(characterTable).Execute();
        }
        public void LoadRound(bool value)
        {   
            characterCmdFactory.LoadPlayer(characterTable).Execute();
        }
    }
}
