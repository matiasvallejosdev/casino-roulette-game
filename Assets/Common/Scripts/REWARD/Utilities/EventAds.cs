using UnityEngine.Events;

public class EventAds
{
    /// <summary>
    /// Event when ads is reward.
    /// </summary>
    [System.Serializable] public class EventRewardVideo : UnityEvent<bool> { }
    [System.Serializable] public class EventRewardShop : UnityEvent<bool, int> { }
}
