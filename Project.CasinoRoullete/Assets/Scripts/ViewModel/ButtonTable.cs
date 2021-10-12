using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ViewModel
{    
    [CreateAssetMenu(fileName = "New Button Table", menuName = "Scriptable/Button Table")]
    public class ButtonTable : ScriptableObject
    {
        public int[] buttonValue;
        public ButtonTable[] buttonParent;
        public KeyButton buttonKey;
        public bool isPleno;

        public int currentChipsOnTop;
        public Vector2 currentSpritePivot;
        public Vector2 currentOffset;  

        public BoolReactiveProperty isActive; 
         
        public ISubject<bool> OnPress = new Subject<bool>();    
        public ISubject<bool> OnWin = new Subject<bool>();    
        public ISubject<bool> OnPressed = new Subject<bool>();   

        public Vector2 GetCurrentOffset()
        {
            Vector2 v = new Vector2(0.01f,0.038f);
            currentOffset = currentOffset + v;
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
