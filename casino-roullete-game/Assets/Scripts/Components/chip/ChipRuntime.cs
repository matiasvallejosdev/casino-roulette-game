using UnityEngine;
using ViewModel;

namespace Components
{
    public class ChipRuntime : MonoBehaviour, IChipRuntime
    {
        public Chip currentChipData { get; set; }
        public Vector2 currentPosition { get; set; }
        public ButtonTable currentButton { get; set; }

        public void StartChip(Chip chipData, Vector2 position, ButtonTable buttonPressed, SpriteRenderer currentSprite)
        {
            this.currentChipData = chipData;
            this.currentPosition = position;
            this.currentButton = buttonPressed;

            //currentSprite.sprite = chipData.chipSprite;
        }
    }
}
