using System.Threading.Tasks;
using Code.Scripts.App.ScenesManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Code.Scripts.App.AppState
{
    [CreateAssetMenu(menuName = "Data/AppState/MainMenuState", fileName = "MainMenuState")]
    public class MainMenuState : AppState
    {
        [SerializeField] private AssetReference _mainMenuSceneReference;
        
        private AsyncOperationHandle<SceneInstance> _operationHandle;

        public override async Task Enter()
        {
            Preloader.Show(true);
            
            Unload();
            
            _operationHandle = Addressables.LoadSceneAsync(_mainMenuSceneReference, LoadSceneMode.Single);
            await _operationHandle.Task;
            
            Preloader.Hide();
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