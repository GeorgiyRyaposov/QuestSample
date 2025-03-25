using System;
using Code.Scripts.Utils;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "InteractionItem", menuName = "Configs/Interactions/InteractionItem")]
    public class InteractionItemInfo : ScriptableObject
    {
        [ScriptableObjectId]
        public string Id;
        
        public InteractionAction Action;

        [Tooltip("More is better, item has more priority")]
        public int Priority;
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id) || Id == Guid.Empty.ToString())
            {
                if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this,
                        out var guid,
                        out long _))
                {
                    Id = guid;
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }
        #endif
    }
}