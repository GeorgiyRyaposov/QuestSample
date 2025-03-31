using System;
using Code.Scripts.Configs.Blackboards;
using UnityEditor.Experimental.GraphView;

namespace Code.Editor.DialogsEditor.Nodes
{
    public class DialogueNode : Node, IHasFlagRequirement
    {
        public string Guid;
        public string SpeakerId;
        public string TextId;
        public string TableId;
        
        public Nullable<BoolKeyValue> FlagRequirement { get; set; }
        public bool IsEntryPoint = false;
    }
}