﻿using System;
using Code.Scripts.Utils;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Configs/Interactions/CharacterInfo")]
    public class CharacterInfo : ScriptableObject
    {
        [ScriptableObjectId]
        public string Id;
        
        public LocalizedString LocalizedName; 
        
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