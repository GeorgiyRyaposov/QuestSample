using System.Linq;
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
        
        public void ResetGameState()
        {
            _gameState.SessionState = new SessionStateData();
            Mediator.SetStateLinks(_gameState);
        }

        public async UniTask LoadSessionStateAsync(SaveMetadata saveFile)
        {
            _gameState.SessionState = await SaveManager.LoadDataAsync(saveFile.DataFileName);
            Mediator.SetStateLinks(_gameState);
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
            _gameState.SessionState.PlayerPosition = _gameState.SessionState.LastGroundedPlayerPosition;
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
            
            metaData = metaData.OrderByDescending(x => x.SaveDate).ToArray();
            
            return metaData;
        }

        public async UniTask<SaveMetadata> GetLatestSaveFile()
        {
            var files = await GetSaveFiles();
            return files.FirstOrDefault();
        }
    }
}