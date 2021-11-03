using System;
using System.Collections;
using UniRx;
using UnityEngine;
using ViewModel;

namespace Infrastructure
{
    public interface ISaveRound 
    {
        IObservable<Unit> RoundSequentialSave(CharacterTable characterTable);
        IObservable<Unit> RoundSequentialLoad(CharacterTable characterTable);
        public Round roundData {get; set;}    
    }
}
