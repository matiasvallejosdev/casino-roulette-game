using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ViewModel
{    
    [CreateAssetMenu(fileName = "New Button Table", menuName = "Scriptable/Button Table")]
    public class ButtonTable : ScriptableObject
    {
        public string buttonName;
        public int[] buttonValue;
        public KeyButton buttonKey;
        public bool isPleno;

        public int currentChipsOnTop;
        public Vector2 currentSpritePivot;
        public Vector2 currentOffset;  

        public Vector2 GetOffset()
        {
            Vector2 offset = currentOffset;

            Vector2 v = new Vector2(0.01f,0.038f);
            currentOffset = currentOffset + v;
            
            return offset;
        }
        public Vector2 SubstractCurrentOffset()
        {
            Vector2 v = new Vector2(0.01f,0.038f);
            currentOffset = currentOffset - v;
            return currentOffset;
        }
    }

    public enum KeyButton
    {
        NumberPleno,
        NumberMiddle,
        Dozen,
        Column,
        EvenOdd,
        Eighteenth,
        BlackRed
    }
}
