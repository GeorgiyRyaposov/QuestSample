using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public string StartDialogueGuid;
        
        public List<DialogueData> Dialogues = new();
        public List<DialogueOptionData> Options = new();
        public List<CommentData> Comments = new();
        
        public List<FlagRequirement> DialoguesFlagsRequirements = new();
        public List<FlagRequirement> OptionsFlagsRequirements = new();
        
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