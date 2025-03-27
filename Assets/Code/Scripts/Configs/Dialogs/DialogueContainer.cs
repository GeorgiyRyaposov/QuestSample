using System;
using System.Collections.Generic;
using Code.Scripts.Configs.Blackboards;
using UnityEngine;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public string StartDialogueGuid;
        
        [Tooltip("Можно стартовать после завершения указанных диалогов")]
        public DialogueContainer[] CanBeStartedAfterDialogues = new DialogueContainer[0];
        [Tooltip("Можно стартовать если активен какой-то флаг")]
        public BoolKeyValue[] CanBeStartedByFlags = new BoolKeyValue[0];
        
        public List<DialogueData> Dialogues = new();
        public List<DialogueOptionData> Options = new();
        public List<CommentData> Comments = new();
        
        public List<FlagRequirement> DialoguesFlagsRequirements = new();
        public List<FlagRequirement> OptionsFlagsRequirements = new();
        
        public List<FlagModifier> OptionsFlagsModifiers = new();
        
#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(DialogueContainer))]
        public class DialogueContainerEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                if (GUILayout.Button("Temp"))
                {
                    var container = (DialogueContainer)target;
                    foreach (var option in container.Options)
                    {
                        option.Guid = Guid.NewGuid().ToString();
                    }
                    
                    UnityEditor.EditorUtility.SetDirty(target);
                }

                DrawDefaultInspector();
            }
        }

#endif
    }
}