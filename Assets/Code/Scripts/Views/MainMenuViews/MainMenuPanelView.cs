using System;
using Code.Scripts.App.AppState;
using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Views.MainMenuViews
{
    public class MainMenuPanelView : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _quitButton;
        
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private SaveMetadata _saveFile;

        private void Start()
        {
            _startGameButton.onClick.AddListener(StartGame);
            _continueGameButton.onClick.AddListener(ContinueGame);
            _loadGameButton.onClick.AddListener(ShowSaveFiles);
            _quitButton.onClick.AddListener(QuitGame);
            
            TryGetLatestSaveFile().Forget();
        }

        private async void StartGame()
        {
            try
            {
                await Mediator.Get<AppStateMachine>().StartNewGame();
            }
            catch (Exception e)
            {
                Debug.Log($"{e.Message}\n{e.StackTrace}");
            }
        }
        
        private async void ContinueGame()
        {
            await Mediator.Get<AppStateMachine>().LoadGameplay(_saveFile);
        }

        private async void ShowSaveFiles()
        {
            await Mediator.LoadMenuView.Show();
        }

        private void QuitGame()
        {
            Application.Quit();
        }

        private async UniTask TryGetLatestSaveFile()
        {
            _saveFile = await Mediator.Get<GameStateService>().GetLatestSaveFile();
            
            _continueGameButton.interactable = _saveFile != null;
        }
    }
}