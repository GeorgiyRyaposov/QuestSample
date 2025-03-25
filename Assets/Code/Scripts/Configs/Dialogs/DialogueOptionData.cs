using System;
using UnityEngine;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class DialogueOptionData
    {
        public string BaseDialogueGuid;
        public string TargetDialogueGuid;
        public string Text;
        public Vector2 NodePosition;
    }
}