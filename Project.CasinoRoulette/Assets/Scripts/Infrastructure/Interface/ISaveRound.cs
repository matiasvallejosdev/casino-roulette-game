using System;
using System.Collections;
using UniRx;
using UnityEngine;
using ViewModel;

namespace Infrastructure
{
    public interface ISaveRound 
    {
        IObservable<Unit> RoundSequentialSave(Round roundData);
        IObservable<Unit> RoundSequentialLoad();
        public Round roundData {get; set;}    
    }
}
