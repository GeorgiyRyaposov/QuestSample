using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using Code.Scripts.Services.Common;

namespace Code.Scripts.Services
{
    public class GameStateService : IService, IInitializable
    {
        private GameStateData _gameState = new();

        public void Initialize()
        {
            Mediator.SetStateLinks(_gameState);
        }
    }
}