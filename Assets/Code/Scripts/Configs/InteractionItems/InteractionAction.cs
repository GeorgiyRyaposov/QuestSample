using Code.Scripts.App.Common;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    public class InteractionAction : ScriptableObject
    {
        protected InteractionsService InteractionsService => Mediator.Get<InteractionsService>();
        
        public virtual string GetHint(InteractionItemInfo itemInfo)
        {
            return string.Empty;
        }
        
        public virtual UniTask Interact(InteractionItemInfo itemInfo)
        {
            return UniTask.CompletedTask;
        }
    }
}