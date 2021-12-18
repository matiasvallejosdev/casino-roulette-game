using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;
using System;
using System.Linq;

namespace Components
{
    public class ButtonTableFxDisplay : MonoBehaviour
    {
        public CharacterTable characterTable;
        public ButtonTable buttonData;
        public Animator animatorButton;

        void Start()
        {
            characterTable.OnWinButton
                .Subscribe(OnWin)
                .AddTo(this);

            characterTable.OnPressedButton
                .Subscribe(OnPressed)
                .AddTo(this);
        }

        private void OnWin(int num)
        {
            if(!buttonData.isPleno)
                return;

            bool containNumber = buttonData.buttonValue.Contains(num);
            
            if(containNumber)
            {
                FxWin();
            } 
        }
        public void OnPressed(LongPress longPress) 
        {
            if(!buttonData.isPleno)
                return;
                
            if(CheckIfIsLongPressed(longPress.values))          
                FxPressed(longPress.isPressed);
        }
        public bool CheckIfIsLongPressed(int[] longPressValues)
        {
            if(buttonData.buttonValue.Count() > 1)
                return false;
            
            return longPressValues.Contains(buttonData.buttonValue[0]);
        }


        public void FxWin()
        {
            animatorButton.SetTrigger("Win");
        }
        public void FxPressed(bool isPress)
        {
            animatorButton.SetBool("IsPressed", isPress);
        }
    }
}
