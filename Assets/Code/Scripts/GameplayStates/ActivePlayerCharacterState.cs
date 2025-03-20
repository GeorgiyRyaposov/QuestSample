using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.GameplayStates
{
    public class ActivePlayerCharacterState : State
    {
        private InputState InputState => Mediator.InputState;
        
        public override UniTask Enter()
        {
            SetPlayerCharacterActive(true);
            
            return base.Enter();
        }

        public override UniTask Exit()
        {
            SetPlayerCharacterActive(false);
            
            return base.Exit();
        }

        public override void OnUpdate()
        {
            if (InputState.Interact)
            {
                InputState.Interact = false;
                
                Interact().Forget();
            }
        }

        private async UniTaskVoid Interact()
        {
            SetPlayerCharacterActive(false);
            await Mediator.Get<InteractionsService>().Interact();
            SetPlayerCharacterActive(true);
        }

        private void SetPlayerCharacterActive(bool active)
        {
            if (active)
            {
                Mediator.Get<InputService>().EnablePlayerInput();
            }
            else
            {
                Mediator.Get<InputService>().DisablePlayerInput();
            }
            
            if (Mediator.PlayerCharacter != null)
            {
                Mediator.PlayerCharacter.enabled = active;
            }
        }
    }
}