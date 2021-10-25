using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using ViewModel;

namespace Commands
{    
    [CreateAssetMenu(fileName = "New GameCmdFactory", menuName = "Factory/Game Command Factory")]
    public class GameCmdFactory : ScriptableObject
    {   
        // Game roullete
        public ChipSelectTurnCmd ChipSelect(CharacterTable characterTable, Chip arrayValue)
        {
            return new ChipSelectTurnCmd(characterTable, arrayValue);
        }      
        public PlayTurnCmd PlayTurn()
        {
            return new PlayTurnCmd();
        }      
        public ResetTurnCmd ResetTableTurn(MagnetDestroyerDisplay magnetDestroyerDisplay, CharacterTable characterTable)
        {
            return new ResetTurnCmd(magnetDestroyerDisplay, characterTable);
        }      
        public UndoTurnCmd UndoTableTurn()
        {
            return new UndoTurnCmd();
        }      
        public ButtonTurnCmd ButtonTableTurn(GameObject chipInstance, GameObject chipsContainer, CharacterTable characterTable, ButtonTable buttonData)
        {
            return new ButtonTurnCmd(chipInstance, chipsContainer, characterTable, buttonData);
        }    
    }
}
