using System;
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
            var dialogueInfo = GetNextDialog(dialoguesInfo);

            await Mediator.GameplayStateMachine.EnterDialogueState();
            await Mediator.DialoguePanelView.ShowDialogue(dialogueInfo);
        }

        public bool HasAvailableDialog(DialoguesInfo dialoguesInfo)
        {
            return GetNextDialog(dialoguesInfo) != null;
        }

        private static DialogueContainer GetNextDialog(DialoguesInfo dialoguesInfo)
        {
            var blackboardService = Mediator.Get<BlackboardService>();
            foreach (var dialogue in dialoguesInfo.Dialogues)
            {
                if (IsDialogueCompleted(dialogue))
                {
                    continue;
                }
                
                //nothing is block from start the dialogue
                if (dialogue.CanBeStartedAfterDialogues.Count == 0 &&
                    dialogue.CanBeStartedByFlags.Count == 0)
                {
                    return dialogue;
                }

                foreach (var afterDialogue in dialogue.CanBeStartedAfterDialogues)
                {
                    if (IsDialogueCompleted(afterDialogue))
                    {
                        return dialogue;
                    }
                }

                foreach (var flagToStart in dialogue.CanBeStartedByFlags)
                {
                    if (blackboardService.IsFlagMatch(flagToStart))
                    {
                        return dialogue;
                    }
                }
            }
            
            return null;
        }

        private static bool IsDialogueCompleted(DialogueContainer dialogue)
        {
            return Mediator.SessionState.CompletedDialogues.Contains(dialogue.StartDialogueGuid);
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

            var blackboardService = Mediator.Get<BlackboardService>();
            foreach (var requirement in requirements)
            {
                if (!blackboardService.IsFlagMatch(requirement.Flag))
                {
                    return true;
                }
            }

            var nextDialogueRequirements = dialogue.DialoguesFlagsRequirements.Where(x =>
                string.Equals(x.TargetId, option.TargetDialogueGuid, StringComparison.Ordinal));

            foreach (var dialogueRequirement in nextDialogueRequirements)
            {
                if (!blackboardService.IsFlagMatch(dialogueRequirement.Flag))
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