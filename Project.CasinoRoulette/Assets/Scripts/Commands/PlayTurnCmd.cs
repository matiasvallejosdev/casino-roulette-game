using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using ViewModel;
using Controllers;
using Infrastructure;
using System;

namespace Commands
{    
    public class PlayTurnCmd : ICommand
    {
        private readonly MonoBehaviour monoBehaviour;
        private CharacterTable characterTable;
        private GameRoullete gameRoullete;
        private IRound roundGateway;

        public PlayTurnCmd(MonoBehaviour monoBehaviour, CharacterTable characterTable, GameRoullete gameRoullete, IRound roundGateway)
        {
            this.monoBehaviour = monoBehaviour;
            this.characterTable = characterTable;
            this.gameRoullete = gameRoullete;
            this.roundGateway = roundGateway;
        }

        public void Execute()
        {
            if(characterTable.currentTableCount <= 0)
                return;
            
            Debug.Log($"The game roullete is executing in {characterTable.tableName} with {characterTable.currentTableCount} chips in table!");
            PlayerSound.Instance.gameSound.OnSound.OnNext(6);

            roundGateway.PlayTurn()
                .Do(_ => monoBehaviour.StartCoroutine(RoulleteGame(roundGateway.randomNumber)))
                .Do(_ => PlayerRound.Instance._lastNumber = roundGateway.randomNumber)
                .Subscribe();
                
        }
        IEnumerator RoulleteGame(int num)
        {
            characterTable.OnRound.OnNext(true); // Initialize round
            characterTable.OnActiveButton.OnNext(false); // Desactivete table buttons
            gameRoullete.OnRotate.OnNext(true);

            yield return new WaitForSeconds(2.0f);
            gameRoullete.currentSpeed = 75f;
            yield return new WaitForSeconds(1.0f);
            gameRoullete.currentSpeed = 145f;
            PlayerSound.Instance.gameSound.OnSound.OnNext(8);
            yield return new WaitForSeconds(0.5f);
            gameRoullete.currentSpeed = 240f;
            yield return new WaitForSeconds(1.2f);
            gameRoullete.currentSpeed = 245f;
            yield return new WaitForSeconds(2.0f);
            gameRoullete.currentSpeed = 265;
            yield return new WaitForSeconds(3.8f);
            gameRoullete.currentSpeed = 245;
            yield return new WaitForSeconds(1.5f);
            gameRoullete.currentSpeed = 240f;
            yield return new WaitForSeconds(1.5f);
            // Ball position
            gameRoullete.currentSpeed = 145;
            gameRoullete.OnNumber.OnNext(num);

            yield return new WaitForSeconds(1.8f);
            gameRoullete.currentSpeed = 75f;
   
            yield return new WaitForSeconds(5.0f);
            // Finish round
            gameRoullete.currentSpeed = gameRoullete.defaultSpeed;
            characterTable.OnRound.OnNext(false); 

            // Intialize the payment system and display the news values
            PlayerPayment.Instance.PaymentSystem(characterTable)
                .Delay(TimeSpan.FromSeconds(3))
                .Do(_ => PlayerRound.Instance.OnPayment(PlayerPayment.Instance.PaymentValue))
                .Do(_ => characterTable.OnWinButton.OnNext(num))
                .Do(_ => PlayerSound.Instance.gameSound.OnSound.OnNext(4))
                .Subscribe();
        }
    }
}
