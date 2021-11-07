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
        public ButtonTurnCmd ButtonTableTurn(GameObject buttonInstance, GameObject chipInstance, GameObject chipsContainer, CharacterTable characterTable, ButtonTable buttonData, Chip chipData)
        {
            return new ButtonTurnCmd(buttonInstance, chipInstance, chipsContainer, characterTable, buttonData, chipData);
        }    
        public ChipSelectTurnCmd ChipSelect(CharacterTable characterTable, Chip arrayValue)
        {
            return new ChipSelectTurnCmd(characterTable, arrayValue);
        }      
        public PlayTurnCmd PlayTurn(CharacterTable characterTable, GameRoullete gameRoullete)
        {
            return new PlayTurnCmd(GameManager.Instance, characterTable, gameRoullete, new PlayRoundGateway());
        }

        // Table events
        public MusicTurnCmd MusicTurnCmd(GameSound gameSound, bool isOn, float value = 0)
        {
            return new MusicTurnCmd(gameSound, isOn, value);
        }    
        public ResetTurnCmd ResetTableTurn(MagnetDestroyerDisplay magnetDestroyerDisplay, CharacterTable characterTable, int delayTime = 0)
        {
            return new ResetTurnCmd(magnetDestroyerDisplay, characterTable, delayTime);
        }      
        public UndoTurnCmd UndoTableTurn()
        {
            return new UndoTurnCmd();
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
        public FortuneTurnCmd FortuneTurn(RewardFortune rewardFortune, int finalPosition)
        {
            return new FortuneTurnCmd(rewardFortune, finalPosition);
        }      
        public FortuneRewardTurnCmd FortuneRewardTurn(RewardFortune rewardFortune)
        {
            return new FortuneRewardTurnCmd(rewardFortune);
        }      
    }
}
