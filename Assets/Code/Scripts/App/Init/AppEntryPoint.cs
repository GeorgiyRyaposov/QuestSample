using System;
using System.Collections.Generic;
using Code.Scripts.App.AppState;
using Code.Scripts.App.Common;
using Code.Scripts.App.ScenesManagement;
using Code.Scripts.Services;
using Code.Scripts.Services.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.App.Init
{
    public class AppEntryPoint : MonoBehaviour
    {
        [SerializeField] private ServicesContainer _servicesContainer;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            SetupServices();
            
            try
            {
                SetupAppState().Forget();
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }

        private static async UniTaskVoid SetupAppState()
        {
            var sceneName = ScenesConsts.GetCurrentScene();
            switch (sceneName)
            {
                case ScenesConsts.MainMenu or ScenesConsts.EmptyScene:
                    await Mediator.Get<AppStateMachine>().SetupFromMainMenu();
                    break;
                
                case ScenesConsts.Gameplay or ScenesConsts.GameplayGui:
                    await Mediator.Get<AppStateMachine>().SetupFromGameplay();
                    break;
                
                default:
                    Debug.LogWarning($"Unknown scene to start: {sceneName}");
                    break;
            }
        }

        private void SetupServices()
        {
            var locator = new ServiceLocator();
            
            var services = new List<IService>(_servicesContainer.ScriptableServices)
            {
                new GameStateService(),
                new InputService(),
                new InteractionsService(),
                new StageService(),
                
            };
            
            foreach (var service in services)
            {
                locator.Register(service);
            }
            
            foreach (var service in services)
            {
                if (service is IInitializable initializable)
                {
                    initializable.Initialize();
                }
            }
            
            Mediator.Setup(locator);
        }

        private void OnDestroy()
        {
            Mediator.Dispose();
        }

        // private void OnDrawGizmos()
        // {
        //     
        // }
        //
        // private void OnGUI()
        // {
        //     
        // }
        
        // private void OnApplicationFocus(bool hasFocus)
        // {
        //     SetCursorState(true);
        // }
        //
        // private void SetCursorState(bool isLocked)
        // {
        //     Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        // }
    }
}