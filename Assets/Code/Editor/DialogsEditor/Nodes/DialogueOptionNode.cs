using System;
using Code.Scripts.Configs.Blackboards;
using UnityEditor.Experimental.GraphView;

namespace Code.Editor.DialogsEditor.Nodes
{
    public class DialogueOptionNode : Node, IHasFlagRequirement
    {
        public string Guid;
        public string Text;
        public Nullable<BoolKeyValue> FlagRequirement { get; set; }
    }
}