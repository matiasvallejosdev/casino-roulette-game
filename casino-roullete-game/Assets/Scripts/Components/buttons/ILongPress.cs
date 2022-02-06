using ViewModel;

namespace Commands
{
    public interface ILongPress
    {
        void SetPointerDown(bool value);
        void LongPressCheck(CharacterTable characterTable, ButtonTable buttonData);
        void ResetPointer();
        void LongPress(CharacterTable characterTable, ButtonTable buttonData, bool currentStatus);
    }
}
