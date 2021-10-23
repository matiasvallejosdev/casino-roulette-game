using System.Collections;
using System.Collections.Generic;
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
        public ButtonTurnCmd ButtonTurnCmd(GameObject chipInstance, GameObject chipsContainer, CharacterTable characterTable, ButtonTable buttonData)
        {
            return new ButtonTurnCmd(chipInstance, chipsContainer, characterTable, buttonData);
        }    
    }
}
