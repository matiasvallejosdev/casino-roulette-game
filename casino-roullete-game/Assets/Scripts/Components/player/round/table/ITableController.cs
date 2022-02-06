using ViewModel;

namespace Components
{
    public interface ITableController
    {
        void DestroyChipFromTable(ChipGame ficha);
        void LastChipDestroy(bool value);
        void RestoreTable(Table table);
        void ResetTable(bool destroyChips);
    }
}
