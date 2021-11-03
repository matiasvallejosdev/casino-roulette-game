using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Components;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "New Character Table", menuName = "Scriptable/Character Table")]
    public class CharacterTable : ScriptableObject
    {
        [Header("Parameters")]
        public string tableName;
        public GameObject chipPrefab;
        public CharacterMoney characterMoney;
        
        [Header("Runtime Execution")]
        // Current round
        public int currentTableCounter;
        public Chip currentChipSelected;
        public List<ChipGame> currentTable = new List<ChipGame>();
        public List<int> currentNumbers = new List<int>();

        // Last round
        public int lastNumber;
        //public List<ChipGame> lastTable = new List<ChipGame>();

        // Events observables
        public ISubject<ChipGame> OnDestroyChip = new Subject<ChipGame>();

        public ISubject<bool> OnActiveButton = new Subject<bool>();
        public ISubject<int> OnWinButton = new Subject<int>();
        public ISubject<LongPress> OnPressedButton = new Subject<LongPress>();

        public ISubject<bool> OnSaveGame = new Subject<bool>();
        public ISubject<bool> OnRound = new Subject<bool>();
    }
}
