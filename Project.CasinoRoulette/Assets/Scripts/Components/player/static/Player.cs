using System;
using System.IO;
using UnityEngine;
using ViewModel;
using System.Threading.Tasks;

namespace Components
{
    public static class Player
    {
        public static async Task CreatePlayer(CharacterTable characterTable, string tableName, string playerPath)
        {
            characterTable.tableName = tableName;
            characterTable.characterMoney.characterMoney.Value = 10000;
            
            if(!File.Exists(playerPath))
            {
                characterTable.OnSaveGame.OnNext(true);
                PlayerPrefs.SetString("LastRewardOpen", DateTime.Now.Ticks.ToString());
                PlayerPrefs.SetFloat("SecondsToWaitReward", 120);
            }

            await Task.Run(() => File.Exists(playerPath)); 
        }
    }
}
