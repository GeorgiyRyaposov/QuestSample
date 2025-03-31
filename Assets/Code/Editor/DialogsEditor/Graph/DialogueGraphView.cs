using System;
using System.Collections.Generic;
using System.Linq;
using Code.Editor.DialogsEditor.Nodes;
using Code.Editor.Utils;
using Code.Scripts.Configs.Blackboards;
using Code.Scripts.Configs.Dialogs;
using Code.Scripts.Configs.InteractionItems;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Localization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;
using CharacterInfo = Code.Scripts.Configs.InteractionItems.CharacterInfo;
using ObjectField = UnityEditor.UIElements.ObjectField;

namespace Code.Editor.DialogsEditor.Graph
{
    public class DialogueGraphView : GraphView
    {
        public DialogueNode StartNode { get; private set; }
        public bool HasChanges { get; set; }

        private const float _topPadding = 80f;
        
        private readonly Blackboard _blackboard = new();

        private readonly CharactersContainer _characterContainer;
        private readonly List<string> _characterNames;

        private NodeSearchWindow _searchWindow;
        private int _lastSelectedCharacterIndex;
        private readonly StringTable _localizationTable;
        private readonly StringTable _charactersTable;
        
        private readonly StyleSheet _nodeStyle;
        private readonly StyleSheet _flagRequirementStyleSheet;
        private readonly StyleSheet _flagModifierStyleSheet;
        private readonly TableReference _dialoguesTableName = "Dialogues";
        private readonly TableReference _charactersTableName = "Characters";


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

            _localizationTable = LoadStringTable(_dialoguesTableName);
            _charactersTable = LoadStringTable(_charactersTableName);
            
            _characterContainer = AssetDatabaseUtils.FindAsset<CharactersContainer>();
            _characterNames = _characterContainer.Characters.Select(x => 
                GetCharacterNameTranslation(x.LocalizedName)).ToList();

            _blackboard.SetPosition(new Rect(10, 20 + _topPadding, 300, 350));
            Add(_blackboard);
            

            _nodeStyle = AssetDatabaseUtils.FindAsset<StyleSheet>("NodeStyleSheet");
            _flagRequirementStyleSheet = AssetDatabaseUtils.FindAsset<StyleSheet>("FlagRequirementStyleSheet");
            _flagModifierStyleSheet = AssetDatabaseUtils.FindAsset<StyleSheet>("FlagModifierStyleSheet");
        }

        private StringTable LoadStringTable(string tableName)
        {
            var stringTable = LocalizationEditorSettings.GetStringTableCollection(tableName);
            var local = LocalizationEditorSettings.GetLocale("ru");
            return stringTable.GetTable(local.Identifier) as StringTable;
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
            miniMap.SetPosition(new Rect(10, 400 + _topPadding, 150, 150));
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
                title = data.LocalizedText?.TableEntryReference,
                TextId = data.LocalizedText?.TableEntryReference,
                TableId = data.LocalizedText?.TableReference,
                SpeakerId = data.SpeakerId,
            };
            node.styleSheets.Add(_nodeStyle);

            AddPort("input", node, Direction.Input, Port.Capacity.Multi);
            AddPort("output", node, Direction.Output, Port.Capacity.Multi);

            node.SetPosition(new Rect(data.NodePosition, node.contentRect.size));

            //add character selection drop down
            var charactersSelector = new DropdownField(_characterNames, _lastSelectedCharacterIndex, selectedName =>
            {
                var index = _characterNames.IndexOf(selectedName);
                if (index == -1)
                {
                    return selectedName;
                }

                _lastSelectedCharacterIndex = index;

                node.SpeakerId = _characterContainer.Characters[index].Id;

                return selectedName;
            });
            var characterInfo = _characterContainer.Characters.FirstOrDefault(x => x.Id == node.SpeakerId);
            if (characterInfo != null)
            {
                charactersSelector.SetValueWithoutNotify(GetCharacterNameTranslation(characterInfo.LocalizedName));
            }
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
            node.mainContainer.Add(button);

            var localizedTextLabel = new Label
            {
                text = GetDialogTranslation(node.TextId),
            };
            localizedTextLabel.AddToClassList("wrap");

            var textKeyField = new TextField();
            textKeyField.SetValueWithoutNotify(node.TextId);
            textKeyField.RegisterValueChangedCallback(evt =>
            {
                node.title = evt.newValue;
                node.TextId = evt.newValue;
                node.TableId = _dialoguesTableName;
                localizedTextLabel.text = GetDialogTranslation(evt.newValue);
            });
            node.contentContainer.Add(textKeyField);
            node.contentContainer.Add(localizedTextLabel);

            node.RefreshPorts();

            AddElement(node);

            HasChanges = true;

