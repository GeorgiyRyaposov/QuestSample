using System;
using System.Collections.Generic;
using System.Linq;
using Code.Editor.DialogsEditor.Nodes;
using Code.Editor.Utils;
using Code.Scripts.Configs.Blackboards;
using Code.Scripts.Configs.Dialogs;
using Code.Scripts.Configs.InteractionItems;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using CharacterInfo = Code.Scripts.Configs.InteractionItems.CharacterInfo;

namespace Code.Editor.DialogsEditor.Graph
{
    public class DialogueGraphView : GraphView
    {
        public DialogueNode StartNode { get; private set; }
        public bool HasChanges { get; set; }
        
        private readonly CharactersContainer _characterContainer;
        private CharacterInfo[] Characters => _characterContainer.Characters;
        private readonly List<string> _characterNames;
        
        private NodeSearchWindow _searchWindow;
        private int _lastSelectedCharacterIndex;
        private readonly StyleSheet _nodeStyle;
        private readonly StyleSheet _flagRequirementStyleSheet;


        public DialogueGraphView(DialogueEditor editorWindow)
        {
            styleSheets.Add(AssetDatabaseUtils.FindAsset<StyleSheet>("DialogueEditorStyleSheet"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            AddBackground();
            AddStartNode();
            AddMiniMap();

            AddSearchWindow(editorWindow);

            _characterContainer = AssetDatabaseUtils.FindAsset<CharactersContainer>();
            _characterNames = _characterContainer.Characters.Select(x => x.CharacterName).ToList();

            _nodeStyle = AssetDatabaseUtils.FindAsset<StyleSheet>("NodeStyleSheet");
            _flagRequirementStyleSheet = AssetDatabaseUtils.FindAsset<StyleSheet>("FlagRequirementStyleSheet");
        }

        private void AddBackground()
        {
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
        }

        private void AddMiniMap()
        {
            var miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(10, 30, 150, 150));
            Add(miniMap);
        }


        private void AddSearchWindow(DialogueEditor editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            //DialogueOptionNode can be connected only to DialogueNode
            var onlyDialogNodes = startPort.node is DialogueOptionNode;

            foreach (var port in ports)
            {
                if (startPort == port || startPort.node == port.node)
                {
                    continue;
                }

                if (onlyDialogNodes && port.node is not DialogueNode)
                {
                    continue;
                }

                compatiblePorts.Add(port);
            }

            return compatiblePorts;
        }

        public StickyNote CreateComment(Vector2 position)
        {
            var commentNodeData = new CommentData
            {
                NodePosition = position,
                Text = string.Empty,
            };

            return CreateComment(commentNodeData);
        }

        public StickyNote CreateComment(CommentData commentData)
        {
            var note = new StickyNote(commentData.NodePosition)
            {
                contents = commentData.Text,
            };

            AddElement(note);
            
            HasChanges = true;
            
            return note;
        }

        public void AddDialogueNode(Vector2 position)
        {
            AddDialogueNode(new DialogueData
            {
                Guid = Guid.NewGuid().ToString(),
                NodePosition = position,
            });
        }

        public DialogueNode AddDialogueNode(DialogueData data)
        {
            var node = new DialogueNode
            {
                Guid = data.Guid,
                title = string.IsNullOrEmpty(data.Text) ? "Dialogue" : data.Text,
                Text = data.Text,
                SpeakerId = data.SpeakerId,
            };
            node.styleSheets.Add(_nodeStyle);
            
            AddPort("input", node, Direction.Input, Port.Capacity.Multi);
            AddPort("output", node, Direction.Output, Port.Capacity.Multi);

            node.SetPosition(new Rect(data.NodePosition, node.contentRect.size));
            
            //add character selection drop down
            var charactersSelector = new DropdownField(_characterNames, _lastSelectedCharacterIndex, selectedName =>
            {
                var index = GetCharacterIndex(selectedName);
                if (index == -1)
                {
                    return selectedName;
                }
                
                _lastSelectedCharacterIndex = index;
                
                node.SpeakerId = Characters[index].Id;

                return selectedName;
            });
            node.contentContainer.Add(charactersSelector);
            
            //add flag requirement
            var button = new Button(() =>
            {
                if (!node.FlagRequirement.HasValue)
                {
                    AddFlagRequirement(node);
                }
            })
            {
                text = "Добавить требование"
            };
            node.titleContainer.Add(button);

            //add text field
            var textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                node.Text = evt.newValue;
                node.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(node.Text);
            node.contentContainer.Add(textField);

            node.RefreshPorts();

            AddElement(node);

            HasChanges = true;
            
            return node;
        }

        private int GetCharacterIndex(string characterName)
        {
            for (int i = 0; i < Characters.Length; i++)
            {
                if (string.Equals(characterName, Characters[i].CharacterName, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddOptionNode(Vector2 position)
        {
            AddOptionNode(new DialogueOptionData
            {
                Guid = Guid.NewGuid().ToString(),
                NodePosition = position,
            });
        }

        public DialogueOptionNode AddOptionNode(DialogueOptionData data)
        {
            var node = new DialogueOptionNode
            {
                Guid = data.Guid,
                title = string.IsNullOrEmpty(data.Text) ? "Option" : data.Text,
                Text = data.Text,
            };
            node.styleSheets.Add(_nodeStyle);

            AddPort("input", node, Direction.Input, Port.Capacity.Multi);
            AddPort("output", node, Direction.Output, Port.Capacity.Single);

            node.SetPosition(new Rect(data.NodePosition, node.contentRect.size));

            var textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                node.Text = evt.newValue;
                node.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(node.Text);
            node.contentContainer.Add(textField);
            
            //add flag requirement
            var button = new Button(() =>
            {
                if (!node.FlagRequirement.HasValue)
                {
                    AddFlagRequirement(node);
                }
            })
            {
                text = "Добавить требование"
            };
            node.titleContainer.Add(button);

            node.RefreshPorts();

            AddElement(node);
            
            HasChanges = true;

            return node;
        }

        public void ResetGraph()
        {
            ClearNodes();
            AddStartNode();
            HasChanges = false;
        }

        private void ClearNodes()
        {
            foreach (var node in nodes.ToList())
            {
                RemoveElement(node);
            }

            foreach (var graphElement in graphElements.ToList())
            {
                RemoveElement(graphElement);
            }
        }

        public void AddFlagRequirement<T>(T node) where T : Node, IHasFlagRequirement
        {
            var root = new VisualElement();
            root.styleSheets.Add(_flagRequirementStyleSheet);
            
            var toggle = new Toggle("");
            toggle.RegisterValueChangedCallback(evt =>
            {
                node.FlagRequirement = new BoolKeyValue
                {
                    Key = node.FlagRequirement?.Key,
                    Value = evt.newValue,
                };
            });
            
            if (node.FlagRequirement.HasValue)
            {
                toggle.SetValueWithoutNotify(node.FlagRequirement.Value.Value);
            }
            root.Add(toggle);
            
            var button = new Button(() =>
            {
                node.FlagRequirement = null;
                node.Remove(root);
            })
            {
                text = "X"
            };
            root.Add(button);
            
            var textField = new TextField("Ключ");
            textField.RegisterValueChangedCallback(evt =>
            {
                node.FlagRequirement = new BoolKeyValue
                {
                    Key = evt.newValue,
                    Value = node.FlagRequirement?.Value ?? false,
                };
            });
            
            if (node.FlagRequirement.HasValue)
            {
                textField.SetValueWithoutNotify(node.FlagRequirement.Value.Key);
            }
            root.Add(textField);
            
            node.contentContainer.Add(root);
        }

        private DialogueNode AddStartNode()
        {
            var node = new DialogueNode
            {
                title = "START",
                Guid = Guid.NewGuid().ToString(),
                Text = "ENTRYPOINT",
                IsEntryPoint = true
            };

            AddPort("Next", node, Direction.Output, Port.Capacity.Single);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(100, 200, 100, 150));

            AddElement(node);

            StartNode = node;

            return node;
        }

        private Port AddPort(string portName, Node node, Direction nodeDirection, Port.Capacity capacity)
        {
            var port = node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));

            port.portName = portName;

            if (nodeDirection == Direction.Input)
            {
                node.inputContainer.Add(port);
            }
            else if (nodeDirection == Direction.Output)
            {
                node.outputContainer.Add(port);
            }

            return port;
        }

        public void ConnectNodes(Node output, Node input)
        {
            ConnectNodes(output.outputContainer[0].Q<Port>(), input.inputContainer[0].Q<Port>());
        }

        private void ConnectNodes(Port outputPort, Port inputPort)
        {
            var edge = new Edge
            {
                output = outputPort,
                input = inputPort
            };
            edge.input.Connect(edge);
            edge.output.Connect(edge);

            Add(edge);
        }
    }
}