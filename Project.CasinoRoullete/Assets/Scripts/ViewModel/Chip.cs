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
    }

    public enum KeyFicha
    {
        Chip10,
        Chip20,
        Chip50,
        Chip100,
        Chip500,
        Chip1000,
        Chip5000,
        Chip10000,
        Chip50000,
        Chip100000,
        Chip500000,
        Chip1000000,
        ChipAll
    }
}
