using System.Collections.Generic;
using Code.Editor.DialogsEditor.Graph;
using Code.Editor.DialogsEditor.Nodes;
using Code.Editor.Utils;
using Code.Scripts.Configs.Dialogs;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Editor.DialogsEditor
{
    public class DialogueGraphSerializer
    {
        private readonly DialogueGraphView _graphView;

        public DialogueGraphSerializer(DialogueGraphView graphView)
        {
            _graphView = graphView;
        }

        public void SaveGraph(string fileName)
        {
            var container = AssetDatabaseUtils.FindAsset<DialogueContainer>(fileName);
            if (container == null)
            {
                container = ScriptableObject.CreateInstance<DialogueContainer>();
                var path = $"Assets/Configs/Data/Dialogues/{fileName}.asset";
                AssetDatabaseUtils.CreateAsset(container, path);
            }

            UpdateNodesData(container);
            
            AssetDatabase.SaveAssets();
        }

        public void LoadDialogues(string fileName)
        {
            var container = AssetDatabaseUtils.FindAsset<DialogueContainer>(fileName);
            if (container == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            _graphView.ResetGraph();
            LoadDialogueNodes(container);
            LoadCommentNodes(container);
        }
        
        private void UpdateNodesData(DialogueContainer container)
        {
            var firstNode = GetOtherNode<DialogueNode>(_graphView.StartNode.outputContainer, _graphView.StartNode);
            if (firstNode != null)
            {
                container.StartDialogueGuid = firstNode.Guid;
            }
            
            container.Options.Clear();
            foreach (var node in _graphView.nodes)
            {
                if (node is not DialogueOptionNode optionNode)
                {
                    continue;
                }
                
                var dialogueFrom = GetOtherNode<DialogueNode>(optionNode.inputContainer, optionNode);
                var dialogueTo = GetOtherNode<DialogueNode>(optionNode.outputContainer, optionNode);

                if (dialogueFrom == null || dialogueTo == null)
                {
                    Debug.LogError($"Dialog option '{optionNode.title}' missing base ({dialogueFrom == null}) or target ({dialogueTo == null}).");
                }
                
                container.Options.Add(new DialogueOptionData
                {
                    BaseDialogueGuid = dialogueFrom?.Guid,
                    TargetDialogueGuid = dialogueTo?.Guid,
                    Text = optionNode.Text,
                    NodePosition = node.GetPosition().position
                });
            }

            container.Dialogues.Clear();
            foreach (var node in _graphView.nodes)
            {
                if (node is not DialogueNode dialogueNode || dialogueNode.IsEntryPoint)
                {
                    continue;
                }
                
                container.Dialogues.Add(new DialogueData
                {
                    Guid = dialogueNode.Guid,
                    SpeakerId = dialogueNode.SpeakerId,
                    Text = dialogueNode.Text,
                    NodePosition = node.GetPosition().position,
                });
            }

            container.Comments.Clear();
            foreach (var graphElement in _graphView.graphElements)
            {
                if (graphElement is StickyNote stickyNote)
                {
                    container.Comments.Add(new CommentData
                    {
                        Text = stickyNote.contents,
                        NodePosition = stickyNote.GetPosition().position,
                    });
                }
            }
            
            EditorUtility.SetDirty(container);
        }

        private T GetOtherNode<T>(VisualElement element, Node ignoredNode) where T : DialogueNode
        {
            foreach (var connection in element[0].Q<Port>().connections)
            {
                if (connection.input.node != ignoredNode && connection.input.node is T input)
                {
                    return input;
                }
                
                if (connection.output.node != ignoredNode && connection.output.node is T output)
                {
                    return output;
                }
            }

            return null;
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        /// <param name="dialogueContainer"></param>
        private void LoadDialogueNodes(DialogueContainer dialogueContainer)
        {
            var dialogueNodesMap = new Dictionary<string, DialogueNode>();
            foreach (var dialogueNodeData in dialogueContainer.Dialogues)
            {
                var dialogueNode = _graphView.AddDialogueNode(dialogueNodeData);
                dialogueNodesMap[dialogueNodeData.Guid] = dialogueNode;
            }
            
            foreach (var optionData in dialogueContainer.Options)
            {
                var optionNode = _graphView.AddOptionNode(optionData);

                if (!string.IsNullOrEmpty(optionData.BaseDialogueGuid))
                {
                    _graphView.ConnectNodes(dialogueNodesMap[optionData.BaseDialogueGuid], optionNode);
                }

                if (!string.IsNullOrEmpty(optionData.TargetDialogueGuid))
                {
                    _graphView.ConnectNodes(optionNode, dialogueNodesMap[optionData.TargetDialogueGuid]);
                }
            }
        }

        private void LoadCommentNodes(DialogueContainer dialogueContainer)
        {
            foreach (var commentBlockData in dialogueContainer.Comments)
            {
               _graphView.CreateComment(commentBlockData);
            }
        }
    }
}