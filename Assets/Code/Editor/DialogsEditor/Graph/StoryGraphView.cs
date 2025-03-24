using System;
using System.Collections.Generic;
using Code.Editor.DialogsEditor.Nodes;
using Code.Scripts.Configs.Dialogs;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Editor.DialogsEditor.Graph
{
    public class StoryGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new(200, 150);
        
        private NodeSearchWindow _searchWindow;

        public StoryGraphView(StoryGraph editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            AddBackground();
            AddStartNode();

            AddSearchWindow(editorWindow);
        }

        private void AddBackground()
        {
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
        }


        private void AddSearchWindow(StoryGraph editorWindow)
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
            return note;
        }

        public void AddDialogueNode(string nodeName, Vector2 position)
        {
            AddDialogueNode(new DialogueData
            {
                Guid = Guid.NewGuid().ToString(),
                NodeName = nodeName,
                NodePosition = position,
            });
        }

        public DialogueNode AddDialogueNode(DialogueData data)
        {
            var node = new DialogueNode
            {
                Guid = data.Guid,
                title = data.NodeName,
                Text = data.Text,
                SpeakerId = data.SpeakerId,
            };
            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            
            AddPort("input", node, Direction.Input, Port.Capacity.Multi);
            AddPort("output", node, Direction.Output, Port.Capacity.Multi);
            
            node.SetPosition(new Rect(data.NodePosition, DefaultNodeSize)); 

            var textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                node.Text = evt.newValue;
            });
            textField.SetValueWithoutNotify(node.Text);
            node.mainContainer.Add(textField);
            
            node.RefreshExpandedState();
            node.RefreshPorts();
            
            AddElement(node);
            
            return node;
        }

        public void AddOptionNode(string nodeName, Vector2 position)
        {
            AddOptionNode(new DialogueOptionData
            {
                NodeName = nodeName,
                NodePosition = position,
            });
        }
        public DialogueOptionNode AddOptionNode(DialogueOptionData data)
        {
            var node = new DialogueOptionNode
            {
                title = data.NodeName,
                Text = data.Text,
            };
            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            
            AddPort("input", node, Direction.Input, Port.Capacity.Multi);
            AddPort("output", node, Direction.Output, Port.Capacity.Single);
            
            node.SetPosition(new Rect(data.NodePosition, DefaultNodeSize)); 

            var textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                node.Text = evt.newValue;
            });
            textField.SetValueWithoutNotify(node.Text);
            node.mainContainer.Add(textField);

            node.RefreshExpandedState();
            node.RefreshPorts();
            
            AddElement(node);
            
            return node;
        }

        public void ClearGraph()
        {
            AddBackground();
            AddStartNode();
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

            AddPort("Next", node, Direction.Output);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(100, 200, 100, 150));

            AddElement(node);
            
            return node;
        }

        private Port AddPort(string portName, Node node, Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
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

        public void ConnectNodes(DialogueNode output, DialogueOptionNode input)
        {
            ConnectNodes(output.outputContainer[0].Q<Port>(), input.inputContainer[0].Q<Port>());
        }
        public void ConnectNodes(DialogueOptionNode output, DialogueNode input)
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