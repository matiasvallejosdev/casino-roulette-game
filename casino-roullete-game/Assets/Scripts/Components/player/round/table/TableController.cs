using UnityEngine;
using ViewModel;
using UniRx;
using Commands;
using System.Linq;

namespace Components
{
    public class TableController : MonoBehaviour, ITableController
    {
        public CharacterTable characterTable;

        public void LastChipDestroy(bool value)
        {
            if(characterTable.currentTableCount > 0)
            {
                Debug.Log("Undo chip of the table!");
                Destroy(characterTable.currentTable[characterTable.currentTable.Count - 1].gameObject);
            }
        }

        public void DestroyChipFromTable(ChipGame ficha) 
        {

            if(ficha._chipRuntime.currentChipData.chipValue > 0 && characterTable.currentTableCount > 0)
            {
                characterTable.characterMoney.DeleteChip(ficha._chipRuntime.currentChipData.chipValue); // Delete money
                characterTable.currentTableCount--;
                characterTable.currentTableInGame.RemoveAt(characterTable.currentTableInGame.Count() - 1);     
            }   

            ficha._chipRuntime.currentButton.SubstractCurrentOffset();
            characterTable.currentTable.Remove(ficha);
        }   

        public void ResetTable(bool destroyChips)
        {
            characterTable.characterMoney.characterBet.Value = 0;
            characterTable.currentTableCount = 0;
            characterTable.currentTableInGame.Clear();

            if(!destroyChips)
                return;

            foreach(ChipGame go in characterTable.currentTable)
            {
                Destroy(go.gameObject);
            }

            characterTable.currentTableActive.Value = true;
            characterTable.currentTable.Clear();
            characterTable.lastTable.Clear();
        }

        public void RestoreTable(Table table)
        {
            if(!characterTable.currentTableActive.Value)
                return;
            
            Debug.Log($"Loading current player table with {table.TableChips.Count()} chips");

            foreach(TableChips buttonChip in table.TableChips)
            {
                GameObject buttonInstance = GameObject.Find(buttonChip.idButton);
                GameObject chipInstance = Instantiate(characterTable.chipPrefab);
                chipInstance.SetActive(false);
                GameObject chipContainer = GameObject.FindGameObjectWithTag("ChipContainer");
                ButtonTable buttonData = buttonInstance.GetComponent<ButtonTableInput>().buttonData;
                Chip chipData = characterTable.chipData.Where(chip => chip.chipkey.ToString() == buttonChip.idChip).First();

                //gameCmdFactory.ButtonTableTurn(characterTable, chipInstance, buttonData).Execute();
            }
        }
    }
}
