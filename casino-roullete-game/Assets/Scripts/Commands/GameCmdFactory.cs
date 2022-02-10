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
        public ButtonPushChipTableCmd ButtonTableTurn(CharacterTable characterTable, GameObject chipInstance, ButtonTable buttonData)
        {
            return new ButtonPushChipTableCmd(new TableManager(characterTable, new GameTable(), new TableInteractable()), chipInstance, buttonData);
        }    
        public ChipSelectCmd ChipSelect(CharacterTable characterTable, Chip arrayValue)
        {
            return new ChipSelectCmd(characterTable, arrayValue);
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
        public RewardSceneOpenCmd RewardTurn(string rewardScene)
        {
            return new RewardSceneOpenCmd(rewardScene);
        }      
        public FortunePaymentCmd FortuneTurn(CharacterCmdFactory characterCmdFactory, CharacterTable characterTable, RewardFortune rewardFortune, int finalPosition)
        {
            return new FortunePaymentCmd(characterCmdFactory, characterTable, rewardFortune, finalPosition);
        }      
        public FortunePlayCmd FortuneRewardTurn(RewardFortune rewardFortune)
        {
            return new FortunePlayCmd(rewardFortune);
        }      
    }
}
