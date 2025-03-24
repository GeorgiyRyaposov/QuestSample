using UnityEditor.Experimental.GraphView;

namespace Code.Editor.DialogsEditor.Nodes
{
    public class DialogueNode : Node
    {
        public string Guid;
        public string SpeakerId;
        public string Text;
        public bool IsEntryPoint = false;
    }
}