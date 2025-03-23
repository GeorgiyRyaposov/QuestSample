using System;
using Code.Scripts.Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Code.Scripts.GameplayStates
{
    public class StageLoader : IDisposable
    {
        private readonly StageInfo _stageToLoad;
        
        private AsyncOperationHandle<SceneInstance> _loadStageHandle;
        private SceneInstance _sceneInstance;

        public StageLoader(StageInfo stageInfo)
        {
            _stageToLoad = stageInfo;
        }
        
        public async UniTask LoadStage()
        {
            _loadStageHandle = Addressables.LoadSceneAsync(_stageToLoad.Scene, LoadSceneMode.Additive);
            await _loadStageHandle.Task;

            _sceneInstance = _loadStageHandle.Result;

            var op = _sceneInstance.ActivateAsync();
            op.allowSceneActivation = true;
            await op;
            
            PrepareStage();
        }

        public GameObject[] GetSceneGameObjects()
        {
            return _sceneInstance.Scene.GetRootGameObjects();
        }

        private void PrepareStage()
        {
            //todo: find StageLinks and remove collected interaction items and etc.
            //spawn player at spawn point
            //or better move to StagesService?..
        }

        public void Dispose()
        {
            if (_loadStageHandle.IsValid())
            {
                Addressables.Release(_loadStageHandle);
            }
        }
    }
}