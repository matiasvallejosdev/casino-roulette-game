using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using ViewModel;
using UniRx;
using System;

namespace Components
{
    public class ChipSelectInput : MonoBehaviour
    {
        public GameCmdFactory gameCmdFactory;
        public CharacterTable characterTable;
        private bool _selectorAnchor;

        void Start()
        {
            characterTable.currentTableActive
                .Subscribe(IsTableActive)
                .AddTo(this);
        }

        private void IsTableActive(bool isActive)
        {
            _selectorAnchor = isActive;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("ChipSelectUI") && characterTable.currentTableActive.Value && _selectorAnchor)
            {
                ChipSelected chipSelected = other.gameObject.GetComponent<ChipSelected>();
                gameCmdFactory.ChipSelect(characterTable, chipSelected.chipData).Execute();
            }
        }
    }
}
