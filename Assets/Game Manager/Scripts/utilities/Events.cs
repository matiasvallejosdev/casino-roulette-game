using UnityEngine.Events;

public class Events
{
    // Event when game stat change
    [System.Serializable] public class EventGameState : UnityEvent<game_manager.GameState, game_manager.GameState> { }
    // Event when fade is completed
    [System.Serializable] public class EventFadeComplete : UnityEvent<bool> { }
    // Event when the game is restarting
    [System.Serializable] public class EventRestartGame : UnityEvent<bool> { }

    // Ej:
    // Podes añadir nuevos eventos 
    //  EJ: onFadeOutComplete
}