            return node;
        }

        private string GetDialogTranslation(string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                return "Введите ключ строки";
            }
            
            var value = _localizationTable.GetEntry(keyName)?.GetLocalizedString();
            return string.IsNullOrEmpty(value) ? "Строка не найдена" : value;
        }
        
        private string GetCharacterNameTranslation(LocalizedString localizedName)
        {
            var value = _charactersTable.GetEntry(localizedName.TableEntryReference.KeyId)?.GetLocalizedString();
            return string.IsNullOrEmpty(value) ? "Строка не найдена" : value;
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
                title = data.LocalizedText?.TableEntryReference,
                TextId = data.LocalizedText?.TableEntryReference,
                TableId = data.LocalizedText?.TableReference,
            };
            node.styleSheets.Add(_nodeStyle);

            AddPort("input", node, Direction.Input, Port.Capacity.Multi);
            AddPort("output", node, Direction.Output, Port.Capacity.Single);

            node.SetPosition(new Rect(data.NodePosition, node.contentRect.size));

            var localizedTextLabel = new Label
            {
                text = GetDialogTranslation(node.TextId),
            };
            localizedTextLabel.AddToClassList("wrap");

            var textKeyField = new TextField();
            textKeyField.SetValueWithoutNotify(node.TextId);
            textKeyField.RegisterValueChangedCallback(evt =>
            {
                node.title = evt.newValue;
                node.TextId = evt.newValue;
                node.TableId = _dialoguesTableName;
                localizedTextLabel.text = GetDialogTranslation(evt.newValue);
            });
            node.contentContainer.Add(textKeyField);
            node.contentContainer.Add(localizedTextLabel);

            //add flag requirement
            var addRequirementBtn = new Button(() =>
            {
                if (!node.FlagRequirement.HasValue)
                {
                    AddFlagRequirement(node);
                    HasChanges = true;
                }
            })
            {
                text = "Добавить требование"
            };
            node.contentContainer.Add(addRequirementBtn);

            //add flag modifier
            var addRequirementModifierBtn = new Button(() =>
            {
                if (string.IsNullOrEmpty(node.FlagModifier.Key))
                {
                    AddFlagModifier(node);
                    HasChanges = true;
                }
            })
            {
                text = "Изменить требование"
            };
            node.contentContainer.Add(addRequirementModifierBtn);

            node.RefreshPorts();

            AddElement(node);

            HasChanges = true;

            return node;
        }

        public void AddPropertiesToBlackBoard(DialogueContainer dialogueContainer)
        {
            var container = new VisualElement();

            var nameTextField = new TextField("Dialogue Name:")
            {
                value = dialogueContainer.DialogueName,
                bindingPath = "DialogueName",
            };
            container.Add(nameTextField);

            var afterDialogsTreePrefab = AssetDatabaseUtils.FindAsset<VisualTreeAsset>("AfterDialogsTree");
            var afterDialogsTree = afterDialogsTreePrefab.CloneTree();
            afterDialogsTree.Q<ListView>().makeItem = () => new ObjectField
                    {
                        objectType = typeof(DialogueContainer),
                    };
            container.Add(afterDialogsTree);

            container.Add(new Label("Доступен если флаг в указанном состоянии:"));
            var flagViewsTreePrefab = AssetDatabaseUtils.FindAsset<VisualTreeAsset>("FlagViewsTree");
            var flagViewPrefab = AssetDatabaseUtils.FindAsset<VisualTreeAsset>("FlagView");
            var flagViewsTree = flagViewsTreePrefab.CloneTree();
            flagViewsTree.Q<ListView>().makeItem = flagViewPrefab.CloneTree;
            container.Add(flagViewsTree);

            container.Bind(new SerializedObject(dialogueContainer));
            _blackboard.Add(container);
        }

        public void ResetGraph()
        {
            _blackboard.Clear();
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

            var textField = new TextField("Требует");
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

        public void AddFlagModifier(DialogueOptionNode node)
        {
            var root = new VisualElement();
            root.styleSheets.Add(_flagModifierStyleSheet);

            var toggle = new Toggle("");
            toggle.RegisterValueChangedCallback(evt =>
            {
                node.FlagModifier = new BoolKeyValue
                {
                    Key = node.FlagModifier.Key,
                    Value = evt.newValue,
                };
            });

            if (!string.IsNullOrEmpty(node.FlagModifier.Key))
            {
                toggle.SetValueWithoutNotify(node.FlagModifier.Value);
            }

            root.Add(toggle);

            var button = new Button(() =>
            {
                node.FlagModifier.Key = string.Empty;
                node.Remove(root);
            })
            {
                text = "X"
            };
            root.Add(button);

            var textField = new TextField("Установить");
            textField.RegisterValueChangedCallback(evt =>
            {
                node.FlagModifier = new BoolKeyValue
                {
                    Key = evt.newValue,
                    Value = node.FlagModifier.Value,
                };
            });

            if (!string.IsNullOrEmpty(node.FlagModifier.Key))
            {
                textField.SetValueWithoutNotify(node.FlagModifier.Key);
            }

            root.Add(textField);

            node.contentContainer.Add(root);
        }

        private void AddStartNode()
        {
            var node = new DialogueNode
            {
                title = "START",
                Guid = Guid.NewGuid().ToString(),
                TextId = "ENTRYPOINT",
                IsEntryPoint = true
            };

            AddPort("Next", node, Direction.Output, Port.Capacity.Single);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(300, 200, 100, 150));

            AddElement(node);

            StartNode = node;
        }

        private void AddPort(string portName, Node node, Direction nodeDirection, Port.Capacity capacity)
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