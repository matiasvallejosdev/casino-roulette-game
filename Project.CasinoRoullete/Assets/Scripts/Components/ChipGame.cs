using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel;
using System.Linq;
using UniRx;

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

        public void DestroyMagnet()
        {
            Destroy(this.gameObject);
        }

        void OnDestroy()
        {
            characterTable.OnDestroyChip
                .OnNext(this);
        }
    }
}
