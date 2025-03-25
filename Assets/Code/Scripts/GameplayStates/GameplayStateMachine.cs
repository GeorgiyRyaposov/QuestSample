﻿using Code.Scripts.App.Common;
using Code.Scripts.Configs;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.GameplayStates
{
    public class GameplayStateMachine : MonoBehaviour
    {
        private readonly State _emptyState = new();
        private readonly State _dialogueState = new DialogueState();
        private readonly State _activeCharacterState = new ActivePlayerCharacterState();
        private readonly LoadStageState _loadingState = new ();
        private State _activeState = new();
        private State _prevState;

        public void Setup()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async UniTask EnterInitialState()
        {
            var stageInfo = Mediator.Get<StageService>().GetCurrentStageOrInitial();
            _loadingState.SetNextStage(stageInfo);
            
            await SetState(_loadingState);
            await SetState(_activeCharacterState);
        }
        
        public async UniTaskVoid GoToStage(StageInfo stageInfo)
        {
            _loadingState.SetNextStage(stageInfo);
            await SetState(_loadingState);
            await SetState(_activeCharacterState);
        }

        public async UniTask EnterDialogueState()
        {
            await SetState(_dialogueState);
        }

        public async UniTask ExitDialogueState()
        {
            await SetState(_activeCharacterState);
        }

        public async UniTask Dispose()
        {
            await _activeState.Exit();
            _activeState = _emptyState;
        }

        private async UniTask SetState(State state)
        {
            if (_activeState == state)
            {
                return;
            }
            
            _prevState = _activeState;
            _activeState = _emptyState;
            
            await _prevState.Exit();
            
            await state.Enter();
            _activeState = state;
        }

        private void Update()
        {
            _activeState.OnUpdate();
        }
    }
}