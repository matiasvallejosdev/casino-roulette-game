using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New button table", menuName = "Button table")]
public class Button : ScriptableObject
{
    public int[] value;
    public bool isPleno;
    public KeyButton key;
}

public enum KeyButton
{
    Numero,
    Docena,
    Columna,
    ParImpar,
    Dieciochoavos,
    RojoNegro,
    Medio
}
