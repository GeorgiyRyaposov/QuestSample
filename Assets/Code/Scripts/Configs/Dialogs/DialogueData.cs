using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class DialogueData
    {
        public string Guid;
        public string SpeakerId;
        public LocalizedString LocalizedText;
        public Vector2 NodePosition;
    }
}