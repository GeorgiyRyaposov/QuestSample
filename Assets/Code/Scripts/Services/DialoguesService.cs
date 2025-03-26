﻿using System;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.Configs.Dialogs;
using Code.Scripts.Configs.InteractionItems;
using Code.Scripts.Services.Common;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.Services
{
    public class DialoguesService : IService
    {
        public async UniTaskVoid StartDialogue(DialoguesInfo dialoguesInfo)
        {
            //TODO: add check which dialogue available to start
            var dialogueInfo = dialoguesInfo.Dialogues.FirstOrDefault();

            await Mediator.GameplayStateMachine.EnterDialogueState();
            await Mediator.DialoguePanelView.ShowDialogue(dialogueInfo);
        }

        public void OnDialogCompleted(DialogueContainer dialogue)
        {
            Mediator.SessionState.CompletedDialogues.Add(dialogue.StartDialogueGuid);
            
            Mediator.GameplayStateMachine.ExitDialogueState().Forget();
        }

        public bool HasUncompletedRequirements(DialogueOptionData option, DialogueContainer dialogue)
        {
            var requirements = dialogue.OptionsFlagsRequirements.Where(x =>
                string.Equals(x.TargetId, option.Guid, StringComparison.Ordinal));

            foreach (var requirement in requirements)
            {
                var currentValue = Mediator.SessionState.BoolBlackboard.Get(requirement.Flag.Key);
                if (currentValue != requirement.Flag.Value)
                {
                    return true;
                }
            }

            var nextDialogueRequirements = dialogue.DialoguesFlagsRequirements.Where(x =>
                string.Equals(x.TargetId, option.TargetDialogueGuid, StringComparison.Ordinal));

            foreach (var dialogueRequirement in nextDialogueRequirements)
            {
                var currentValue = Mediator.SessionState.BoolBlackboard.Get(dialogueRequirement.Flag.Key);
                if (currentValue != dialogueRequirement.Flag.Value)
                {
                    return true;
                }
            }
            
            return false;
        }

        public void OnOptionSelected(DialogueOptionData optionData, DialogueContainer dialogue)
        {
            var flagModifier = dialogue.OptionsFlagsModifiers.FirstOrDefault(x =>
                string.Equals(x.TargetId, optionData.Guid, StringComparison.Ordinal));

            if (flagModifier != null)
            {
                Mediator.SessionState.BoolBlackboard.Set(flagModifier.Flag.Key, flagModifier.Flag.Value);
            }
        }
    }
}