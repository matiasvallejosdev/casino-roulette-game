using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using ViewModel;

namespace Components
{
    public class ChipSelectInput : MonoBehaviour
    {
        public GameCmdFactory gameCmdFactory;
        public CharacterTable characterTable;

        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("FichaUI"))
            {
                //SoundContoller.Instance.PlayFxSound(5);
                ChipSelected chipSelected = other.gameObject.GetComponent<ChipSelected>();
                gameCmdFactory.ChipSelect(characterTable, chipSelected.chipData).Execute();
            }
        }
    }
}
