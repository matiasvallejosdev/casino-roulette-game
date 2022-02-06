using ViewModel;

namespace Components
{
    public interface IRewardTimer
    {
        public ulong LastChestOpen();
        public bool IsRewardReady(RewardFortune rewardFortune, float SecondsToWait);
        public string CalculateTimer(float secondsToWait);
    }
}
