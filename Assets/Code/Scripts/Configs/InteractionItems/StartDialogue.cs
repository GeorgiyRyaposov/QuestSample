using Code.Scripts.App.Common;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "StartDialogue", menuName = "Configs/Interactions/StartDialogue")]
    public class StartDialogue : InteractionAction
    {
        public override string GetHint(InteractionItemInfo itemInfo)
        {
            //todo: replace hardcoded F 
            return "Press 'F' to begin dialogue";
        }

        public override UniTask Interact(InteractionItemInfo itemInfo)
        {
            if (itemInfo is not DialoguesInfo dialoguesInfo)
            {
                Debug.LogError($"Unsupported InteractionItemInfo {itemInfo}", itemInfo);
                return UniTask.CompletedTask;
            }
            
            Mediator.Get<DialoguesService>().StartDialogue(dialoguesInfo).Forget();
            return UniTask.CompletedTask;
        }
    }
}