using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using ViewModel;
using UniRx;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Commands
{
    [RequireComponent (typeof(Button))]
    public class ButtonTableInput : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
    {
        public GameCmdFactory gameCmdFactory;
        public CharacterTable characterTable;
        public ButtonTable buttonData;
        public GameObject chipsContainer;
        public GameObject centerPivot;

        [Tooltip ("Hold duration in seconds")]
        [Range (0.1f, 5f)] public float holdDuration = 0.5f ;
        public Button button;

        private bool isPointerDown = false ;
        private bool isLongPressed = false ;
        private float elapsedTime = 0f;

        private bool _isActive;

        private float startTime, endTime;
        bool _isPressed; 

        
        public bool HasFichasOnTop
        {
            get
            {
                bool _fichasTopBoolean = false;
                if (buttonData.currentChipsOnTop != 0)
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
            buttonData.currentSpritePivot = centerPivot.transform.position;
            buttonData.currentChipsOnTop = 0;
            buttonData.currentOffset = new Vector2(0, 0);
        
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
            gameCmdFactory.ButtonTableTurn(this.gameObject, newChip, chipsContainer, characterTable, buttonData, characterTable.currentChipSelected).Execute();
        }

        public void OnPointerDown (PointerEventData eventData) 
        {
            isPointerDown = true ;
            
        }

        private void Update () {
            if (isPointerDown && !isLongPressed) {
                elapsedTime += Time.deltaTime ;
                if (elapsedTime >= holdDuration) {
                    isLongPressed = true ;
                    elapsedTime = 0f ;
                    if (button.interactable)
                    {
                        LongPress longPress = new LongPress()
                        {
                            isPressed = true,
                            values = buttonData.buttonValue
                        };

                        characterTable.OnPressedButton.OnNext(longPress);
                    }
                }
            }
        }

        public  void OnPointerUp (PointerEventData eventData) 
        {
            isPointerDown = false ;
            isLongPressed = false ;
            elapsedTime = 0f ;

            Click();

            LongPress longPress = new LongPress()
            {
                isPressed = false,
                values = buttonData.buttonValue
            };
            characterTable.OnPressedButton.OnNext(longPress);
        }
    }
}
