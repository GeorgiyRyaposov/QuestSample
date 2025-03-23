using System;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.Components;
using Code.Scripts.Configs;
using Code.Scripts.GameplayStates;
using Code.Scripts.Services.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Services
{
    public class StageService : IService
    {
        private StagesContainer StagesContainer => Mediator.Get<AssetsService>().StagesContainer;
        private StageLoader _stageLoader;

        public async UniTask LoadNextStage(StageInfo stageToLoad)
        {
            Mediator.SessionState.PreviousStageId = Mediator.SessionState.CurrentStageId;
            Mediator.SessionState.CurrentStageId = stageToLoad.Id;

            _stageLoader?.Dispose();

            _stageLoader = new StageLoader(stageToLoad);
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
            var spawnPoint = FindSpawnPoint(playerSpawnPoints);
            if (!spawnPoint)
            {
                Debug.LogError($"Failed to find player spawn point at '{Mediator.SessionState.CurrentStageId}'");
                return;
            }

            var ray = new Ray(spawnPoint.transform.position + Vector3.up * 10, Vector3.down);
            if (!Physics.Raycast(ray, out var hitInfo, 20f, 1 << LayerMask.NameToLayer("Ground")))
            {
                Debug.LogError($"Failed to find player spawn point ground at '{Mediator.SessionState.CurrentStageId}'");
                return;
            }
            
            Mediator.PlayerCharacter.transform.position = hitInfo.point;
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