using System.Threading.Tasks;
using Code.Scripts.App.Common;
using Code.Scripts.App.ScenesManagement;
using Code.Scripts.Services;
using UnityEngine;

namespace Code.Scripts.App.AppState
{
    [CreateAssetMenu(menuName = "Data/AppState/GameplayState", fileName = "GameplayState")]
    public class GameplayState : AppState
    {
        public override Task Enter()
        {
            Preloader.Hide();
            
            Mediator.Get<InputService>().EnablePlayerInput();
            
            return Task.CompletedTask;
        }

        public override Task Exit()
        {
            Mediator.Get<InputService>().DisablePlayerInput();
            
            return Task.CompletedTask;
        }
    }
}