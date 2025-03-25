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
    }
}