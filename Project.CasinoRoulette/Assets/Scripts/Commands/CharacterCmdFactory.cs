using System.Collections;
using System.Collections.Generic;
using Infrastructure;
using UnityEngine;
using ViewModel;

namespace Commands
{    
    [CreateAssetMenu(fileName = "New CharacterCmdFactory", menuName = "Factory/Character Command Factory")]
    public class CharacterCmdFactory : ScriptableObject
    {  
        // Player system      
        public LoadTurnCmd LoadPlayer(CharacterTable characterTable)
        {
            return new LoadTurnCmd(characterTable, new SaveRoundGateway());
        }        
        public SaveTurnCmd SavePlayer(CharacterTable characterTable)
        {
            return new SaveTurnCmd(characterTable, new SaveRoundGateway());
        }        
        public SaveCashTurnCmd SaveCash(CharacterCmdFactory characterCmdFactory, CharacterTable characterTable, int payment)
        {
            return new SaveCashTurnCmd(characterCmdFactory, characterTable, payment, new SaveRoundGateway());
        }        
    }
}
