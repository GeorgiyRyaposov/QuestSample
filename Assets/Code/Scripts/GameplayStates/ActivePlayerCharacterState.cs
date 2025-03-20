using Code.Scripts.App.Common;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.GameplayStates
{
    public class ActivePlayerCharacterState : State
    {
        public override UniTask Enter()
        {
            Mediator.Get<InputService>().EnablePlayerInput();
            
            return base.Enter();
        }

        public override UniTask Exit()
        {
            Mediator.Get<InputService>().DisablePlayerInput();
            
            return base.Exit();
        }
    }
}