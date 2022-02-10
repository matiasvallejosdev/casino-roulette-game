using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;
using Components;

namespace Commands
{    
    public class ResetTurnCmd : ICommand
    {
        private MagnetDestroyerDisplay magnetDestroyerDisplay;
        private float magnetTime;
        private CharacterTable characterTable;
        private int delayTime;

        public ResetTurnCmd(MagnetDestroyerDisplay magnetDestroyerDisplay, CharacterTable characterTable, int delayTime)
        {
            this.magnetDestroyerDisplay = magnetDestroyerDisplay;
            this.characterTable = characterTable;
            this.delayTime = delayTime;
        }

        public void Execute()
        {
            // Delete chips of the table
            // Using MagnetDestroyerAnimation
            
            PlayerSound.Instance.gameSound.OnSound.OnNext(PlayerSound.Instance.gameSound.audioReferences[5]);

            if(characterTable.currentTableCount <= 0)
                return;
                
            Debug.Log(@"Destroying chips of the table!");
            magnetDestroyerDisplay.StartCoroutine(ResetRoundProcess(magnetDestroyerDisplay.magnetTime));
        }
         
        IEnumerator ResetRoundProcess(float seg)
        {
            characterTable.currentTableActive.Value = false;
            yield return new WaitForSeconds(delayTime);
            magnetDestroyerDisplay.magnetDestroyer.SetActive(true);
            yield return new WaitForSeconds(seg);
            magnetDestroyerDisplay.magnetDestroyer.SetActive(false);
            characterTable.currentTableActive.Value = true;
        }
    }
}
