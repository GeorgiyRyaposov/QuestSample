using System.Collections.Generic;
using Code.Editor.DialogsEditor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Editor.DialogsEditor.Graph
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow _window;
        private StoryGraphView _graphView;

        private Texture2D _indentationIcon;

        public void Configure(EditorWindow window, StoryGraphView graphView)
        {
            _window = window;
            _graphView = graphView;

            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeEntry(new GUIContent("Dialogue", _indentationIcon))
                {
                    level = 1, 
                    userData = new DialogueNode()
                },
                new SearchTreeEntry(new GUIContent("Option", _indentationIcon))
                {
                    level = 1, 
                    userData = new DialogueOptionNode()
                },
                new SearchTreeEntry(new GUIContent("Comment", _indentationIcon))
                {
                    level = 1,
                    userData = new StickyNote()
                }
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            
            switch (searchTreeEntry.userData)
            {
                case DialogueNode:
                    _graphView.AddDialogueNode("Dialogue Node", graphMousePosition);
                    return true;
                
                case DialogueOptionNode:
                    _graphView.AddOptionNode("Option", graphMousePosition);
                    return true;
                
                case StickyNote:
                    _graphView.CreateComment(graphMousePosition);
                    return true;
            }

            return false;
        }
    }
}