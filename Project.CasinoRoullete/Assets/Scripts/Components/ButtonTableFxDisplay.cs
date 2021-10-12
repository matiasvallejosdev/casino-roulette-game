using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using UniRx;

namespace Components
{
    public class ButtonTableFxDisplay : MonoBehaviour
    {
        public ButtonTable buttonData;
        public SpriteRenderer spriteRenderer;

        void Start()
        {
            buttonData.OnPress
                .Subscribe(FxFicha)
                .AddTo(this);

            buttonData.OnWin
                .Subscribe(FxNumberWinner)
                .AddTo(this);
                
            buttonData.OnPressed
                .Subscribe(FxButtonPressed)
                .AddTo(this);
        }

        public void FxFicha(bool parameter)
        {
            if(parameter)
            {
                spriteRenderer.color = new Color(255,255,255,0.58f);
            } 
            else
            {
                spriteRenderer.color = new Color(255,255,255,0);
            }
        }

        public void FxNumberWinner(bool parameter)
        {
            if (parameter)
            {
                spriteRenderer.color = new Color(233, 191, 9, 0.58f);
            }
            else
            {
                spriteRenderer.color = new Color(233, 191, 9, 0);
            }
        }

        public void FxButtonPressed(bool isPress) 
        {
            // Activate this button
            this.FxFicha(isPress);
            // Activate the parent buttons
            foreach(ButtonTable btn in buttonData.buttonParent) 
            {
                btn.OnPress.OnNext(isPress);
            }
        }
    }
}
