using System.Collections;
using Infrastructure;
using UnityEngine;
using ViewModel;
using UniRx;
using Components;
using Managers;
using System.IO;
using System;
using System.Threading.Tasks;

namespace Components
{
    public class PlayerRoundInput : MonoBehaviour
    {
        public CharacterTable characterTable;
        public ITableController _tableController;

        void Awake()
        {
            _tableController = GetComponent<ITableController>();
        }

        void Start()
        {
            characterTable.OnDestroyChip
                .Subscribe(_tableController.DestroyChipFromTable)
                .AddTo(this);

            characterTable.OnDestroyLastChip
                .Subscribe(_tableController.LastChipDestroy)
                .AddTo(this);
            
            characterTable.OnRound
                .Subscribe(OnRoundFinish)
                .AddTo(this);
            
            characterTable.OnResetGame
                .Subscribe(_tableController.ResetTable)
                .AddTo(this);

            characterTable.OnRestoreTable
                .Subscribe(_tableController.RestoreTable)
                .AddTo(this);
        }

        public async void OnRoundFinish(bool isRound)
        {
            if(isRound)
                return;

            foreach(var item in characterTable.currentTableInGame)
            {
                characterTable.lastTable.Add(item);   
            }

            characterTable.currentNumbers.Add(characterTable.lastNumber);

            await Task.Delay(TimeSpan.FromSeconds(2));
            _tableController.ResetTable(false);
        }
    }
}
