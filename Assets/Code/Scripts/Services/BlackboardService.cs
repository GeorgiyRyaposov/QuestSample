using Code.Scripts.App.Common;
using Code.Scripts.Configs.Blackboards;
using Code.Scripts.Services.Common;

namespace Code.Scripts.Services
{
    public class BlackboardService : IService
    {
        public bool IsFlagMatch(BoolKeyValue flag)
        {
            var currentValue = Mediator.SessionState.BoolBlackboard.Get(flag.Key);
            return currentValue == flag.Value;
        }
    }
}