using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using System.Linq;
using UniRx;
using Controllers;

namespace Components
{
    public class ChipGame : MonoBehaviour
    {
        [SerializeField] private CharacterTable characterTable;
        [SerializeField] private SpriteRenderer spriteRenderer;
        public Chip currentChipData {get; private set;}
        public Vector2 currentPosition {get; private set;}
        public ButtonTable currentButton {get; private set;}

        public void StartChip(Chip chipData, Vector2 position, ButtonTable buttonPressed)
        {
            this.currentChipData = chipData;
            this.currentPosition = position;
            this.currentButton = buttonPressed;

            spriteRenderer.sprite = chipData.chipSprite;
        }

        public bool HasNumber(int num)
        {
            return currentButton.buttonValue.Contains(num);
        }

        public void DestroyChip()
        {
            Destroy(this.gameObject);
        }

        void OnDestroy()
        {
            if(currentChipData == null)
                return;

            characterTable.OnDestroyChip
                .OnNext(this);
            
            try
            {
                PlayerSound.Instance.gameSound.OnSound.OnNext(2);
            }
            catch
            {
                return;
            }
        }
    }
}
