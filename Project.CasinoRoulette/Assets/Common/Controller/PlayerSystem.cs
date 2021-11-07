using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Commands;
using UnityEngine;
using ViewModel;
using UniRx;
using Managers;

namespace Controllers
{
    public class PlayerSystem : Singlenton<PlayerSystem>
    {
        // Player save system
        // Save game
        // Load game

        public CharacterTable characterTable;
        public CharacterCmdFactory characterCmdFactory;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
    
            characterTable.characterMoney.currentPayment.Value = 0;
            
            characterTable.OnSaveGame
                .Subscribe(SaveRound)
                .AddTo(this);
        }

        public void SaveRound(bool value) 
        {
            if(!value)
                return;

            characterCmdFactory.SavePlayer(characterTable).Execute();
        }
        public void LoadRound()
        {   
            characterCmdFactory.LoadPlayer(characterTable).Execute();
        }
    }
}
