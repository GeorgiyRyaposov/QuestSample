using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using Code.Scripts.Services.Common;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.Services
{
    public class GameStateService : IService, IInitializable
    {
        private readonly GameStateData _gameState = new();

        public void Initialize()
        {
            Mediator.SetStateLinks(_gameState);
        }

        public async UniTask LoadSessionStateAsync(SaveMetadata saveFile)
        {
            _gameState.SessionState = await SaveManager.LoadDataAsync(saveFile.DataFileName);
        }

        public async UniTask AutoSaveSessionStateAsync()
        {
            await SaveSessionStateAsync($"AutoSave_{Mediator.SessionState.SessionId}");
        }
        
        public async UniTask SaveSessionStateAsync()
        {
            //todo: replace with player input value?
            var saveName = $"#{Mediator.SessionState.SessionId}.";
            await SaveSessionStateAsync(saveName);
        }
        
        private async UniTask SaveSessionStateAsync(string saveName)
        {
            _gameState.SaveInProgress = true;
            await SaveManager.SaveAsync(_gameState.SessionState, saveName);
            _gameState.SaveInProgress = false;
        }

        public async UniTask<SaveMetadata[]> GetSaveFiles()
        {
            var paths = SaveManager.GetAllSavePaths();
            var metaData = new SaveMetadata[paths.Length];
            var i = 0;
            foreach (var path in paths)
            {
                var data = await SaveManager.LoadMetadataAsync(path);
                metaData[i++] = data;
            }
            
            return metaData;
        }
    }
}