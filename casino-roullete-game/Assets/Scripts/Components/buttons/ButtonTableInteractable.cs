using UnityEngine;
using ViewModel;

namespace Commands
{
    public class ButtonTableInteractable : MonoBehaviour, IInteractableButton
    {
        public GameCmdFactory gameCmdFactory;

        public void InstantiateChip(CharacterTable characterTable, ButtonTable buttonData)
        {
            GameObject chipInstance = Instantiate(characterTable.chipPrefab);
            chipInstance.SetActive(false);
            gameCmdFactory.ButtonTableTurn(characterTable, chipInstance, buttonData).Execute();
        }
    }
}
