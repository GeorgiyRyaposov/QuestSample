using Code.Scripts.App.AppState;
using Code.Scripts.App.Common;
using Code.Scripts.Services;
using Code.Scripts.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Views.GameplayViews
{
    public class InGameMenuView : MonoBehaviour
    {
        public bool IsVisible => _canvasGroup.alpha > 0;
        
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _closeMenuButton;
        [SerializeField] private Button _toMainMenuButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showDuration = 0.5f;

        private void Awake()
        {
            Mediator.InGameMenuView = this;
            
            _loadButton.onClick.AddListener(OnLoadClicked);
            _saveButton.onClick.AddListener(OnSaveClicked);
            _closeMenuButton.onClick.AddListener(OnCloseClicked);
            _toMainMenuButton.onClick.AddListener(OnToMainMenuClicked);
        }

        private void OnDestroy()
        {
            Mediator.InGameMenuView = null;
        }
        
        public async UniTask Show()
        {
            await CanvasGroupUtil.Show(_canvasGroup, _showDuration);
        }
        
        public async UniTask Hide()
        {
            await CanvasGroupUtil.Hide(_canvasGroup, _showDuration);
        }
        
        private void OnSaveClicked()
        {
            Hide().Forget();
            Mediator.Get<GameStateService>().SaveSessionStateAsync().Forget();
        }
        
        private void OnLoadClicked()
        {
            Hide().Forget();
            Mediator.LoadMenuView.Show().Forget();
        }
        

        private void OnToMainMenuClicked()
        {
            Mediator.Get<AppStateMachine>().ReturnToMainMenu().Forget();
        }

        private void OnCloseClicked()
        {
            Hide().Forget();
        }
    }
}