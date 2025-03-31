using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class DialogueOptionData
    {
        public string Guid;
        public string BaseDialogueGuid;
        public string TargetDialogueGuid;
        public LocalizedString LocalizedText;
        public Vector2 NodePosition;
    }
}