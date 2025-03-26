﻿using System;
using System.Collections.Generic;
using System.Linq;
using Code.Editor.DialogsEditor.Graph;
using Code.Editor.DialogsEditor.Nodes;
using Code.Editor.Utils;
using Code.Scripts.Configs.Blackboards;
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

        public void SaveGraph(string fileName, DialogueContainer container)
        {
            var existingContainer = AssetDatabaseUtils.FindAsset<DialogueContainer>(fileName);
            if (existingContainer == null)
            {
                var path = $"Assets/Configs/Data/Dialogues/{fileName}.asset";
                AssetDatabaseUtils.CreateAsset(container, path);
            }

            UpdateNodesData(container);
            
            AssetDatabase.SaveAssets();
        }

        public void LoadDialogues(DialogueContainer container)
        {
            _graphView.ResetGraph();
            LoadDialogueNodes(container);
            LoadCommentNodes(container);
            _graphView.HasChanges = false;
        }
        
        private void UpdateNodesData(DialogueContainer container)
        {
            var firstNode = GetOtherNode<DialogueNode>(_graphView.StartNode.outputContainer, _graphView.StartNode);
            if (firstNode != null)
            {
                container.StartDialogueGuid = firstNode.Guid;
            }
            
            container.DialoguesFlagsRequirements.Clear();
            container.OptionsFlagsRequirements.Clear();
            
            container.Options.Clear();
            foreach (var node in _graphView.nodes)
            {
                if (node is not DialogueOptionNode optionNode)
                {
                    continue;
                }
                
                var dialogueFrom = GetOtherNode<DialogueNode>(optionNode.inputContainer, optionNode);
                var dialogueTo = GetOtherNode<DialogueNode>(optionNode.outputContainer, optionNode);

                if (dialogueFrom == null)
                {
                    Debug.LogError($"Ответ диалога '{optionNode.title}' не содержит родительского диалога, свяжите ответ с диалогом");
                }
                
                container.Options.Add(new DialogueOptionData
                {
                    Guid = optionNode.Guid,
                    BaseDialogueGuid = dialogueFrom?.Guid,
                    TargetDialogueGuid = dialogueTo?.Guid,
                    Text = optionNode.Text,
                    NodePosition = node.GetPosition().position
                });

                if (optionNode.FlagRequirement.HasValue)
                {
                    container.OptionsFlagsRequirements.Add(new FlagRequirement
                    {
                        TargetId = optionNode.Guid,
                        Requirement = new BoolKeyValue
                        {
                            Key = optionNode.FlagRequirement.Value.Key,
                            Value = optionNode.FlagRequirement.Value.Value
                        }
                    });
                }
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
                
                if (dialogueNode.FlagRequirement.HasValue)
                {
                    container.DialoguesFlagsRequirements.Add(new FlagRequirement
                    {
                        TargetId = dialogueNode.Guid,
                        Requirement = new BoolKeyValue
                        {
                            Key = dialogueNode.FlagRequirement.Value.Key,
                            Value = dialogueNode.FlagRequirement.Value.Value
                        }
                    });
                }
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

                var requirement = dialogueContainer.DialoguesFlagsRequirements.FirstOrDefault(x => 
                    string.Equals(x.TargetId, dialogueNodeData.Guid, StringComparison.Ordinal));

                if (requirement != null)
                {
                    dialogueNode.FlagRequirement = requirement.Requirement;
                    _graphView.AddFlagRequirement(dialogueNode);
                }
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
                
                var requirement = dialogueContainer.OptionsFlagsRequirements.FirstOrDefault(x => 
                    string.Equals(x.TargetId, optionData.Guid, StringComparison.Ordinal));

                if (requirement != null)
                {
                    optionNode.FlagRequirement = requirement.Requirement;
                    _graphView.AddFlagRequirement(optionNode);
                }
            }
            
            if (!string.IsNullOrEmpty(dialogueContainer.StartDialogueGuid))
            {
                _graphView.ConnectNodes(_graphView.StartNode, dialogueNodesMap[dialogueContainer.StartDialogueGuid]);
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