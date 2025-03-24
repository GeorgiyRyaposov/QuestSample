using Code.Scripts.Components;
using Code.Scripts.GameplayStates;
using Code.Scripts.Persistence;
using Code.Scripts.Services.Common;
using Code.Scripts.Views.GameplayViews;
using UnityEngine;

namespace Code.Scripts.App.Common
{
    public static class Mediator
    {
        public static GameplayStateMachine GameplayStateMachine;
        public static PlayerCharacter PlayerCharacter;
        public static CharacterCinemachineWrapper PlayerCineMachine;

        public static HintsView HintsView;
        
        /// <summary>
        /// Do not cache, lazy ass edition
        /// </summary>
        public static GameStateData GameState { get; private set; }

        /// <summary>
        /// Do not cache, lazy ass edition
        /// </summary>
        public static SessionStateData SessionState { get; private set; } = new();
        public static InputState InputState { get; private set; }
        public static InputSettings InputSettings { get; private set; }

        private static ServiceLocator _serviceLocator;

        public static void Setup(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public static T Get<T>() where T : IService
        {
            return _serviceLocator.Get<T>();
        }

        public static void Dispose()
        {
            _serviceLocator.Dispose();
            _serviceLocator = null;
        }

        public static void SetStateLinks(GameStateData gameState)
        {
            GameState = gameState;
            SessionState = gameState.SessionState;
            InputState = gameState.InputState;
            InputSettings = gameState.InputSettings;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearValues()
        {
            GameplayStateMachine = null;
            PlayerCharacter = null;
            PlayerCineMachine = null;
            HintsView = null;
            GameState = null;
            SessionState = null;
            InputState = null;
            InputSettings = null;
            _serviceLocator = null;
        }
    }
}