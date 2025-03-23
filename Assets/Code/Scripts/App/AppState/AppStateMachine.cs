using Code.Scripts.Services.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.App.AppState
{
    //[CreateAssetMenu(menuName = "Data/AppState/AppStateMachine", fileName = "AppStateMachine")]
    public class AppStateMachine : ScriptableService
    {
        [SerializeField] private AppState _mainMenuState;
        [SerializeField] private AppState _prepareGameState;
        [SerializeField] private AppState _gameplayState;
        
        private IAppState _activeState;

        public async UniTask SetupFromMainMenu()
        {
            await Setup(_mainMenuState);
        }
        
        public async UniTask SetupFromGameplay()
        {
            await Setup(_prepareGameState);
            await Enter(_gameplayState);
        }
        
        public async UniTask LoadGameplay()
        {
            await Enter(_prepareGameState);
            await Enter(_gameplayState);
        }
        
        private async UniTask Setup(IAppState state)
        {
            _activeState = state;
            await _activeState.Enter();
        }

        private async UniTask Enter(IAppState state)
        {
            if (_activeState == state)
            {
                return;
            }
            
            if (_activeState != null)
            {
                await _activeState.Exit();
            }
            
            _activeState = state;
            await _activeState.Enter();
        }
    }
}
