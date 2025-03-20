using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.GameplayStates
{
    public class GameplayStateMachine : MonoBehaviour
    {
        
        private State _activeCharacterState;
        private State _activeState = new State();
        private State _emptyState = new State();
        
        public void Setup()
        {
            _activeCharacterState = new ActivePlayerCharacterState();
        }

        public async UniTask EnterInitialState()
        {
            await SetState(_activeCharacterState);
        }

        public async UniTask Dispose()
        {
            await _activeState.Exit();
            _activeState = _emptyState;
        }

        private async UniTask SetState(State state)
        {
            if (_activeState == state)
            {
                return;
            }
            
            var prevState = _activeState;
            _activeState = _emptyState;
            
            await prevState.Exit();
            
            await state.Enter();
            _activeState = state;
        }

        private void Update()
        {
            _activeState.OnUpdate();
        }
    }
}