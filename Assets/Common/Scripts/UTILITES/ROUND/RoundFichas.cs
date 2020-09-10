using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class RoundFichas
{
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
            AddFicha(ficha.GetComponent<fichas>());
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
            SoundContoller.Instance.fx_sound(4);
            foreach (var ficha in fichasInGame)
            {
                fichas _scFicha = ficha.GetComponent<fichas>();
                AddFicha(_scFicha);
            }
        }
        else
        {
            SoundContoller.Instance.fx_sound(3);
        }
    }
    
    // Add new fichas to the table game
    private static void AddFicha(fichas fichaScript)
    {
        if (RoundController.Instance.CheckBetValue(fichaScript._valor))
        {
            // Get Parameter
            #region Parameters
            string clave = fichaScript.button.GetComponent<fx_button>().clave;
            int[] valor = fichaScript.button.GetComponent<fx_button>().valor;
            bool pleno = fichaScript.button.GetComponent<fx_button>().pleno;
            Vector2 pivot = fichaScript.button.GetComponent<fx_button>().GetSpritePivot(fichaScript.button.GetComponent<fx_button>()._spriteRender.sprite);
            int fichaIndex = fichaScript.GetIndex();
            bool _fichasTopBoolean = false;
            if (fichaScript.button.GetComponent<fx_button>()._fichasOnTop != 0)
            {
                _fichasTopBoolean = true;
            }
            Vector2 offset = fichaScript.button.GetComponent<fx_button>().GetOffsetFicha();
            #endregion
            // Ficha Nueva
            RoundController.Instance._scManejadorFichas.RecoverFichas(pivot, _fichasTopBoolean, offset, clave, valor, pleno, fichaScript.button, fichaIndex);
            // Sound Control
            SoundContoller.Instance.fx_sound(1);
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
            bool _fichasTopBoolean = false;
            GameObject btn = GameObject.Find(ficha.btn);
            #endregion
            // Ficha Nueva
            RoundController.Instance._scManejadorFichas.RecoverFichas(pivot, _fichasTopBoolean, offset, clave, valor, pleno, btn, fichaIndex);
            // Sound Control
            SoundContoller.Instance.fx_sound(1);
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
            fichas _scFicha = ficha.GetComponent<fichas>();
            aux = aux + _scFicha._valor;
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
            fichas fichaScript = ficha.GetComponent<fichas>();
            #region Parameters
            string clave = fichaScript.button.GetComponent<fx_button>().clave;
            int[] valor = fichaScript.button.GetComponent<fx_button>().valor;
            bool pleno = fichaScript.button.GetComponent<fx_button>().pleno;
            Vector2 pivot = ficha.transform.position;
            float[] position = { pivot.x, pivot.y };
            int index = fichaScript.GetIndex();
            int costo = fichaScript._valor;
            string btn = fichaScript.button.GetComponent<fx_button>().name;

            FichasSave toSave = new FichasSave(position, clave, valor, pleno, index, costo, btn);
            #endregion
            f.Add(toSave);
        }
        return f.ToArray();
    }
    
}

