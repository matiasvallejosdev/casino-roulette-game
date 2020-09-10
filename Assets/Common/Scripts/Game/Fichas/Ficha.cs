using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Ficha", menuName = "Ficha")]
public class Ficha : ScriptableObject
{
    public KeyFicha keyFicha;
    public int valueFicha;
    public Sprite imageFicha;
    public int indexArrayFicha;
}

public enum KeyFicha
{
    Ficha10,
    Ficha20,
    Ficha50,
    Ficha100,
    Ficha500,
    Ficha1000,
    Ficha5000,
    Ficha10000,
    Ficha50000,
    Ficha100000,
    FichaAll
}
