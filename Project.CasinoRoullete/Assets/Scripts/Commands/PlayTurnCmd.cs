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
            PlayerSound.Instance.gameSound.OnSound.OnNext(5);

            if(characterTable.currentTable.Count <= 0)
                return;
            
            Debug.Log($"The game roullete is executing in {characterTable.tableName} with {characterTable.currentTableCounter} chips in table!");
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
        // Before to call OnRound you have to call ResetTable
        
        /* 
        IEnumerator StartRotationRoullete(int randomNumber)
        {
            sphere.SetActive(true);
            
            speed = 35f;
            
            FxFocusFoundedRoullete(true);
            _cameraAnimator.SetBool("Mover", true);
        
            FxCanvas(false);
            //fx_focus(true);

            yield return new WaitForSeconds(2.0f);
            speed = 75f;
            yield return new WaitForSeconds(1.0f);
            speed = 145f;
            SoundContoller.Instance.PlayFxSound(8);
            yield return new WaitForSeconds(0.5f);
            speed = 240f;
            yield return new WaitForSeconds(1.2f);
            speed = 245f;
            yield return new WaitForSeconds(2.0f);
            speed = 265;
            yield return new WaitForSeconds(3.8f);
            speed = 245;
            yield return new WaitForSeconds(1.5f);
            speed = 240f;
            sphere.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            speed = 145;

            // Ball position
            handlerBallScript.SetBallInWheel(randomNumber);
            yield return new WaitForSeconds(1.8f);
            FxFocusFoundedRoullete(false);
            speed = 75f;
            // Fx
            _scFxNewNumber.fxNewNumber(4,randomNumber);
            yield return new WaitForSeconds(5.0f);
            speed = 35f;
            _cameraAnimator.SetBool("Mover", false);
            FxCanvas(true);

            // Intialize the payment system and display the news values
            // Finished the rounded 

            // Active Buttons
            RoundController.Instance.ActivateButtons(true);
            PaymentController.Instance.roundFinished();
        }
        */
    }
}
