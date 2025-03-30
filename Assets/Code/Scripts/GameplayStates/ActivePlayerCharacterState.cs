using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.GameplayStates
{
    public class ActivePlayerCharacterState : State
    {
        private InputState InputState => Mediator.InputState;
        private readonly PauseGameListener _pauseGameListener = new();
        
        public override UniTask Enter()
        {
            Mediator.Get<InteractionsService>().SetPlayerCharacterActive(true);
            
            return base.Enter();
        }

        public override UniTask Exit()
        {
            Mediator.Get<InteractionsService>().SetPlayerCharacterActive(false);
            return base.Exit();
        }

        public override void OnUpdate()
        {
            if (InputState.Interact)
            {
                InputState.Interact = false;
                
                Interact().Forget();
            }
            
            _pauseGameListener.OnUpdate();
        }

        private async UniTaskVoid Interact()
        {
            await Mediator.Get<InteractionsService>().Interact();
        }
    }
}