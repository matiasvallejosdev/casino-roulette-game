using System;
using UniRx;
using ViewModel;

public interface IPayment 
{
    public IObservable<Unit> PaymentSystem(CharacterTable characterTable);
    public int PaymentValue
    {
        get;
        set;
    }
}