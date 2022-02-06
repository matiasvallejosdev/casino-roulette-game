using System.Collections;
using System.Collections.Generic;
using Components;
using Infrastructure;
using Managers;
using UnityEngine;
using ViewModel;

namespace Commands
{    
    [CreateAssetMenu(fileName = "New GameCmdFactory", menuName = "Factory/Game Command Factory")]
    public class GameCmdFactory : ScriptableObject
    {   
        // Game roullete events      
        public ButtonTurnCmd ButtonTableTurn(CharacterTable characterTable, GameObject chipInstance, ButtonTable buttonData)
        {
            return new ButtonTurnCmd(characterTable, chipInstance, buttonData);
        }    
        public ChipSelectTurnCmd ChipSelect(CharacterTable characterTable, Chip arrayValue)
        {
            return new ChipSelectTurnCmd(characterTable, arrayValue);
        }      
        public PlayTurnCmd PlayTurn(CharacterTable characterTable, GameRoullete gameRoullete)
        {
            return new PlayTurnCmd(GameManager.Instance, characterTable, gameRoullete, new PlayRoundGateway(), new PaymnentGateway());
        }

        // Table events
        public MusicTurnCmd MusicTurnCmd(GameSound gameSound, bool isOn)
        {
            return new MusicTurnCmd(gameSound, isOn);
        }    
        public ResetTurnCmd ResetTableTurn(MagnetDestroyerDisplay magnetDestroyerDisplay, CharacterTable characterTable, int delayTime = 0)
        {
            return new ResetTurnCmd(magnetDestroyerDisplay, characterTable, delayTime);
        }      
        public UndoTurnCmd UndoTableTurn(CharacterTable characterTable)
        {
            return new UndoTurnCmd(characterTable);
        }      
        public RestoreTurnCmd RestoreTableTurn(CharacterTable characterTable)
        {
            return new RestoreTurnCmd(characterTable);
        } 

        // Reward 
        public RewardTurnCmd RewardTurn(string rewardScene)
        {
            return new RewardTurnCmd(rewardScene);
        }      
        public FortunePaymentTurnCmd FortuneTurn(CharacterCmdFactory characterCmdFactory, CharacterTable characterTable, RewardFortune rewardFortune, int finalPosition)
        {
            return new FortunePaymentTurnCmd(characterCmdFactory, characterTable, rewardFortune, finalPosition);
        }      
        public FortuneRewardTurnCmd FortuneRewardTurn(RewardFortune rewardFortune)
        {
            return new FortuneRewardTurnCmd(rewardFortune);
        }      
    }
}
