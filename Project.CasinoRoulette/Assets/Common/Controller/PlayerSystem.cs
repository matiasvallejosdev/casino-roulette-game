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
using System.Threading.Tasks;

namespace Controllers
{
    public class PlayerSystem : Singlenton<PlayerSystem>
    {
        // Player save system
        // Save game
        // Load game

        public CharacterTable characterTable;
        public RewardFortune rewardFortune;
        public CharacterCmdFactory characterCmdFactory;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
    
            characterTable.characterMoney.currentPayment.Value = 0;
            
            characterTable.OnSaveGame
                .Subscribe(SaveRound)
                .AddTo(this);
        }

        public async Task CreatePlayer(string tableName, string playerPath)
        {
            characterTable.tableName = tableName;
            characterTable.characterMoney.characterMoney.Value = 10000;
            
            if(!File.Exists(playerPath))
            {
                PlayerRound.Instance.characterTable.OnSaveGame.OnNext(true);
                PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
                PlayerPrefs.SetFloat("SecondsToWaitReward", 120);
                //Debug.Log(long.Parse(PlayerPrefs.GetString("LastRewardOpen")));
            }

            await Task.Run(() => File.Exists(playerPath)); 
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
