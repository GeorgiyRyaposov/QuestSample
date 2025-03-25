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
        public void StartDialogue(DialoguesInfo dialoguesInfo)
        {
            //TODO: add check which dialogue available to start
            var dialogueInfo = dialoguesInfo.Dialogues.FirstOrDefault();

            Mediator.DialoguePanelView.ShowDialogue(dialogueInfo).Forget();
        }

        public void OnDialogCompleted(DialogueContainer dialogue)
        {
            Mediator.SessionState.CompletedDialogues.Add(dialogue.StartDialogueGuid);
        }
    }
}