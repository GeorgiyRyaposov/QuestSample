﻿using Code.Scripts.App.Common;
using Code.Scripts.App.ScenesManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Code.Scripts.App.AppState
{
    //[CreateAssetMenu(menuName = "Data/AppState/MainMenuState", fileName = "MainMenuState")]
    public class MainMenuState : AppState
    {
        [SerializeField] private AssetReference _mainMenuSceneReference;
        
        private AsyncOperationHandle<SceneInstance> _operationHandle;

        public override async UniTask Enter()
        {
            await Preloader.Show(true);
            
            Unload();
            
            _operationHandle = Addressables.LoadSceneAsync(_mainMenuSceneReference, LoadSceneMode.Single);
            await _operationHandle.Task;
            
            Mediator.SessionState.CurrentStageId = string.Empty;
            Mediator.SessionState.PreviousStageId = string.Empty;
            
            await Preloader.Hide();
        }

        private void Unload()
        {
            if (_operationHandle.IsValid())
            {
                Addressables.Release(_operationHandle);
            }
        }
    }
}