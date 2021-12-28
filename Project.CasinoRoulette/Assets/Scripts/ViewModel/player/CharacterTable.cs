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
        public Chip[] chipData;
        
        [Header("Runtime Execution")]
        // Current round
        public int currentTableCount;
        public List<ChipGame> currentTable = new List<ChipGame>();
        public List<TableChips> currentTableInGame = new List<TableChips>();
        public List<int> currentNumbers = new List<int>();
        public Chip currentChipSelected;
        public BoolReactiveProperty currentTableActive;

        [Header("Last Execution")]
        // Last round
        public int lastNumber;
        public List<TableChips> lastTable = new List<TableChips>();

        // Events observables
        public ISubject<ChipGame> OnDestroyChip = new Subject<ChipGame>();
        public ISubject<bool> OnDestroyLastChip = new Subject<bool>();

        public ISubject<int> OnWinButton = new Subject<int>();
        public ISubject<LongPress> OnPressedButton = new Subject<LongPress>();

        public ISubject<Table> OnRestoreTable = new Subject<Table>();
        public ISubject<bool> OnSaveGame = new Subject<bool>();
        public ISubject<bool> OnLoadGame = new Subject<bool>();
        public ISubject<bool> OnResetGame = new Subject<bool>();
        public ISubject<bool> OnRound = new Subject<bool>();
    }
}
