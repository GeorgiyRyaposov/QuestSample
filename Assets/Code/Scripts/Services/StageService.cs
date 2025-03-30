using System;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.Components;
using Code.Scripts.Configs;
using Code.Scripts.GameplayStates;
using Code.Scripts.Persistence;
using Code.Scripts.Services.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Services
{
    public class StageService : IService
    {
        private StagesContainer StagesContainer => Mediator.Get<AssetsService>().StagesContainer;
        private StageLoader _stageLoader;
        
        public async UniTask LoadStage(StageInfo stageToLoad)
        {
            if (_stageLoader != null)
            {
                await _stageLoader.UnloadAsync();
            }

            _stageLoader = new StageLoader(stageToLoad.Scene);
            await _stageLoader.LoadStage();

            PrepareScene(_stageLoader.GetSceneGameObjects());
        }

        private void PrepareScene(GameObject[] sceneGameObjects)
        {
            if (!TryGetStageLinks(sceneGameObjects, out var links))
            {
                Debug.Log($"Scene '{Mediator.SessionState.CurrentStageId}' links not found");
                return;
            }
            
            links.CollectLinks();

            Mediator.Get<InteractionsService>().SetupItemsState(links.InteractionItems);
            SpawnPlayer(links.PlayerSpawnPoints);
        }

        private bool TryGetStageLinks(GameObject[] sceneGameObjects, out StageLinks links)
        {
            foreach (var gameObject in sceneGameObjects)
            {
                if (gameObject.TryGetComponent(out links))
                {
                    return true;
                }
            }

            links = null;
            return false;
        }
        
        private void SpawnPlayer(PlayerSpawnPoint[] playerSpawnPoints)
        {
            var position = GetSpawnPoint(playerSpawnPoints);

            var ray = new Ray(position + Vector3.up * 2, Vector3.down);
            if (!Physics.Raycast(ray, out var hitInfo, 20f, 1 << LayerMask.NameToLayer("Ground")))
            {
                Debug.LogError($"Failed to find player spawn point ground at '{Mediator.SessionState.CurrentStageId}'");
                return;
            }
            
            Mediator.PlayerCharacter.transform.position = hitInfo.point;
        }

        private static Vector3 GetSpawnPoint(PlayerSpawnPoint[] playerSpawnPoints)
        {
            var loadAtSpawnPoint =
                Mediator.GameState.StageLoadingMode 
                    is StageLoadingMode.StageTransition 
                    or StageLoadingMode.NewGame;
                                   
            if (loadAtSpawnPoint)
            {
                var spawnPoint = FindSpawnPoint(playerSpawnPoints);
                if (!spawnPoint)
                {
                    Debug.LogError($"Failed to find player spawn point at '{Mediator.SessionState.CurrentStageId}'");
                    spawnPoint = playerSpawnPoints.FirstOrDefault();
                
                    return !spawnPoint ? Vector3.zero : spawnPoint.transform.position;
                }

                return spawnPoint.transform.position;
            }

            if (Mediator.GameState.StageLoadingMode == StageLoadingMode.SavesLoading)
            {
                return Mediator.SessionState.PlayerPosition;
            }

            Debug.LogError($"Loading stage mode undefined '{Mediator.GameState.StageLoadingMode}'");
            return Vector3.zero;
        }

        private static PlayerSpawnPoint FindSpawnPoint(PlayerSpawnPoint[] playerSpawnPoints)
        {
            PlayerSpawnPoint spawnPoint;

            var prevStage = Mediator.SessionState.PreviousStageId;
            if (string.IsNullOrEmpty(prevStage))
            {
                spawnPoint = playerSpawnPoints.FirstOrDefault(x => !x.CameFromStage);
            }
            else
            {
                spawnPoint = playerSpawnPoints.FirstOrDefault(x => 
                    x.CameFromStage && string.Equals(x.CameFromStage.Id, prevStage, StringComparison.Ordinal));
            }

            return spawnPoint;
        }

        public StageInfo GetCurrentStageOrInitial()
        {
            var currentStageId = Mediator.SessionState.CurrentStageId;
            if (string.IsNullOrEmpty(currentStageId))
            {
                return StagesContainer.StartStage;
            }
            
            return StagesContainer.Stages.FirstOrDefault(x => 
                string.Equals(x.Id, currentStageId, StringComparison.Ordinal));
        }
    }
}