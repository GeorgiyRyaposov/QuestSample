﻿using Code.Scripts.App.ScenesManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Code.Scripts.App.AppState
{
    //[CreateAssetMenu(menuName = "Data/AppState/PrepareGameState", fileName = "PrepareGameState")]
    public class PrepareGameState : AppState
    {
        [SerializeField] private AssetReference _gameplaySceneReference;
        [SerializeField] private AssetReference _gameplayUISceneReference;
        
        private AsyncOperationHandle<SceneInstance> _loadGuiSceneHandle;
        private AsyncOperationHandle<SceneInstance> _loadGameplaySceneHandle;

        public override async UniTask Enter()
        {
            await Preloader.Show(true);
            
            await Unload();

            if (!ScenesConsts.IsSceneLoaded(ScenesConsts.Gameplay))
            {
                await LoadGameplay();
            }

            if (!ScenesConsts.IsSceneLoaded(ScenesConsts.GameplayGui))
            {
                await LoadGameplayGUI();
            }
        }
        
        private async UniTask LoadGameplay()
        {
            _loadGameplaySceneHandle = Addressables.LoadSceneAsync(_gameplaySceneReference, LoadSceneMode.Single);
            await _loadGameplaySceneHandle.Task;

            var instance = _loadGameplaySceneHandle.Result;

            var op = instance.ActivateAsync();
            op.allowSceneActivation = true;
            await op;
        }
        
        private async UniTask LoadGameplayGUI()
        {
            _loadGuiSceneHandle = Addressables.LoadSceneAsync(_gameplayUISceneReference, LoadSceneMode.Additive);
            await _loadGuiSceneHandle.Task;

            var guiInstance = _loadGuiSceneHandle.Result;

            var op = guiInstance.ActivateAsync();
            op.allowSceneActivation = true;
            await op;
        }

        private async UniTask Unload()
        {
            if (_loadGameplaySceneHandle.IsValid())
            {
                var unloadOp = Addressables.UnloadSceneAsync(_loadGameplaySceneHandle);
                await unloadOp.Task;
            }

            if (_loadGuiSceneHandle.IsValid())
            {
                var unloadOp = Addressables.UnloadSceneAsync(_loadGuiSceneHandle);
                await unloadOp.Task;
            }
        }
    }
}