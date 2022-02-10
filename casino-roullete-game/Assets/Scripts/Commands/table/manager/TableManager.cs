using UnityEngine;
using ViewModel;
using Components;

namespace Commands
{
    public class TableManager
    {
        public ITable table;
        public ITableInteractable tableInteracatable;

        public TableManager(CharacterTable characterTable, ITable table, ITableInteractable tableInteraction)
        {
            this.table = table;
            this.table.characterTable = characterTable;

            this.tableInteracatable = tableInteraction;
        }
    }
}
