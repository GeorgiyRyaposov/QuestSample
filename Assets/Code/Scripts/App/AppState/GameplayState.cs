using Code.Scripts.App.Common;
using Code.Scripts.App.ScenesManagement;
using Code.Scripts.GameplayStates;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.App.AppState
{
    [CreateAssetMenu(menuName = "Data/AppState/GameplayState", fileName = "GameplayState")]
    public class GameplayState : AppState
    {
        public override async UniTask Enter()
        {
            if (Mediator.GameplayStateMachine == null)
            {
                var stateMachineGameObj = new GameObject("GameplayStateMachine", typeof(GameplayStateMachine));
                Mediator.GameplayStateMachine = stateMachineGameObj.GetComponent<GameplayStateMachine>();

                Mediator.GameplayStateMachine.Setup();
            }
            
            Preloader.Hide();

            await Mediator.GameplayStateMachine.EnterInitialState();
        }

        public override async UniTask Exit()
        {
            await Mediator.GameplayStateMachine.Dispose();
        }
    }
}