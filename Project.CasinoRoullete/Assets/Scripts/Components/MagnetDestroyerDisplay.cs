using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;
using Commands;

namespace Components
{
    public class MagnetDestroyerDisplay : MonoBehaviour
    {
        public CharacterTable characterTable;
        public GameCmdFactory gameCmdFactory;
        public GameObject magnetDestroyer;
        public float magnetTime;

        void Start()
        {
            characterTable.OnRoundFinished
                .Subscribe(MagnetDestroyer)
                .AddTo(this);
        }

        public void MagnetDestroyer(bool value)
        {
            gameCmdFactory.ResetTableTurn(this, characterTable);  
        }
    }
}
