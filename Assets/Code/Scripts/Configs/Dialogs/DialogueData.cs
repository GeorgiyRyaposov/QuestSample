using System;
using UnityEngine;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class DialogueData
    {
        public string Guid;
        public string SpeakerId;
        public string Text;
        public Vector2 NodePosition;
    }
}