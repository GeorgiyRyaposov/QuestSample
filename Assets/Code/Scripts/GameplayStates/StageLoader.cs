using System;
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
        private readonly AssetReference _sceneToLoad;
        
        private AsyncOperationHandle<SceneInstance> _loadHandle;
        private SceneInstance _sceneInstance;
        private bool _isDisposed;

        public StageLoader(AssetReference scene)
        {
            _sceneToLoad = scene;
        }
        
        public async UniTask LoadStage()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("StageLoader is already disposed");
            }
            
            _loadHandle = Addressables.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
            await _loadHandle.ToUniTask();

            _sceneInstance = _loadHandle.Result;
        }

        public GameObject[] GetSceneGameObjects()
        {
            return _sceneInstance.Scene.GetRootGameObjects();
        }

        public async UniTask UnloadAsync()
        {
            if (_isDisposed || !_loadHandle.IsValid())
            {
                return;
            }
            
            var unloadHandle = Addressables.UnloadSceneAsync(_loadHandle);
            await unloadHandle.ToUniTask();
            
            if (unloadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Scene unload failed: {unloadHandle.OperationException}");
            }
            
            Dispose();
        }
        
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
        
            // Явно освобождаем ресурсы, если загрузка ещё не завершена
            if (!_loadHandle.IsDone)
            {
                Addressables.Release(_loadHandle);
            }

            _sceneInstance = default;
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
        
        ~StageLoader()
        {
            if (!_isDisposed)
            {
                Dispose();
            }
        }
    }
}