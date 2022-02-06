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
        public CharacterTable characterTable;
        public ButtonTable buttonData;

        private IStatusButton _statusButton;
        private IReseteableButton _resetableButton;
        private IInteractableButton _interactableButton;
        public ILongPress _longPress;
        
        void Awake() 
        {
            _statusButton = GetComponent<IStatusButton>();    
            _resetableButton = GetComponent<IReseteableButton>();    
            _interactableButton = GetComponent<IInteractableButton>();    
            _longPress = GetComponent<ILongPress>();    
        }
        
        void Start()
        {
            _resetableButton.ResetButton(buttonData);

            characterTable.currentTableActive
                .Subscribe(OnActiveButton)
                .AddTo(this);
        }
        
        private void OnActiveButton(bool isActive)
        {
            _statusButton._isActive = isActive;
            if(_statusButton._isActive) _resetableButton.ResetButton(buttonData);       
        }

        public void Click()
        {
            if (!_statusButton._isActive)
                return;

            _interactableButton.InstantiateChip(characterTable, buttonData);
        }

        public void OnPointerDown (PointerEventData eventData) 
        {
            _longPress.SetPointerDown(true);   
        }

        private void Update ()
        {
            _longPress.LongPressCheck(characterTable, buttonData);
        }
        
        public  void OnPointerUp (PointerEventData eventData) 
        {
            _longPress.LongPress(characterTable, buttonData, false);
            _longPress.ResetPointer();
            Click();
        }
    }
}
