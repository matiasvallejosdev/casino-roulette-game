using UnityEngine.Events;

public class EventsRound
{
    // Round Saldos       (SALDO ANTERIOR, SALDO NUEVO)
    [System.Serializable] public class EventRoundState : UnityEvent<int, int> { }
    // Round Apuestas     (APUESTA ANTERIOR, NUEVA Y TOTAL)
    [System.Serializable] public class EventApuestaChanged : UnityEvent<int, int, int> { }
    // Round New Apuesta  (
}
