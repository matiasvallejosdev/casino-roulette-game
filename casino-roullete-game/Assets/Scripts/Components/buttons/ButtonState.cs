using UnityEngine;
using ViewModel;

namespace Commands
{
    public class ButtonState : MonoBehaviour, IReseteableButton, IStatusButton
    {
        public Transform centerPivot;
        public bool _isActive { get; set; }
        public bool _isPressed {get;set;}

        public bool _hasFichasOnTop => throw new System.NotImplementedException();

        public void ResetButton(ButtonTable buttonData)
        {
            buttonData.currentSpritePivot = centerPivot.position;
            buttonData.currentChipsOnTop = 0;
            buttonData.currentOffset = new Vector2(0, 0);
            _isPressed = false;
        }
    }
}
