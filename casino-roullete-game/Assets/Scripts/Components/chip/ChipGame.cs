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
        public Transform chipsContainer;
        public SpriteRenderer spriteRenderer;
        public CharacterTable characterTable;
        public IChipRuntime _chipRuntime;

        void Awake()
        {
            chipsContainer = GameObject.Find("ChipsContainer").GetComponent<Transform>();
            _chipRuntime = GetComponent<IChipRuntime>();
        }
        
        public bool HasNumber(int num)
        {
            return _chipRuntime.currentButton.buttonValue.Contains(num);
        }

        public void DestroyChip()
        {
            Destroy(this.gameObject);
        }

        void OnDestroy()
        {
            if(_chipRuntime.currentChipData == null)
                return;

            characterTable.OnDestroyChip
                .OnNext(this);
            
            PlayerSound.Instance.gameSound.OnSound.OnNext(PlayerSound.Instance.gameSound.audioReferences[4]);
        }
    }
}
