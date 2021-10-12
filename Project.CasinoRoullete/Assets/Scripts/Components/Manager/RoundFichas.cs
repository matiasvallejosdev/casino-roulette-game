using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class RoundFichas
{/*
    /// <summary>
    /// Restore group of fichas in the game.
    /// </summary>
    /// <param name="fichas"></param>
    public static void RestorePlayerRound(FichasSave[] fichas)
    {
        FichasSave[] fichasPrevious = fichas;
        foreach (FichasSave ficha in fichasPrevious)
        {
            RestoreFicha(ficha);
        }
    }
    /// <summary>
    /// Restore the prevoius played round.
    /// </summary>
    /// <param name="fichas"></param>
    public static void RestorePreviousRound(GameObject[] fichas)
    {
        GameObject[] fichasPrevious = fichas;
        foreach (var ficha in fichasPrevious)
        {
            AddFicha(ficha.GetComponent<FichaDisplay>());
        }
    }
    /// <summary>
    /// Delete the previous ficha one at time
    /// </summary>
    public static void UndoFichasInTable() 
    {
        GameObject[] fichasInGame = GameObject.FindGameObjectsWithTag("Fichas");
        GameObject lastFicha; 
        if(fichasInGame != null) 
        {
            lastFicha = FindLastFicha(fichasInGame);
            if(lastFicha != null) 
            {
                // Destroy ficha
                RoundController.Instance.UndoTable(lastFicha);
                SoundContoller.Instance.PlayFxSound(2);
            }
        }
    }
    private static GameObject FindLastFicha(GameObject[] fichasInGame) 
    {
        GameObject f = null;
        int r = 0;
        foreach (var ficha in fichasInGame)
        {
            if(Convert.ToInt32(ficha.name) > r) 
            {
                f = ficha;
                r = Convert.ToInt32(r);
            }
        }
        return f;
    }
    /// <summary>
    /// Multiplier the fichas in the table when is possible.
    /// </summary>
    public static void MultiplierFichasInTable()
    {
        GameObject[] fichasInGame = GameObject.FindGameObjectsWithTag("Fichas");
        if (FindIsPossibleToDuplicate(fichasInGame))
        {
            SoundContoller.Instance.PlayFxSound(1);
            foreach (var ficha in fichasInGame)
            {
                FichaDisplay _scFicha = ficha.GetComponent<FichaDisplay>();
                AddFicha(_scFicha);
            }
        }
        else
        {
            SoundContoller.Instance.PlayFxSound(4);
        }
    }
    
    // Add new fichas to the table game
    private static void AddFicha(FichaDisplay fichaScript)
    {
        if (RoundController.Instance.CheckBetValue(fichaScript.GetValueFicha()))
        {
            ButtonDisplay scButton = fichaScript.currentButton;
            // Get Parameter
            string clave = scButton.button.buttonKey.ToString();
            int[] valor = scButton.button.buttonValue;
            bool pleno = scButton.button.isPleno;
            Vector2 pivot = scButton.GetSpritePivot(fichaScript.currentButton._spriteRender.sprite);
            int fichaIndex = fichaScript.GetIndexFicha();
            bool _fichasTopBoolean = false;
            if (scButton.GetCurrentFichasOnTop() != 0)
            {
                _fichasTopBoolean = true;
            }
            scButton.AddFichasOnTop(1);

            Vector2 offset = scButton.GetOffsetFicha();
            // Ficha Nueva
            RoundController.Instance._scManejadorFichas.RecoverFichas(pivot, _fichasTopBoolean, offset, clave, valor, pleno, scButton.gameObject, fichaIndex);
            // Sound Control
            SoundContoller.Instance.PlayFxSound(1);
        }
        else
        {
            Debug.Log("Bet is not possible because the value of ficha is very high");
        }
    }
    private static void RestoreFicha(FichasSave ficha)
    {
        if (RoundController.Instance.CheckBetValue(ficha.costo))
        {
            //Debug.Log("Bet possible");
            // Get Parameter
            #region Parameters
            string clave = ficha.clave;
            int[] valor = ficha.valor;
            bool pleno = ficha.pleno;
            Vector2 pivot = new Vector2(ficha.positionXY[0], ficha.positionXY[1]);
            Vector2 offset = new Vector2(0, 0);
            int fichaIndex = ficha.index;
            GameObject btn = GameObject.Find(ficha.btn);
            int _fichasOnTop = btn.GetComponent<ButtonDisplay>().GetCurrentFichasOnTop();
            bool _fichasTopBoolean = false;
            if (_fichasOnTop != 0)
            {
                _fichasTopBoolean = true;
            }
            btn.GetComponent<ButtonDisplay>().AddFichasOnTop(1);
            #endregion
            // Ficha Nueva
            RoundController.Instance._scManejadorFichas.RecoverFichas(pivot, _fichasTopBoolean, offset, clave, valor, pleno, btn, fichaIndex);
            // Sound Control
            SoundContoller.Instance.PlayFxSound(1);
        }
        else
        {
            Debug.Log("Bet is not possible because the value of ficha is very high");
        }
    }

    // Search if is possible i will duplicate the new fichas
    public static bool FindIsPossibleToDuplicate(GameObject[] fichasInGame)
    {
        bool isPossible;
        int aux = 0;
        foreach (var ficha in fichasInGame)
        {
            FichaDisplay _scFicha = ficha.GetComponent<FichaDisplay>();
            aux = aux + _scFicha.GetValueFicha();
        }
        if (aux <= RoundController.Instance.GetCashTotal())
        {
            isPossible = true;
        }
        else
        {
            isPossible = false;
        }
        return isPossible;
    }

    // Search all the fichas in table to save in archive
    public static FichasSave[] ReturnFichasToSave(GameObject[] round)
    {
        List<FichasSave> f = new List<FichasSave>();
        foreach (var ficha in round) 
        {
            FichaDisplay fichaScript = ficha.GetComponent<FichaDisplay>();
            string clave = fichaScript.currentButton.button.buttonKey.ToString();
            int[] valor = fichaScript.currentButton.button.buttonValue;
            bool pleno = fichaScript.currentButton.button.isPleno;

            Vector2 pivot = ficha.transform.position;
            float[] position = { pivot.x, pivot.y };
            int index = fichaScript.GetIndexFicha();
            int costo = fichaScript.GetValueFicha();
            string btn = fichaScript.currentButton.button.name;

            FichasSave toSave = new FichasSave(position, clave, valor, pleno, index, costo, btn);
            f.Add(toSave);
        }
        return f.ToArray();
    }
    */
}

