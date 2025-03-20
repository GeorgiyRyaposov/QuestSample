using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    public class InteractionAction : ScriptableObject
    {
        public virtual UniTask Interact()
        {
            return UniTask.CompletedTask;
        }
    }
}