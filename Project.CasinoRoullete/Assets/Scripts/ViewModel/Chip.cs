using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ViewModel
{
    [CreateAssetMenu(fileName = "new chip", menuName = "Scriptable/Chip")]
    public class Chip : ScriptableObject
    {
        public string chipName;
        public KeyFicha chipkey;
        public int chipValue;
        public Sprite chipSprite;
        public int chipArray;
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
}
