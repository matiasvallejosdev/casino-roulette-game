using UnityEngine.Events;

public class EventButtonRoullete
{
    /// <summary>
    /// Event when ads is reward.
    /// </summary>
    [System.Serializable] public class EventLongPress : UnityEvent<bool, ButtonDisplay> { }
}
