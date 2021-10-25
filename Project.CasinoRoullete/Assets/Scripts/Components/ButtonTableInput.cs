using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using ViewModel;
using UniRx;
using System;

namespace Commands
{
    public class ButtonTableInput : MonoBehaviour
    {
        public GameCmdFactory gameCmdFactory;
        public CharacterTable characterTable;
        public ButtonTable buttonTableData;
        public GameObject chipsContainer;
        public GameObject centerPivot;

        private Boolean _isActive;

        private float startTime, endTime;
        public bool isLongPressed {get; private set; }
        bool _isPressed; 

        
        public bool HasFichasOnTop
        {
            get
            {
                bool _fichasTopBoolean = false;
                if (buttonTableData.currentChipsOnTop != 0)
                {
                    _fichasTopBoolean = true;
                }   
                return _fichasTopBoolean;
            }
            private set{ }
        }

        void Start()
        {
            ResetButton();

            characterTable.OnActiveButton
                .Subscribe(OnActiveButton)
                .AddTo(this);
        }

        void ResetButton()
        {
            buttonTableData.currentSpritePivot = centerPivot.transform.position;
            buttonTableData.currentChipsOnTop = 0;
            buttonTableData.currentOffset = new Vector2(0, 0);
        
            startTime = 0;
            endTime = 0;
            _isPressed = false;
        }

        private void OnActiveButton(bool isActive)
        {
            _isActive = isActive;
            if(isActive)
            {
                ResetButton();
            } 
        }

        public void Click()
        {
            if(!_isActive)
                return;
                
            GameObject newChip = Instantiate(characterTable.chipPrefab);
            newChip.SetActive(false);
            gameCmdFactory.ButtonTableTurn(newChip, chipsContainer, characterTable, buttonTableData).Execute();
        }
    }
}
