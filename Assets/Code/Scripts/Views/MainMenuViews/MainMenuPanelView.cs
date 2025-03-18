using System;
using Code.Scripts.App.AppState;
using Code.Scripts.App.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Views.MainMenuViews
{
    public class MainMenuPanelView : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _exitButton;

        private void Start()
        {
            _startGameButton.onClick.AddListener(StartGame);
            _exitButton.onClick.AddListener(ExitGame);
        }

        private async void StartGame()
        {
            try
            {
                await Mediator.Get<AppStateMachine>().LoadGameplay();
            }
            catch (Exception e)
            {
                Debug.Log($"{e.Message}\n{e.StackTrace}");
            }
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}