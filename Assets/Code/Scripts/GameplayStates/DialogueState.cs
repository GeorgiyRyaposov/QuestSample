﻿using Code.Scripts.App.Common;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.GameplayStates
{
    public class DialogueState : State
    {
        public override UniTask Enter()
        {
            Mediator.HintsView.Hide();
            Mediator.Get<InputService>().EnableDialogueInput();
            return base.Enter();
        }

        public override void OnUpdate()
        {
            if (Mediator.InputState.DialogueClicked)
            {
                Mediator.InputState.DialogueClicked = false;

                if (Mediator.DialoguePanelView.IsDialogueTyping)
                {
                    Mediator.DialoguePanelView.SkipDialog();
                }
            }
        }

        public override async UniTask Exit()
        {
            Mediator.Get<InputService>().DisableDialogueInput();
            await Mediator.DialoguePanelView.Hide();
        }
    }
}