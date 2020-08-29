using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SavePlayer
{
    /// <summary>
    /// Get the values of fichas to save.
    /// Vector2 Position, string Clave, int[] Valor, bool pleno, int Index
    /// </summary>
    /// <returns></returns>
    public static FichasSave[] GetFichas() 
    {
        GameObject[] fichasInGame = GameObject.FindGameObjectsWithTag("Fichas");
        List<FichasSave> temp = new List<FichasSave>();

        foreach (GameObject ficha in fichasInGame)
        {
            fichas fichaSc = ficha.GetComponent<fichas>();
            FichasSave f = new FichasSave(fichaSc.pos, fichaSc._key, fichaSc.getValueOfDictionary(fichaSc._key), fichaSc._pleno, fichaSc._fichaSelected);
            temp.Add(f);
        }

        return temp.ToArray();
    } 
}
