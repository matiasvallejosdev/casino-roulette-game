using ViewModel;

namespace Commands
{
    public interface IInteractableButton
    {
        void InstantiateChip(CharacterTable characterTable, ButtonTable buttonData);
    }
}
