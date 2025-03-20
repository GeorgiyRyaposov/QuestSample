using Code.Scripts.Utils;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    public class InteractionItemInfo : ScriptableObject
    {
        [ScriptableObjectId]
        public string Id;
        
        public InteractionAction Action;

        [Tooltip("More is better, item has more priority")]
        public int Priority;
    }
}