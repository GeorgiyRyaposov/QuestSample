using Code.Scripts.Configs.Dialogs;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Editor.DialogsEditor.Graph
{
    public class DialogueEditor : EditorWindow
    {
        private string _fileName = "New Narrative";

        private DialogueGraphView _graphView;
        private DialogueContainer _dialogueContainer;

        [MenuItem("Tools/DialogueEditor")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Narrative Graph");
        }

        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView(this)
            {
                name = "Narrative Graph",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(Save) { text = "Save Data" });
            toolbar.Add(new Button(Load) { text = "Load Data" });
            
            rootVisualElement.Add(toolbar);
        }

        private void Save()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var serializer = new DialogueGraphSerializer(_graphView);
                serializer.SaveGraph(_fileName);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }
        
        private void Load()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var serializer = new DialogueGraphSerializer(_graphView);
                serializer.LoadDialogues(_fileName);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}